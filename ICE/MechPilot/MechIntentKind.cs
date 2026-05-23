namespace ICE.MechPilot;

/// <summary>
/// Describes the action category a Mech controller may execute.
/// </summary>
internal enum MechIntentKind
{
    Wait = 0,
    ManualFallback = 1,
    Abandon = 2,
}
