using System;
using System.IO;

namespace AcPluginLib.Protocol
{
    public struct LeaderboardEntry
    {
        public byte CarId { get; }
        public TimeSpan LapTime { get; }
        public UInt16 Laps { get; }
        public bool HasFinished { get; }

        internal LeaderboardEntry( byte carId, TimeSpan lapTime, ushort laps, bool hasFinished ) : this()
        {
            CarId = carId;
            LapTime = lapTime;
            Laps = laps;
            HasFinished = hasFinished;
        }

        internal static LeaderboardEntry Parse( BinaryReader br )
        {
            return new LeaderboardEntry(
                br.ReadByte(),
                TimeSpan.FromMilliseconds( br.ReadUInt32() ), 
                br.ReadUInt16(),
                br.ReadBoolean()
            );
        }
    }
}