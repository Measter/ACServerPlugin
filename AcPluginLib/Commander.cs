using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace AcPluginLib
{
    public class Commander
    {
        private static readonly NLog.Logger m_logger = NLog.LogManager.GetCurrentClassLogger();

        private const int CHUNK_LENGTH = 104;

        private readonly UdpClient m_server;
        private readonly ServerPoint m_config;

        internal Commander( UdpClient server, ServerPoint config )
        {
            m_server = server ?? throw new ArgumentNullException( nameof( server ) );
            m_config = config;
        }

        private static void WriteUnicodeString( BinaryWriter bw, string message )
        {
            m_logger.Debug( "Writing UTF32 string" );
            m_logger.Trace( "String contents: {0}", message );
            bw.Write( (byte)message.Length );
            bw.Write( Encoding.UTF32.GetBytes( message ) );
        }

        public void SendChat( byte carId, string message )
        {
            m_logger.Debug( "Sending chat message to car {0}", carId );
            m_logger.Trace( "Message contents: {0}", message);
            var buffer = new byte[419];
            using( var bw = new BinaryWriter( new MemoryStream( buffer ) ) )
            {
                var numChunks = (int) Math.Ceiling( (float) message.Length / CHUNK_LENGTH );

                m_logger.Trace( "Splitting message into {0} chunks", numChunks );

                for( int c = 0; c < numChunks; c++ )
                {
                    var start = c * CHUNK_LENGTH;
                    var end = Math.Min( start + CHUNK_LENGTH, message.Length );
                    var len = end - start;
                    
                    bw.Write( (byte) ACSCommand.SendChat );
                    bw.Write( carId );

                    var chunk = message.Substring( start, len );
                    m_logger.Trace( "Chunk {0} - Start: {1}, Length: {2}, Contents: {3}", c, start, len, chunk );
                    WriteUnicodeString( bw, chunk );

                    m_server.Send( buffer, (int) bw.BaseStream.Length, m_config.CommandPoint );

                    bw.BaseStream.Seek( 0, SeekOrigin.Begin );
                }
            }
        }

        public void SetRealtimeInterval( TimeSpan interval )
        {
            m_logger.Debug( "Setting realtime position interval to {0}ms", interval.TotalMilliseconds );
            var buffer = new byte[3];
            using( var bw = new BinaryWriter( new MemoryStream(buffer) ) )
            {
                bw.Write( (byte)ACSCommand.RealtimeposInterval );
                bw.Write( (UInt16)interval.TotalMilliseconds );

                m_server.Send( buffer, (int)bw.BaseStream.Length, m_config.CommandPoint );
            }
        }

        public void GetCarInfo( byte carId )
        {
            m_logger.Debug( "Car info request for car {0}", carId );
            var buffer = new byte[3];
            using( var bw = new BinaryWriter( new MemoryStream( buffer ) ) )
            {
                bw.Write( (byte)ACSCommand.GetCarInfo );
                bw.Write( carId );

                m_server.Send( buffer, (int)bw.BaseStream.Length, m_config.CommandPoint );
            }
        }

        public void GetSessionInfoCurrent()
        {
            m_logger.Debug( "Current session info request" );
            var buffer = new byte[3];
            using( var bw = new BinaryWriter( new MemoryStream( buffer ) ) )
            {
                bw.Write( (byte)ACSCommand.GetSessionInfo );
                bw.Write( (Int16)(-1) );

                m_server.Send( buffer, (int)bw.BaseStream.Length, m_config.CommandPoint );
            }
        }

        public void GetSessionInfoOther(Int16 id)
        {
            m_logger.Debug( "Session info request for session {0}", id );
            var buffer = new byte[3];
            using( var bw = new BinaryWriter( new MemoryStream( buffer ) ) )
            {
                bw.Write( (byte)ACSCommand.GetSessionInfo );
                bw.Write( id );

                m_server.Send( buffer, (int)bw.BaseStream.Length, m_config.CommandPoint );
            }
        }

        public void SetSessionInfo( byte sessionId, string name, SessionType sessionType, UInt32 lengthSec, UInt32 lengthLaps, UInt32 waitTime )
        {
            m_logger.Debug( "Setting session info for session {0}", sessionId );
            m_logger.Trace( "    Name: {0}", name );
            m_logger.Trace( "    Type: {0}", sessionType );
            m_logger.Trace( "    Length(Seconds): {0}", lengthSec );
            m_logger.Trace( "    Length(Laps): {0}", lengthLaps );
            m_logger.Trace( "    Wait Time: {0}", waitTime );

            var buffer = new byte[100];
            var trimmedName = name.Substring( 0, Math.Min( 20, name.Length ) );
            
            using( var bw = new BinaryWriter( new MemoryStream( buffer ) ) )
            {
                bw.Write( (byte)ACSCommand.SetSessionInfo );
                bw.Write( sessionId );
                WriteUnicodeString( bw, trimmedName );
                bw.Write( (byte)sessionType );

                bw.Write( lengthLaps );
                bw.Write( lengthSec );
                bw.Write( waitTime );


                m_server.Send( buffer, (int)bw.BaseStream.Length, m_config.CommandPoint );
            }
        }

        public void KickDriver( byte carId )
        {
            m_logger.Debug( "Kick driver request for car {0}", carId );
            var buffer = new byte[2];
            using( var bw = new BinaryWriter( new MemoryStream( buffer ) ) )
            {
                bw.Write( (byte)ACSCommand.KickUser );
                bw.Write( carId );

                m_server.Send( buffer, (int)bw.BaseStream.Length, m_config.CommandPoint );
            }
        }

        public void NextSession()
        {
            m_logger.Debug( "Next session request" );
            var buffer = new byte[1];
            using( var bw = new BinaryWriter( new MemoryStream( buffer ) ) )
            {
                bw.Write( (byte)ACSCommand.NextSession );

                m_server.Send( buffer, (int)bw.BaseStream.Length, m_config.CommandPoint );
            }
        }

        public void RestartSession()
        {
            m_logger.Debug( "Restart session request" );
            var buffer = new byte[1];
            using( var bw = new BinaryWriter( new MemoryStream( buffer ) ) )
            {
                bw.Write( (byte)ACSCommand.RestartSession );

                m_server.Send( buffer, (int)bw.BaseStream.Length, m_config.CommandPoint );
            }
        }

        public void SetBallast(byte carId, byte amount)
        {
            m_logger.Debug( "Set ballast to {0}kg for car {1}", amount, carId );
            var clampedAmount = Math.Min( amount, (byte)150 );
            var message = $"/ballast {carId} {clampedAmount}";
            SendAdminCommand( message );
        }

        public void SetRestrictor( byte carId, byte amount )
        {
            m_logger.Debug( "Set restrictor to {0}% for car {1}", amount, carId );
            var clampedAmount = Math.Min( amount, (byte) 100 );
            var message = $"/restrictor {carId} {clampedAmount}";
            SendAdminCommand( message );
        }

        public void BanDriver( byte carId )
        {
            m_logger.Debug( "Ban driver request for car {0}", carId );
            var message = $"/ban_id {carId}";
            SendAdminCommand( message );
        }

        public void SendAdminCommand( string message )
        {
            m_logger.Debug( "Admin command" );
            m_logger.Trace( "Command contents: {0}", message );
            var buffer = new byte[255];
            using( var bw = new BinaryWriter( new MemoryStream( buffer ) ) )
            {
                bw.Write( (byte)ACSCommand.AdminCommand );
                WriteUnicodeString( bw, message );

                m_server.Send( buffer, (int)bw.BaseStream.Length, m_config.CommandPoint );
            }
        }

        public void BroadcastMessage( string message )
        {
            m_logger.Debug( "Sending broadcast message" );
            m_logger.Trace( "Message contents: {0}", message );
            var buffer = new byte[255];
            using( var bw = new BinaryWriter( new MemoryStream( buffer ) ) )
            {
                bw.Write( (byte)ACSCommand.BroadcastChat );
                var trimmedMessage = message.Substring( 0, Math.Min( 63, message.Length ) );
                WriteUnicodeString( bw, trimmedMessage );

                m_server.Send( buffer, (int)bw.BaseStream.Length, m_config.CommandPoint );
            }
        }
    }
}