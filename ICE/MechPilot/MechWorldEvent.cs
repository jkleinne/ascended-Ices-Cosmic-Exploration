namespace ICE.MechPilot;

/// <summary>
/// Represents WKS world event states that matter to Mech Pilot decisions without exposing scheduler handler types to pure logic.
/// </summary>
internal enum MechWorldEvent
{
    Unknown = 0,
    MechOpsCommenced = 1,
    RedAlertIncoming = 2,
    RedAlertProgressing = 3,
    MechOpsIssues = 4,
    MechOpsDeploying = 5,
    WaitingForDevelopmentStage = 6,
}
