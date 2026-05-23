using System;

namespace ICE.MechPilot;

/// <summary>
/// Stores the latest Mech Pilot instrumentation values for debug rendering and capture logs.
/// </summary>
internal static class MechDebugState
{
    /// <summary>
    /// Stores the most recent snapshot so UI and capture logs read one shared observation.
    /// </summary>
    public static MechMissionSnapshot? LastSnapshot { get; private set; }

    /// <summary>
    /// Stores the latest intent so capture logs can connect observed state to pilot choice.
    /// </summary>
    public static MechIntent? LastIntent { get; private set; }

    /// <summary>
    /// Stores the latest controller result so debug output can show what the pilot attempted.
    /// </summary>
    public static MechIntentResult? LastResult { get; private set; }

    /// <summary>
    /// Records when the newest debug value was captured for stale-state checks in UI.
    /// </summary>
    public static DateTime? LastCapturedAt { get; private set; }

    /// <summary>
    /// Captures a snapshot using the current local runtime timestamp for debug display.
    /// </summary>
    public static void CaptureSnapshot(MechMissionSnapshot snapshot)
    {
        CaptureSnapshot(snapshot, DateTime.Now);
    }

    /// <summary>
    /// Captures a snapshot with an injected timestamp so tests can verify ordering deterministically.
    /// </summary>
    public static void CaptureSnapshot(MechMissionSnapshot snapshot, DateTime capturedAt)
    {
        LastSnapshot = snapshot;
        LastCapturedAt = capturedAt;
    }

    /// <summary>
    /// Captures an intent using the current local runtime timestamp for debug display.
    /// </summary>
    public static void CaptureIntent(MechIntent intent)
    {
        CaptureIntent(intent, DateTime.Now);
    }

    /// <summary>
    /// Captures an intent with an injected timestamp so tests can verify ordering deterministically.
    /// </summary>
    public static void CaptureIntent(MechIntent intent, DateTime capturedAt)
    {
        LastIntent = intent;
        LastCapturedAt = capturedAt;
    }

    /// <summary>
    /// Captures a controller result using the current local runtime timestamp for debug display.
    /// </summary>
    public static void CaptureResult(MechIntentResult result)
    {
        CaptureResult(result, DateTime.Now);
    }

    /// <summary>
    /// Captures a controller result with an injected timestamp so tests can verify ordering deterministically.
    /// </summary>
    public static void CaptureResult(MechIntentResult result, DateTime capturedAt)
    {
        LastResult = result;
        LastCapturedAt = capturedAt;
    }

    /// <summary>
    /// Clears debug state so a new capture session does not inherit stale mission data.
    /// </summary>
    public static void Clear()
    {
        LastSnapshot = null;
        LastIntent = null;
        LastResult = null;
        LastCapturedAt = null;
    }
}
