using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game.WKS;
using ICE.Utilities.Cosmic_Helper;

namespace ICE.MechPilot;

/// <summary>
/// Reads live game state into value snapshots so Mech decision logic stays deterministic.
/// </summary>
internal static class MechMissionReader
{
    private const uint MissingMissionId = 0;
    private const int MaxTargetCandidates = 12;
    private const float MaxTargetDistance = 50.0f;
    private const string UnsupportedReasonSeparator = "; ";
    private const string WksHudAddonName = "WKSHud";
    private const string BoardingAddonName = "WKSMechaBoarding";
    private const string RecordAddonName = "WKSMechaRecord";
    private const string MissionInfoAddonName = "WKSMissionInfomation";

    /// <summary>
    /// Captures the current Mech adjacent runtime state without exposing live services to pure logic.
    /// </summary>
    public static MechMissionSnapshot ReadSnapshot()
    {
        var unsupportedReasons = new List<string>();
        var localPlayer = PlayerHelper.LocalPlayer;
        var playerPosition = localPlayer?.Position ?? Vector3.Zero;
        var isPlayerAvailable = localPlayer is not null && ReadIsPlayerAvailable(unsupportedReasons);
        var worldEventInfo = ReadWorldEventInfo(unsupportedReasons);
        var currentMissionId = ReadCurrentMissionId(unsupportedReasons);
        var targets = localPlayer is null
            ? Array.Empty<MechTargetSnapshot>()
            : ReadTargetCandidates(playerPosition, unsupportedReasons);

        if (localPlayer is null)
        {
            unsupportedReasons.Add("Local player is unavailable");
        }
        else if (!isPlayerAvailable)
        {
            unsupportedReasons.Add("Player is not available");
        }

        return new MechMissionSnapshot(
            currentMissionId,
            worldEventInfo.WorldEvent,
            worldEventInfo.EndTimestamp,
            ReadCurrentScore(unsupportedReasons),
            playerPosition,
            IsPlayerAvailable: isPlayerAvailable,
            IsPlayerBusy: localPlayer is not null && ReadIsPlayerBusy(unsupportedReasons),
            IsNavmeshRunning: ReadIsNavmeshRunning(unsupportedReasons),
            IsWksHudVisible: ReadAddonVisibility(WksHudAddonName, unsupportedReasons),
            IsBoardingAddonVisible: ReadAddonVisibility(BoardingAddonName, unsupportedReasons),
            IsRecordAddonVisible: ReadAddonVisibility(RecordAddonName, unsupportedReasons),
            IsMissionInfoVisible: ReadAddonVisibility(MissionInfoAddonName, unsupportedReasons),
            Targets: targets,
            SelectedTarget: ReadSelectedTarget(playerPosition, unsupportedReasons),
            UnsupportedReason: FormatUnsupportedReason(unsupportedReasons));
    }

    private static bool ReadIsPlayerAvailable(List<string> unsupportedReasons)
    {
        try
        {
            return Player.Available;
        }
        catch (Exception ex) when (ex is AccessViolationException or NullReferenceException or InvalidOperationException)
        {
            unsupportedReasons.Add($"Player availability read failed: {ex.Message}");
            return false;
        }
    }

    private static (MechWorldEvent WorldEvent, uint EndTimestamp) ReadWorldEventInfo(List<string> unsupportedReasons)
    {
        try
        {
            (CosmicHandler.WKSEvents wksEvent, uint timer)? eventInfo;
            unsafe
            {
                eventInfo = CosmicHandler.EventInfo();
            }

            if (eventInfo is null)
            {
                unsupportedReasons.Add("WKS event info is unavailable");
                return (MechWorldEvent.Unknown, EndTimestamp: 0);
            }

            return (MapWorldEvent(eventInfo.Value.wksEvent), eventInfo.Value.timer);
        }
        catch (Exception ex) when (ex is AccessViolationException or NullReferenceException or InvalidOperationException)
        {
            unsupportedReasons.Add($"WKS event info read failed: {ex.Message}");
            return (MechWorldEvent.Unknown, EndTimestamp: 0);
        }
    }

    private static uint ReadCurrentMissionId(List<string> unsupportedReasons)
    {
        try
        {
            uint missionId;
            unsafe
            {
                missionId = CosmicHelper.CurrentLunarMission;
            }

            if (missionId == MissingMissionId)
                unsupportedReasons.Add("Current lunar mission is unavailable");

            return missionId;
        }
        catch (Exception ex) when (ex is AccessViolationException or NullReferenceException or InvalidOperationException)
        {
            unsupportedReasons.Add($"Current lunar mission read failed: {ex.Message}");
            return MissingMissionId;
        }
    }

