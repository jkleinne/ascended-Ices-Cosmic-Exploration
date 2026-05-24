namespace ICE.MechPilot;

/// <summary>
/// Selects safe instrumentation-phase Mech Pilot intents from snapshot values.
/// </summary>
internal static class MechDecisionEngine
{
    public static MechIntent Decide(
        MechMissionSnapshot snapshot,
        MechMissionProfile? profile,
        MechFallbackMode fallbackMode)
    {
        if (snapshot.HasUnsupportedReason)
        {
            return fallbackMode == MechFallbackMode.Abandon
                ? MechIntent.Abandon($"Mech Pilot cannot continue: {snapshot.UnsupportedReason}")
                : MechIntent.ManualFallback($"Mech Pilot cannot continue: {snapshot.UnsupportedReason}");
        }

        if (profile is null || profile.MissionId != snapshot.CurrentMissionId)
        {
            var reason = $"No Mech profile is recorded for mission {snapshot.CurrentMissionId}";
            return fallbackMode == MechFallbackMode.Abandon
                ? MechIntent.Abandon(reason)
                : MechIntent.ManualFallback(reason);
        }

        return MechIntent.Wait($"Mech profile is recorded for mission {profile.MissionId}. Instrumentation is waiting for captured tactics.");
    }
}
