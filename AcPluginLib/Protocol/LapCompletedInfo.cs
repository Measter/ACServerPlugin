using System;
using System.Collections.Generic;
using System.IO;

namespace AcPluginLib.Protocol
{
    public class LapCompletedInfo
    {
        public byte CarId { get; }
        public TimeSpan LapTime { get; }
        public byte Cuts { get; }
        public List<LeaderboardEntry> Leaderboard { get; }
        public float GripLevel { get; }

        internal LapCompletedInfo( byte carId, TimeSpan lapTime, byte cuts, List<LeaderboardEntry> leaderboard, float gripLevel )
        {
            CarId = carId;
            LapTime = lapTime;
            Cuts = cuts;
            Leaderboard = leaderboard ?? throw new ArgumentNullException( nameof( leaderboard ) );
            GripLevel = gripLevel;
        }

        internal static LapCompletedInfo Parse( BinaryReader br )
        {
            var id = br.ReadByte();
            var laptime = TimeSpan.FromMilliseconds( br.ReadUInt32() );
            var cuts = br.ReadByte();

            var boardLen = br.ReadByte();
            var board = new List<LeaderboardEntry>();

            for( int i = 0; i < boardLen; i++ )
                board.Add( LeaderboardEntry.Parse( br ) );

            var grip = br.ReadSingle();

            return new LapCompletedInfo( id, laptime, cuts, board, grip );
        }
    }
}