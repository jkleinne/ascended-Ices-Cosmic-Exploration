using Dalamud.Interface.Utility.Raii;
using System.Globalization;
using ICE.MechPilot;

namespace ICE.Ui.DebugWindowTabs;

internal static class Ui_MechPilotDebug
{
    private const uint MaxTextBlockLength = 32768;
    private const float TextBlockHeight = 160.0f;
    private const string LogTag = "[Mech Pilot Capture]";

    public static void Draw()
    {
        DrawConfigControls();
        DrawCaptureControls();

        ImGui.Separator();
        ImGui.Text($"Scheduler State: {SchedulerMain.State}");
        ImGui.Text($"Automation Enabled: {C.MechPilotAutomationEnabled}");
        ImGui.Text($"Fallback Mode: {C.MechPilotFallbackMode}");
        ImGui.Text($"Last Captured: {FormatLastCapturedAt()}");

        DrawTextBlock("Snapshot", FormatSnapshot());
        DrawTextBlock("Intent", MechSnapshotFormatter.FormatIntent(MechDebugState.LastIntent));
        DrawTextBlock("Result", MechSnapshotFormatter.FormatResult(MechDebugState.LastResult));
    }

    private static void DrawConfigControls()
    {
        var isAutomationEnabled = C.MechPilotAutomationEnabled;
        if (ImGui.Checkbox("Enable Mech Pilot Automation", ref isAutomationEnabled))
        {
            C.MechPilotAutomationEnabled = isAutomationEnabled;
            C.Save();
        }

        DrawFallbackModeOption("Manual", MechFallbackMode.Manual);
        ImGui.SameLine();
        DrawFallbackModeOption("Abandon", MechFallbackMode.Abandon);
    }

    private static void DrawFallbackModeOption(string label, MechFallbackMode fallbackMode)
    {
        if (ImGui.RadioButton(label, C.MechPilotFallbackMode == fallbackMode))
            SetFallbackMode(fallbackMode);
    }

    private static void SetFallbackMode(MechFallbackMode fallbackMode)
    {
        C.MechPilotFallbackMode = fallbackMode;
        C.Save();
    }

    private static void DrawCaptureControls()
    {
        if (ImGui.Button("Capture Snapshot"))
        {
            var snapshot = MechMissionReader.ReadSnapshot();
            MechDebugState.CaptureSnapshot(snapshot);
        }

        ImGui.SameLine();
        if (ImGui.Button("Enter Mech State"))
        {
            P.TaskManager.Tasks.Clear();
            P.TaskManager.Abort();
            SchedulerMain.State = IceState.MechPilot;
        }

        ImGui.SameLine();
        if (ImGui.Button("Clear Capture"))
        {
            MechDebugState.Clear();
        }

        ImGui.SameLine();
        using (ImRaii.Disabled(MechDebugState.LastSnapshot is null))
        {
            if (ImGui.Button("Log Snapshot"))
                LogSnapshot();
        }
    }

    private static void LogSnapshot()
    {
        if (MechDebugState.LastSnapshot is null)
            return;

        var snapshotText = MechSnapshotFormatter.FormatSnapshot(MechDebugState.LastSnapshot);
        IceLogging.Info(snapshotText, LogTag);
    }

    private static string FormatLastCapturedAt()
    {
        return MechDebugState.LastCapturedAt?.ToString("O", CultureInfo.InvariantCulture) ?? "None";
    }

    private static string FormatSnapshot()
    {
        return MechDebugState.LastSnapshot is null
            ? "Snapshot: None"
            : MechSnapshotFormatter.FormatSnapshot(MechDebugState.LastSnapshot);
    }

    private static void DrawTextBlock(string label, string text)
    {
        ImGui.Text(label);
        var displayText = text;
        ImGui.InputTextMultiline(
            $"##{label}",
            ref displayText,
            MaxTextBlockLength,
            new Vector2(ImGui.GetContentRegionAvail().X, TextBlockHeight),
            ImGuiInputTextFlags.ReadOnly);
    }
}
