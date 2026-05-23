using ICE.MechPilot;

namespace ICE.ConfigFiles;

public partial class Config
{
    /// <summary>
    /// Enables the scheduler gate that routes visible Mech runtime state into Mech Pilot.
    /// </summary>
    public bool MechPilotAutomationEnabled { get; set; } = false;

    /// <summary>
    /// Chooses how ICE exits Mech Pilot when the current mission cannot be automated safely.
    /// </summary>
    public MechFallbackMode MechPilotFallbackMode { get; set; } = MechFallbackMode.Manual;
}
