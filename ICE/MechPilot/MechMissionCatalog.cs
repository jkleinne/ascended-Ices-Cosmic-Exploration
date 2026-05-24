namespace ICE.MechPilot;

/// <summary>
/// Looks up captured Mech mission profiles while intentionally refusing every mission until profile data exists.
/// </summary>
internal static class MechMissionCatalog
{
    /// <summary>
    /// Returns a captured profile for the mission, or null so Mech tactics stay blocked until recordings are added.
    /// </summary>
    public static MechMissionProfile? FindProfile(uint missionId)
    {
        return null;
    }
}
