using ICE.MechPilot;
using Xunit;

namespace ICE.Tests.MechPilot;

public sealed class MechWorldEventTests
{
    [Fact]
    public void IsMechRuntimeEvidence_ReturnsTrueOnlyForMechOpsEvents()
    {
        (MechWorldEvent WorldEvent, bool Expected)[] cases =
        [
            (MechWorldEvent.MechOpsCommenced, true),
            (MechWorldEvent.MechOpsIssues, true),
            (MechWorldEvent.MechOpsDeploying, true),
            (MechWorldEvent.RedAlertIncoming, false),
            (MechWorldEvent.RedAlertProgressing, false),
            (MechWorldEvent.WaitingForDevelopmentStage, false),
            (MechWorldEvent.Unknown, false),
        ];

        foreach (var testCase in cases)
        {
            Assert.Equal(testCase.Expected, testCase.WorldEvent.IsMechRuntimeEvidence());
        }
    }
}
