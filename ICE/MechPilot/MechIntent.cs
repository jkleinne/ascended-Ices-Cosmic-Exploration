namespace ICE.MechPilot;

/// <summary>
/// Represents a pure Mech Pilot action request before any controller side effects run.
/// </summary>
internal sealed record MechIntent(MechIntentKind Kind, string Reason)
{
    public static MechIntent Wait(string reason) => new(MechIntentKind.Wait, reason);
    public static MechIntent ManualFallback(string reason) => new(MechIntentKind.ManualFallback, reason);
    public static MechIntent Abandon(string reason) => new(MechIntentKind.Abandon, reason);
}
