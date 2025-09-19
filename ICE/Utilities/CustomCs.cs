using System.Runtime.InteropServices;

namespace ICE.Utilities;

[StructLayout(LayoutKind.Explicit)]
unsafe struct WKSManagerCustom
{
    [FieldOffset(0xC55)] public fixed byte MissionCompletionFlags[136];
    [FieldOffset(0xCDD)] public fixed byte MissionGoldFlags[136];

    public bool IsMissionCompleted(uint missionUnitId)
    {
        var group = (byte)(missionUnitId >> 3);
        var mask = 1 << ((int)missionUnitId & 7);
        return (mask & MissionCompletionFlags[group]) != 0;
    }

    public bool IsMissionGolded(uint missionUnitId)
    {
        var group = (byte)(missionUnitId >> 3);
        var mask = 1 << ((int)missionUnitId & 7);
        return (mask & MissionGoldFlags[group]) != 0;
    }
}
