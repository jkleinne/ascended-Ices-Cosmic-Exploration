namespace ICE.Scheduler.Tasks.OldTask
{
    internal class TaskManualMode
    {
        public static void ZenMode()
        {
            if (CosmicHelper.CurrentLunarMission == 0)
            {
                SchedulerMain.State = IceState.GrabMission;
            }
            if (!OldConfig.Missions.SingleOrDefault(x => x.Id == CosmicHelper.CurrentLunarMission).ManualMode && !OldConfig.OnlyGrabMission)
            {
                SchedulerMain.State &= ~IceState.ManualMode;
            }
        }
    }
}
