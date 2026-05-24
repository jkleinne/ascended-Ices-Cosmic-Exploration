using System;
using System.Globalization;
using System.Text;

namespace ICE.MechPilot;

/// <summary>
/// Formats Mech Pilot values for debug display and log capture.
/// </summary>
internal static class MechSnapshotFormatter
{
    /// <summary>
    /// Creates deterministic snapshot text so capture logs and debug UI show the same state.
    /// </summary>
    public static string FormatSnapshot(MechMissionSnapshot snapshot)
    {
        var builder = new StringBuilder();
        builder.AppendLine(CultureInfo.InvariantCulture, $"Mission: {snapshot.CurrentMissionId}");
        builder.AppendLine(CultureInfo.InvariantCulture, $"World Event: {snapshot.WorldEvent}");
        builder.AppendLine(CultureInfo.InvariantCulture, $"Event End Timestamp: {snapshot.WorldEventEndTimestamp}");
        builder.AppendLine(CultureInfo.InvariantCulture, $"Score: {snapshot.CurrentScore}");
        builder.AppendLine(CultureInfo.InvariantCulture, $"Player: {snapshot.PlayerPosition.X:N2}, {snapshot.PlayerPosition.Y:N2}, {snapshot.PlayerPosition.Z:N2}");
        builder.AppendLine(CultureInfo.InvariantCulture, $"Available: {snapshot.IsPlayerAvailable}");
        builder.AppendLine(CultureInfo.InvariantCulture, $"Busy: {snapshot.IsPlayerBusy}");
        builder.AppendLine(CultureInfo.InvariantCulture, $"Navmesh Running: {snapshot.IsNavmeshRunning}");
        builder.AppendLine(CultureInfo.InvariantCulture, $"WKS HUD: {snapshot.IsWksHudVisible}");
        builder.AppendLine(CultureInfo.InvariantCulture, $"Boarding Addon: {snapshot.IsBoardingAddonVisible}");
        builder.AppendLine(CultureInfo.InvariantCulture, $"Record Addon: {snapshot.IsRecordAddonVisible}");
        builder.AppendLine(CultureInfo.InvariantCulture, $"Mission Info Addon: {snapshot.IsMissionInfoVisible}");
        builder.AppendLine(CultureInfo.InvariantCulture, $"Targets: {snapshot.Targets.Count}");
        builder.AppendLine(CultureInfo.InvariantCulture, $"Selected Target: {FormatTarget(snapshot.SelectedTarget)}");

        for (var index = 0; index < snapshot.Targets.Count; index++)
        {
            builder.AppendLine(CultureInfo.InvariantCulture, $"Target {index + 1}: {FormatTarget(snapshot.Targets[index])}");
        }

        builder.AppendLine(CultureInfo.InvariantCulture, $"Unsupported: {snapshot.UnsupportedReason ?? "None"}");

        return builder.ToString();
    }

    /// <summary>
    /// Creates deterministic intent text so debug output can explain the chosen pilot action.
    /// </summary>
    public static string FormatIntent(MechIntent? intent)
    {
        if (intent is null)
        {
            return "Intent: None";
        }

        return string.Create(
            CultureInfo.InvariantCulture,
            $"Intent: {intent.Kind}{Environment.NewLine}Reason: {intent.Reason}");
    }

    /// <summary>
    /// Creates deterministic result text so debug output can show the latest controller outcome.
    /// </summary>
    public static string FormatResult(MechIntentResult? result)
    {
        if (result is null)
        {
            return "Result: None";
        }

        return string.Create(
            CultureInfo.InvariantCulture,
            $"Result Complete: {result.IsComplete}{Environment.NewLine}Message: {result.Message}");
    }

    private static string FormatTarget(MechTargetSnapshot? target)
    {
        if (target is null)
        {
            return "None";
        }

        return string.Create(
            CultureInfo.InvariantCulture,
            $"{target.DataId} {target.Name} distance {target.Distance:N2} targetable {target.IsTargetable}");
    }
}
