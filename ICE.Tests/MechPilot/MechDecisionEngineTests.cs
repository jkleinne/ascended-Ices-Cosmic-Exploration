using System.Numerics;
using ICE.MechPilot;
using Xunit;

namespace ICE.Tests.MechPilot;

public sealed class MechDecisionEngineTests
{
    private const uint RecordedMissionId = 1234;
    private const string RecordedMissionName = "Recorded mission";
    private const string RecordedMissionNotes = "Instrumentation profile";
    private const string UnsupportedReason = "mission state is unsupported";

    [Fact]
    public void Decide_WithUnsupportedSnapshotAndManualFallback_ReturnsManualFallback()
    {
        var snapshot = CreateSnapshot(UnsupportedReason);
        var profile = CreateProfile();

        var intent = MechDecisionEngine.Decide(snapshot, profile, MechFallbackMode.Manual);

        Assert.Equal(MechIntentKind.ManualFallback, intent.Kind);
        Assert.Equal($"Mech Pilot cannot continue: {UnsupportedReason}", intent.Reason);
    }

    [Fact]
    public void Decide_WithUnsupportedSnapshotAndAbandonFallback_ReturnsAbandon()
    {
        var snapshot = CreateSnapshot(UnsupportedReason);
        var profile = CreateProfile();

        var intent = MechDecisionEngine.Decide(snapshot, profile, MechFallbackMode.Abandon);

        Assert.Equal(MechIntentKind.Abandon, intent.Kind);
        Assert.Equal($"Mech Pilot cannot continue: {UnsupportedReason}", intent.Reason);
    }

    [Fact]
    public void Decide_WithUnknownProfile_ReturnsManualFallback()
    {
        var snapshot = CreateSnapshot();

        var intent = MechDecisionEngine.Decide(snapshot, profile: null, MechFallbackMode.Abandon);

        Assert.Equal(MechIntentKind.ManualFallback, intent.Kind);
        Assert.Equal($"No Mech profile is recorded for mission {RecordedMissionId}", intent.Reason);
    }

    [Fact]
    public void Decide_WithMismatchedProfile_ReturnsManualFallback()
    {
        var snapshot = CreateSnapshot();
        var profile = new MechMissionProfile(RecordedMissionId + 1, RecordedMissionName, RecordedMissionNotes);

        var intent = MechDecisionEngine.Decide(snapshot, profile, MechFallbackMode.Abandon);

        Assert.Equal(MechIntentKind.ManualFallback, intent.Kind);
        Assert.Equal($"No Mech profile is recorded for mission {RecordedMissionId}", intent.Reason);
    }

    [Fact]
    public void Decide_WithKnownProfile_ReturnsWaitDuringInstrumentation()
    {
        var snapshot = CreateSnapshot();
        var profile = CreateProfile();

        var intent = MechDecisionEngine.Decide(snapshot, profile, MechFallbackMode.Abandon);

        Assert.Equal(MechIntentKind.Wait, intent.Kind);
        Assert.Equal(
            $"Mech profile is recorded for mission {RecordedMissionId}. Instrumentation is waiting for captured tactics.",
            intent.Reason);
    }

    private static MechMissionSnapshot CreateSnapshot(string? unsupportedReason = null)
    {
        return new MechMissionSnapshot(
            RecordedMissionId,
            MechWorldEvent.Unknown,
            WorldEventEndTimestamp: 0,
            CurrentScore: 0,
            PlayerPosition: Vector3.Zero,
            IsPlayerAvailable: true,
            IsPlayerBusy: false,
            IsNavmeshRunning: false,
            IsWksHudVisible: true,
            IsBoardingAddonVisible: false,
            IsRecordAddonVisible: false,
            IsMissionInfoVisible: true,
            Targets: [],
            SelectedTarget: null,
            unsupportedReason);
    }

    private static MechMissionProfile CreateProfile()
    {
        return new MechMissionProfile(
            RecordedMissionId,
            RecordedMissionName,
            RecordedMissionNotes);
    }
}
