﻿using System;
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

        public override string ToString()
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendFormat( "{0} {{", nameof( LapCompletedInfo ) ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( CarId ), CarId.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( LapTime ), LapTime.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( Cuts ), Cuts.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( GripLevel ), GripLevel.ToString() ).AppendLine();
            builder.AppendFormat( "    Leaderboard: " );
            for( var i = 0; i < Leaderboard.Count; i++ )
            {
                LeaderboardEntry entry = Leaderboard[i];
                builder.AppendFormat( "    - {0}: {1}", i, entry).AppendLine();
            }

            builder.AppendFormat( "}}" ).AppendLine();
            return builder.ToString();
        }
    }
}