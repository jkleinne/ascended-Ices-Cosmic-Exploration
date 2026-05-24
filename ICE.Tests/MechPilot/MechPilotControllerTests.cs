using ICE.MechPilot;
using Xunit;

namespace ICE.Tests.MechPilot;

public sealed class MechPilotControllerTests
{
    private const string WaitReason = "Mech instrumentation is waiting";
    private const string ManualFallbackReason = "No Mech profile is recorded";
    private const string AbandonReason = "Mech state is unsupported";
    private const string MovementStopFailure = "Navmesh IPC failed";

    [Fact]
    public void Execute_WithWaitIntent_CompletesSchedulerTaskWithoutSideEffects()
    {
        var actions = new FakeMechPilotActions();
        var controller = new MechPilotController(actions);

        var result = controller.Execute(MechIntent.Wait(WaitReason));

        Assert.True(result.IsComplete);
        Assert.Equal(WaitReason, result.Message);
        Assert.Equal(0, actions.StopMovementCalls);
        Assert.False(actions.EnteredManualMode);
        Assert.False(actions.EnteredAbandonMission);
    }

    [Fact]
    public void Execute_WithManualFallback_StopsMovementAndEntersManualMode()
    {
        var actions = new FakeMechPilotActions();
        var controller = new MechPilotController(actions);

        var result = controller.Execute(MechIntent.ManualFallback(ManualFallbackReason));

        Assert.True(result.IsComplete);
        Assert.Equal(ManualFallbackReason, result.Message);
        Assert.Equal(1, actions.StopMovementCalls);
        Assert.True(actions.EnteredManualMode);
        Assert.False(actions.EnteredAbandonMission);
    }

    [Fact]
    public void Execute_WithManualFallbackAndMovementStopFailure_StillEntersManualMode()
    {
        var actions = new FakeMechPilotActions
        {
            StopMovementException = new InvalidOperationException(MovementStopFailure),
        };
        var controller = new MechPilotController(actions);

        MechIntentResult? result = null;
        var exception = Record.Exception(() =>
        {
            result = controller.Execute(MechIntent.ManualFallback(ManualFallbackReason));
        });

        Assert.Null(exception);
        Assert.NotNull(result);
        Assert.True(result.IsComplete);
        Assert.Contains(ManualFallbackReason, result.Message);
        Assert.Contains(MovementStopFailure, result.Message);
        Assert.True(actions.EnteredManualMode);
        Assert.False(actions.EnteredAbandonMission);
    }

    [Fact]
    public void Execute_WithAbandonIntent_StopsMovementAndEntersAbandonMission()
    {
        var actions = new FakeMechPilotActions();
        var controller = new MechPilotController(actions);

        var result = controller.Execute(MechIntent.Abandon(AbandonReason));

        Assert.True(result.IsComplete);
        Assert.Equal(AbandonReason, result.Message);
        Assert.Equal(1, actions.StopMovementCalls);
        Assert.False(actions.EnteredManualMode);
        Assert.True(actions.EnteredAbandonMission);
    }

    private sealed class FakeMechPilotActions : IMechPilotActions
    {
        public int StopMovementCalls { get; private set; }
        public bool EnteredManualMode { get; private set; }
        public bool EnteredAbandonMission { get; private set; }
        public Exception? StopMovementException { get; init; }

        public void StopMovement()
        {
            StopMovementCalls++;
            if (StopMovementException is not null)
                throw StopMovementException;
        }

        public void EnterManualMode()
        {
            EnteredManualMode = true;
        }

        public void EnterAbandonMission()
        {
            EnteredAbandonMission = true;
        }
    }
}
