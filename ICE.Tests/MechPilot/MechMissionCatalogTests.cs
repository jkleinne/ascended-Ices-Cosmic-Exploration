using ICE.MechPilot;
using Xunit;

namespace ICE.Tests.MechPilot;

public sealed class MechMissionCatalogTests
{
    private const uint UnknownMissionId = 1234;

    [Fact]
    public void FindProfile_WithUnknownMission_ReturnsNoProfile()
    {
        var profile = MechMissionCatalog.FindProfile(UnknownMissionId);

        Assert.Null(profile);
    }
}
