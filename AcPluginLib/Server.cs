﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using AcPluginLib.Protocol;

namespace AcPluginLib
{
    public class Server
    {
        private static readonly NLog.Logger m_logger = NLog.LogManager.GetCurrentClassLogger();

        private const uint IOC_IN = 0x80000000;
        private const uint IOC_VENDOR = 0x18000000;
        private const uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;

        private readonly Config m_config;
        private readonly List<ACEventHandler> m_handlers = new List<ACEventHandler>();

        public Server( Config config )
        {
            m_config = config;
        }

        public void AddEventHandler( ACEventHandler handler )
        {
            m_logger.Debug( $"Adding event handler: {handler}" );
            if( handler == null ) throw new ArgumentNullException( nameof( handler ) );
            m_handlers.Add( handler );
        }

        public void Run()
        {
            m_logger.Info( $"Opening UDP client at {m_config.Server.DataPort}" );
            var server = new UdpClient( m_config.Server.DataPort );

            if( m_config.SuppressSocketError )
            {
                m_logger.Debug( "Suppressing socket error" );
                SuppressSocketError( server );
            }

            if( m_config.Forward.HasValue )
            {
                m_logger.Info( $"Enabling forwarding from {m_config.Forward.Value.CommandPoint}" );
                var commandThread = new Thread( () => CommandForwardTask( server ) );
                commandThread.Start();
            }

            var recievePoint = m_config.Server.CommandPoint;
            m_logger.Debug( "Creating Commander" );
            var commander = new Commander( server, m_config.Server );

            m_logger.Info( "Waiting for messages from server" );

            while( true )
            {
                var bytes = server.Receive( ref recievePoint );
                m_logger.Debug( "Recieved data packet." );

                if( m_config.Forward.HasValue )
                {
                    m_logger.Debug( "Forwarding data packet" );
                    server.Send( bytes, bytes.Length, m_config.Forward.Value.DataPort );
                }
                
                var br = new BinaryReader( new MemoryStream( bytes ) );
                var packetType = (ACSMessage) br.ReadByte();
                m_logger.Debug( $"Packet type: {packetType}" );

                switch( packetType )
                {
                    case ACSMessage.NewSession:
                        var nsInfo = SessionInfo.Parse( br );
                        foreach( var handler in m_handlers )
                            handler.OnNewSession( commander, nsInfo );
                        break;
                    case ACSMessage.NewConnection:
                        var ncInfo = ConnectionInfo.Parse( br );
                        foreach( var handler in m_handlers )
                            handler.OnNewConnection( commander, ncInfo );
                        break;
                    case ACSMessage.ConnectionClosed:
                        var ccInfo = ConnectionInfo.Parse( br );
                        foreach( var handler in m_handlers )
                            handler.OnConnectionClosed( commander, ccInfo );
                        break;
                    case ACSMessage.CarUpdate:
                        var cuInfo = CarUpdateInfo.Parse( br );
                        foreach( var handler in m_handlers )
                            handler.OnCarUpdate( commander, cuInfo );
                        break;
                    case ACSMessage.CarInfo:
                        var ciInfo = CarInfo.Parse( br );
                        foreach( var handler in m_handlers )
                            handler.OnCarInfo( commander, ciInfo );
                        break;
                    case ACSMessage.EndSession:
                        var esInfo = Parsing.ReadUnicodeString( br );
                        foreach( var handler in m_handlers )
                            handler.OnEndSession( commander, esInfo );
                        break;
                    case ACSMessage.LapCompleted:
                        var lcInfo = LapCompletedInfo.Parse( br );
                        foreach( var handler in m_handlers )
                            handler.OnLapCompleted( commander, lcInfo );
                        break;
                    case ACSMessage.Version:
                        var version = br.ReadByte();
                        foreach( var handler in m_handlers )
                            handler.OnProtocolVersion( commander, version );
                        break;
                    case ACSMessage.Chat:
                        var chat = ChatMessage.Parse( br );
                        foreach( var handler in m_handlers )
                            handler.OnChatMessage( commander, chat );
                        break;
                    case ACSMessage.ClientLoaded:
                        var clId = br.ReadByte();
                        foreach( var handler in m_handlers )
                            handler.OnClientLoaded( commander, clId );
                        break;
                    case ACSMessage.SessionInfo:
                        var siInfo = SessionInfo.Parse( br );
                        foreach( var handler in m_handlers )
                            handler.OnSessionInfo( commander, siInfo );
                        break;
                    case ACSMessage.Error:
                        var err = Parsing.ReadUnicodeString( br );
                        m_logger.Info( $"Error recieved from server: {err}" );
                        foreach( var handler in m_handlers )
                            handler.OnError( commander, err );
                        break;
                    case ACSMessage.ClientEvent:
                        var ceInfo = ClientEventInfo.Parse( br );
                        foreach( var handler in m_handlers )
                            handler.OnClientEvent( commander, ceInfo );
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void CommandForwardTask( UdpClient server )
        {
            if( m_config.Forward != null )
            {
                m_logger.Debug( $"Opening forawding client to {m_config.Forward.Value.CommandPoint}" );
                var forwardClient = new UdpClient(m_config.Forward.Value.CommandPoint);

                if( m_config.SuppressSocketError )
                {
                    m_logger.Debug( "Suppressing socket error for forwarding client" );
                    SuppressSocketError( forwardClient );
                }

                var recievePoint = m_config.Forward.Value.CommandPoint;

                while( true )
                {
                    m_logger.Debug( "Awaiting message to forward." );
                    var bytes = forwardClient.Receive( ref recievePoint );
                    server.Send( bytes, bytes.Length, m_config.Server.CommandPoint );
                    m_logger.Debug( "Message forwarded" );
                }
            }
            else
            {
                m_logger.Debug( "Forwarding task function called with a null ServerPoint" );
            }
        }

        /// <summary>
        /// This is used to prevent Windows closing the connection if there's no response.
        /// </summary>
        /// <param name="client"></param>
        private static void SuppressSocketError( UdpClient client )
        {
            unchecked
            {
                client.Client.IOControl( (int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte( false ) }, null );
            }
        }
    }
}
