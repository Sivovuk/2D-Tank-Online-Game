using System;
using Unity.Collections;
using Unity.Netcode;

namespace UI.Leaderboard
{
    public struct LeaderboardEntityState : INetworkSerializable, IEquatable<LeaderboardEntityState>
    {
        public ulong ClientID;
        public int TeamIndex;
        public FixedString32Bytes PlayerName;
        public int Coins;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientID);
            serializer.SerializeValue(ref TeamIndex);
            serializer.SerializeValue(ref PlayerName);
            serializer.SerializeValue(ref Coins);
        }

        public bool Equals(LeaderboardEntityState other)
        {
            return  ClientID == other.ClientID && 
                    TeamIndex == other.TeamIndex &&
                    PlayerName.Equals(other.PlayerName) && 
                    Coins == other.Coins;
        }
    }
}