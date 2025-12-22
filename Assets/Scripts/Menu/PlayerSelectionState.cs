using Unity.Netcode;
using System;

public struct PlayerSelectionState :
    INetworkSerializable,
    IEquatable<PlayerSelectionState>
{
    public ulong ClientId;
    public int CharacterId;

    public bool Equals(PlayerSelectionState other)
    {
        return ClientId == other.ClientId &&
               CharacterId == other.CharacterId;
    }

    public override bool Equals(object obj)
    {
        return obj is PlayerSelectionState other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ClientId, CharacterId);
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer)
        where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref CharacterId);
    }
}
