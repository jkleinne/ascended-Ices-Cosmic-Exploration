namespace ICE.MechPilot;

/// <summary>
/// Provides the scheduler side effects that Mech Pilot may request after pure intent selection.
/// </summary>
internal interface IMechPilotActions
{
    /// <summary>
    /// Stops movement before Mech Pilot hands control to another scheduler state.
    /// </summary>
    void StopMovement();

    /// <summary>
    /// Enters manual mode so the player can finish the mission directly.
    /// </summary>
    void EnterManualMode();

    /// <summary>
    /// Enters the existing abandon mission flow when automation cannot continue safely.
    /// </summary>
    void EnterAbandonMission();
}

/// <summary>
/// Executes Mech Pilot intents through injected actions so scheduler side effects stay testable.
/// </summary>
internal sealed class MechPilotController
{
    private const string MovementStopFailurePrefix = "Mech Pilot could not stop movement";
    private readonly IMechPilotActions actions;

    /// <summary>
    /// Accepts the action boundary used to perform scheduler and movement side effects.
    /// </summary>
    public MechPilotController(IMechPilotActions actions)
    {
        this.actions = actions ?? throw new ArgumentNullException(nameof(actions));
    }

    /// <summary>
    /// Performs the side effects requested by one intent and returns the controller outcome.
    /// </summary>
    public MechIntentResult Execute(MechIntent intent)
    {
        ArgumentNullException.ThrowIfNull(intent);

        switch (intent.Kind)
        {
            case MechIntentKind.Wait:
                return MechIntentResult.Waiting(intent.Reason);

            case MechIntentKind.ManualFallback:
                var manualReason = StopMovementAndBuildResultReason(intent.Reason);
                actions.EnterManualMode();
                return MechIntentResult.Complete(manualReason);

            case MechIntentKind.Abandon:
                var abandonReason = StopMovementAndBuildResultReason(intent.Reason);
                actions.EnterAbandonMission();
                return MechIntentResult.Complete(abandonReason);

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(intent),
                    intent.Kind,
                    "Unknown Mech Pilot intent kind.");
        }
    }

    private string StopMovementAndBuildResultReason(string reason)
    {
        try
        {
            actions.StopMovement();
            return reason;
        }
        catch (Exception ex)
        {
            return $"{reason} ({MovementStopFailurePrefix}: {ex.Message})";
        }
    }
}
