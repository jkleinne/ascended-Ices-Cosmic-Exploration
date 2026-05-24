using System.Numerics;

namespace ICE.MechPilot;

/// <summary>
/// Captures target candidate data as values so decision code does not depend on live game objects.
/// </summary>
internal sealed record MechTargetSnapshot(
    ulong GameObjectId,
    uint DataId,
    string Name,
    Vector3 Position,
    float Distance,
    bool IsTargetable);