    private static uint ReadCurrentScore(List<string> unsupportedReasons)
    {
        try
        {
            unsafe
            {
                var manager = WKSManager.Instance();
                if (manager is null)
                {
                    unsupportedReasons.Add("Current score is unavailable");
                    return 0;
                }

                return manager->CurrentScore;
            }
        }
        catch (Exception ex) when (ex is AccessViolationException or NullReferenceException or InvalidOperationException)
        {
            unsupportedReasons.Add($"Current score read failed: {ex.Message}");
            return 0;
        }
    }

    private static bool ReadIsPlayerBusy(List<string> unsupportedReasons)
    {
        try
        {
            return PlayerHelper.CustomIsBusy;
        }
        catch (Exception ex) when (ex is AccessViolationException or NullReferenceException or InvalidOperationException)
        {
            unsupportedReasons.Add($"Player busy state read failed: {ex.Message}");
            return true;
        }
    }

    private static bool ReadIsNavmeshRunning(List<string> unsupportedReasons)
    {
        try
        {
            if (!P.Navmesh.Installed)
            {
                unsupportedReasons.Add("Navmesh is not installed");
                return false;
            }

            return P.Navmesh.IsRunning();
        }
        catch (Exception ex) when (ex is NullReferenceException or InvalidOperationException)
        {
            unsupportedReasons.Add($"Navmesh state read failed: {ex.Message}");
            return false;
        }
    }

    private static bool ReadAddonVisibility(string addonName, List<string> unsupportedReasons)
    {
        try
        {
            return AddonHelper.IsAddonActive(addonName);
        }
        catch (Exception ex) when (ex is AccessViolationException or NullReferenceException or InvalidOperationException)
        {
            unsupportedReasons.Add($"Addon {addonName} visibility read failed: {ex.Message}");
            return false;
        }
    }

    private static IReadOnlyList<MechTargetSnapshot> ReadTargetCandidates(
        Vector3 playerPosition,
        List<string> unsupportedReasons)
    {
        try
        {
            return Svc.Objects
                .Where(gameObject => gameObject is not null && gameObject.IsTargetable)
                .Select(gameObject => CreateTargetSnapshot(gameObject, playerPosition))
                .Where(target => target.Distance <= MaxTargetDistance)
                .OrderBy(target => target.Distance)
                .Take(MaxTargetCandidates)
                .ToArray();
        }
        catch (Exception ex) when (ex is AccessViolationException or NullReferenceException or InvalidOperationException)
        {
            unsupportedReasons.Add($"Target candidate read failed: {ex.Message}");
            return Array.Empty<MechTargetSnapshot>();
        }
    }

    private static MechTargetSnapshot? ReadSelectedTarget(Vector3 playerPosition, List<string> unsupportedReasons)
    {
        try
        {
            return Svc.Targets.Target is { } target
                ? CreateTargetSnapshot(target, playerPosition)
                : null;
        }
        catch (Exception ex) when (ex is AccessViolationException or NullReferenceException or InvalidOperationException)
        {
            unsupportedReasons.Add($"Selected target read failed: {ex.Message}");
            return null;
        }
    }

    private static MechTargetSnapshot CreateTargetSnapshot(IGameObject gameObject, Vector3 playerPosition)
    {
        return new MechTargetSnapshot(
            gameObject.GameObjectId,
            unchecked((uint)gameObject.BaseId),
            gameObject.Name.ToString(),
            gameObject.Position,
            Vector3.Distance(playerPosition, gameObject.Position),
            gameObject.IsTargetable);
    }

    private static MechWorldEvent MapWorldEvent(CosmicHandler.WKSEvents wksEvent)
    {
        return wksEvent switch
        {
            CosmicHandler.WKSEvents.Mechops_Commenced => MechWorldEvent.MechOpsCommenced,
            CosmicHandler.WKSEvents.RedAlert_Incoming => MechWorldEvent.RedAlertIncoming,
            CosmicHandler.WKSEvents.RedAlert_Progressing => MechWorldEvent.RedAlertProgressing,
            CosmicHandler.WKSEvents.MechOps_Issues => MechWorldEvent.MechOpsIssues,
            CosmicHandler.WKSEvents.MechOps_Deploying => MechWorldEvent.MechOpsDeploying,
            CosmicHandler.WKSEvents.WaitingforDevStage => MechWorldEvent.WaitingForDevelopmentStage,
            _ => MechWorldEvent.Unknown,
        };
    }

    private static string? FormatUnsupportedReason(IReadOnlyCollection<string> unsupportedReasons)
    {
        return unsupportedReasons.Count == 0
            ? null
            : string.Join(UnsupportedReasonSeparator, unsupportedReasons);
    }
}
