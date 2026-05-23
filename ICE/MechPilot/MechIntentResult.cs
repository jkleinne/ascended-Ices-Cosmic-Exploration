namespace ICE.MechPilot;

/// <summary>
/// Represents the outcome of executing one Mech Pilot intent at the controller boundary.
/// </summary>
internal sealed record MechIntentResult(bool IsComplete, string Message)
{
    public static MechIntentResult Waiting(string message) => new(false, message);
    public static MechIntentResult Complete(string message) => new(true, message);
}
