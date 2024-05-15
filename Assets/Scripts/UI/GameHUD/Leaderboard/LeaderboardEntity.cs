using System;
using Unity.Collections;
using Unity.Netcode;

public struct LeaderboardEntity : INetworkSerializable, IEquatable<LeaderboardEntity>
{
    public ulong ClientId;
    public FixedString32Bytes PlayerName;
    public int Coins;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref Coins);
    }

    public bool Equals(LeaderboardEntity other)
    {
        return other.ClientId == this.ClientId
        && other.PlayerName.Equals(this.PlayerName)
        && other.Coins == this.Coins;
    }
}