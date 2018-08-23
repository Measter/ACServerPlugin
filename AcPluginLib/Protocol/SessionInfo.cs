using System;
using System.IO;

namespace AcPluginLib.Protocol
{
    public class SessionInfo
    {
        public byte Version { get; }
        public byte MessageSessionIndex { get; }
        public byte ServerSessionIndex { get; }
        public byte SessionCount { get; }
        public string ServerName { get; }
        public string TrackName { get; }
        public string TrackLayout { get; }
        public string SessionName { get; }
        public SessionType SessionType { get; }
        public UInt16 SessionLengthLaps { get; }
        public TimeSpan SessionLengthMinutes { get; }
        public TimeSpan WaitTime { get; }
        public byte AmbientTemperature { get; }
        public byte RoadTemperature { get; }
        public string Weather { get; }
        public TimeSpan Elapsed { get; }

        internal SessionInfo( byte version, byte messageSessionIndex, byte serverSessionIndex, byte sessionCount,
            string serverName, string trackName, string trackLayout, string sessionName, SessionType sessionType,
            ushort sessionLengthLaps, TimeSpan sessionLengthMinutes, TimeSpan waitTime, byte ambientTemperature,
            byte roadTemperature, string weather, TimeSpan elapsed )
        {
            Version = version;
            MessageSessionIndex = messageSessionIndex;
            ServerSessionIndex = serverSessionIndex;
            SessionCount = sessionCount;
            ServerName = serverName ?? throw new ArgumentNullException( nameof( serverName ) );
            TrackName = trackName ?? throw new ArgumentNullException( nameof( trackName ) );
            TrackLayout = trackLayout ?? throw new ArgumentNullException( nameof( trackLayout ) );
            SessionName = sessionName ?? throw new ArgumentNullException( nameof( sessionName ) );
            SessionType = sessionType;
            SessionLengthLaps = sessionLengthLaps;
            SessionLengthMinutes = sessionLengthMinutes;
            WaitTime = waitTime;
            AmbientTemperature = ambientTemperature;
            RoadTemperature = roadTemperature;
            Weather = weather ?? throw new ArgumentNullException( nameof( weather ) );
            Elapsed = elapsed;
        }

        internal static SessionInfo Parse( BinaryReader br )
        {
            var version = br.ReadByte();
            var messageSessionIndex = br.ReadByte();
            var serverSessionIndex = br.ReadByte();
            var sessionCount = br.ReadByte();
            var serverName = Parsing.ReadUnicodeString( br );
            var trackName = Parsing.ReadAsciiString( br );
            var trackLayout = Parsing.ReadAsciiString( br );
            var sessionName = Parsing.ReadAsciiString( br );

            var sessionTypeId = br.ReadByte();
            SessionType sessionType;
            switch( sessionTypeId )
            {
                case 1:
                    sessionType = SessionType.Practice;
                    break;
                case 2:
                    sessionType = SessionType.Qualifying;
                    break;
                case 3:
                    sessionType = SessionType.Race;
                    break;
                default:
                    sessionType = SessionType.None;
                    break;
            }

            var sessionLengthTime = TimeSpan.FromMinutes( br.ReadUInt16() );
            var sessionLengthLaps = br.ReadUInt16();
            var waitTime = TimeSpan.FromMinutes( br.ReadUInt16() );
            var ambientTemp = br.ReadByte();
            var roadTemp = br.ReadByte();
            var weather = Parsing.ReadAsciiString( br );
            var elaspedMs = TimeSpan.FromMilliseconds( br.ReadInt32() );

            return new SessionInfo(
                version, messageSessionIndex, serverSessionIndex, sessionCount,
                serverName, trackName, trackLayout, sessionName, sessionType,
                sessionLengthLaps, sessionLengthTime, waitTime, ambientTemp,
                roadTemp, weather, elaspedMs
            );
        }

        public override string ToString()
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendFormat( "{0} {{", nameof( SessionInfo ) ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( Version ), Version.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( MessageSessionIndex ), MessageSessionIndex.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( ServerSessionIndex ), ServerSessionIndex.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( SessionCount ), SessionCount.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( ServerName ), ServerName.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( TrackName ), TrackName.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( TrackLayout ), TrackLayout.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( SessionName ), SessionName.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( SessionType ), SessionType.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( SessionLengthLaps ), SessionLengthLaps.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( SessionLengthMinutes ), SessionLengthMinutes.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( WaitTime ), WaitTime.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( AmbientTemperature ), AmbientTemperature.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( RoadTemperature ), RoadTemperature.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( Weather ), Weather.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( Elapsed ), Elapsed.ToString() ).AppendLine();
            builder.AppendFormat( "}}" ).AppendLine();
            return builder.ToString();
        }
    }
}