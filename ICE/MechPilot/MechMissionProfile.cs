namespace ICE.MechPilot;

/// <summary>
/// Represents one recorded Mech Pilot mission profile that can later grow objective and tactic data.
/// </summary>
internal sealed record MechMissionProfile(
    uint MissionId,
    string Name,
    string Notes);
