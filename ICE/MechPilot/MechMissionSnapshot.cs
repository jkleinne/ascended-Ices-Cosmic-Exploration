using System.Collections.Generic;
using System.Numerics;

namespace ICE.MechPilot;

/// <summary>
/// Captures the observable Mech Pilot runtime state used by pure decision logic.
/// </summary>
internal sealed record MechMissionSnapshot(
    uint CurrentMissionId,
    MechWorldEvent WorldEvent,
    uint WorldEventEndTimestamp,
    uint CurrentScore,
    Vector3 PlayerPosition,
    bool IsPlayerAvailable,
    bool IsPlayerBusy,
    bool IsNavmeshRunning,
    bool IsWksHudVisible,
    bool IsBoardingAddonVisible,
    bool IsRecordAddonVisible,
    bool IsMissionInfoVisible,
    IReadOnlyList<MechTargetSnapshot> Targets,
    MechTargetSnapshot? SelectedTarget,
    string? UnsupportedReason)
{
    public bool HasUnsupportedReason => !string.IsNullOrWhiteSpace(UnsupportedReason);
}
