using System;
using System.Numerics;
using ICE.MechPilot;
using Xunit;

namespace ICE.Tests.MechPilot;

public sealed class MechSnapshotFormatterTests
{
    private const uint RecordedMissionId = 882;
    private const uint RecordedWorldEventEndTimestamp = 123456;
    private const uint RecordedScore = 120;
    private const ulong RecordedGameObjectId = 10;
    private const uint RecordedDataId = 20;
    private const string RecordedTargetName = "Target";
    private const float RecordedTargetDistance = 7.5f;
    private const string UnsupportedReason = "No profile";

    [Fact]
    public void FormatSnapshot_IncludesCapturedMissionAndTargetState()
    {
        var snapshot = CreateSnapshot();

        var formattedSnapshot = MechSnapshotFormatter.FormatSnapshot(snapshot);

        Assert.Contains("Mission: 882", formattedSnapshot);
        Assert.Contains("World Event: MechOpsDeploying", formattedSnapshot);
        Assert.Contains("Event End Timestamp: 123456", formattedSnapshot);
        Assert.Contains("Score: 120", formattedSnapshot);
        Assert.Contains("WKS HUD: True", formattedSnapshot);
        Assert.Contains("Boarding Addon: False", formattedSnapshot);
        Assert.Contains("Record Addon: True", formattedSnapshot);
        Assert.Contains("Mission Info Addon: True", formattedSnapshot);
        Assert.Contains("Targets: 1", formattedSnapshot);
        Assert.Contains("Selected Target: 20 Target", formattedSnapshot);
        Assert.Contains("Target 1: 20 Target", formattedSnapshot);
        Assert.Contains("Unsupported: No profile", formattedSnapshot);
    }

    [Fact]
    public void FormatIntent_WithIntent_IncludesKindAndReason()
    {
        var intent = MechIntent.ManualFallback(UnsupportedReason);

        var formattedIntent = MechSnapshotFormatter.FormatIntent(intent);

        Assert.Contains("Intent: ManualFallback", formattedIntent);
        Assert.Contains("Reason: No profile", formattedIntent);
    }

    [Fact]
    public void FormatResult_WithResult_IncludesCompletionAndMessage()
    {
        var result = MechIntentResult.Complete("Done");

        var formattedResult = MechSnapshotFormatter.FormatResult(result);

        Assert.Contains("Result Complete: True", formattedResult);
        Assert.Contains("Message: Done", formattedResult);
    }

    [Fact]
    public void MechDebugState_CapturesLatestValuesAndClearsThem()
    {
        var snapshot = CreateSnapshot();
        var intent = MechIntent.ManualFallback(UnsupportedReason);
        var result = MechIntentResult.Complete("Done");
        var snapshotCapturedAt = new DateTime(2026, 5, 23, 10, 30, 0, DateTimeKind.Local);
        var intentCapturedAt = new DateTime(2026, 5, 23, 10, 31, 0, DateTimeKind.Local);
        var resultCapturedAt = new DateTime(2026, 5, 23, 10, 32, 0, DateTimeKind.Local);

        MechDebugState.Clear();

        Assert.Null(MechDebugState.LastSnapshot);
        Assert.Null(MechDebugState.LastIntent);
        Assert.Null(MechDebugState.LastResult);
        Assert.Null(MechDebugState.LastCapturedAt);

        MechDebugState.CaptureSnapshot(snapshot, snapshotCapturedAt);
        MechDebugState.CaptureIntent(intent, intentCapturedAt);
        MechDebugState.CaptureResult(result, resultCapturedAt);

        Assert.Same(snapshot, MechDebugState.LastSnapshot);
        Assert.Same(intent, MechDebugState.LastIntent);
        Assert.Same(result, MechDebugState.LastResult);
        Assert.Equal(resultCapturedAt, MechDebugState.LastCapturedAt);

        MechDebugState.Clear();

        Assert.Null(MechDebugState.LastSnapshot);
        Assert.Null(MechDebugState.LastIntent);
        Assert.Null(MechDebugState.LastResult);
        Assert.Null(MechDebugState.LastCapturedAt);
    }

    private static MechMissionSnapshot CreateSnapshot()
    {
        var target = new MechTargetSnapshot(
            RecordedGameObjectId,
            RecordedDataId,
            RecordedTargetName,
            new Vector3(4.0f, 5.0f, 6.0f),
            RecordedTargetDistance,
            IsTargetable: true);

        return new MechMissionSnapshot(
            RecordedMissionId,
            MechWorldEvent.MechOpsDeploying,
            RecordedWorldEventEndTimestamp,
            RecordedScore,
            new Vector3(1.0f, 2.0f, 3.0f),
            IsPlayerAvailable: true,
            IsPlayerBusy: false,
            IsNavmeshRunning: false,
            IsWksHudVisible: true,
            IsBoardingAddonVisible: false,
            IsRecordAddonVisible: true,
            IsMissionInfoVisible: true,
            Targets: [target],
            SelectedTarget: target,
            UnsupportedReason);
    }
}
