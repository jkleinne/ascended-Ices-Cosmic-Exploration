namespace ICE.MechPilot;

/// <summary>
/// Defines how ICE leaves Mech Pilot automation when it cannot proceed safely.
/// </summary>
public enum MechFallbackMode
{
    /// <summary>
    /// Leaves the scheduler in manual mode so the player can finish the Mech mission directly.
    /// </summary>
    Manual = 0,

    /// <summary>
    /// Routes the scheduler to the existing abandon flow when Mech automation cannot proceed safely.
    /// </summary>
    Abandon = 1,
}
