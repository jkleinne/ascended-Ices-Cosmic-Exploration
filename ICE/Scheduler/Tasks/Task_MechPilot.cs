using ICE.MechPilot;

namespace ICE.Scheduler.Tasks;

/// <summary>
/// Connects the Mech Pilot reader, decision engine, and controller to one scheduler tick.
/// </summary>
internal static class Task_MechPilot
{
    /// <summary>
    /// Queues one Mech Pilot controller pass so scheduler state remains centrally managed.
    /// </summary>
    public static void Enqueue()
    {
        P.TaskManager.Enqueue(() => ExecuteMechPilot(), "Running Mech Pilot controller");
    }

    /// <summary>
    /// Captures current runtime state and checks whether the scheduler can enter Mech Pilot.
    /// </summary>
    public static bool ShouldEnter()
    {
        if (!C.MechPilotAutomationEnabled)
            return false;

        var snapshot = MechMissionReader.ReadSnapshot();
        MechDebugState.CaptureSnapshot(snapshot);
        return IsMechRuntimeVisible(snapshot);
    }

    private static bool? ExecuteMechPilot()
    {
        var snapshot = MechMissionReader.ReadSnapshot();
        MechDebugState.CaptureSnapshot(snapshot);

        var profile = MechMissionCatalog.FindProfile(snapshot.CurrentMissionId);
        var intent = MechDecisionEngine.Decide(snapshot, profile, C.MechPilotFallbackMode);
        MechDebugState.CaptureIntent(intent);

        var controller = new MechPilotController(new SchedulerMechPilotActions());
        var result = controller.Execute(intent);
        MechDebugState.CaptureResult(result);

        return result.IsComplete;
    }

    private static bool IsMechRuntimeVisible(MechMissionSnapshot snapshot)
    {
        return snapshot.WorldEvent.IsMechRuntimeEvidence()
               || snapshot.IsBoardingAddonVisible
               || snapshot.IsRecordAddonVisible;
    }

    private sealed class SchedulerMechPilotActions : IMechPilotActions
    {
        public void StopMovement()
        {
            if (!P.Navmesh.Installed)
                return;

            if (P.Navmesh.IsRunning())
                P.Navmesh.Stop();
        }

        public void EnterManualMode()
        {
            SchedulerMain.State = IceState.ManualMode;
        }

        public void EnterAbandonMission()
        {
            SchedulerMain.State = IceState.AbandonMission;
        }
    }
}
