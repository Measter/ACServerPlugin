using System;
using System.Net;

namespace AcPluginLib
{
    public struct ServerPoint
    {
        public IPEndPoint CommandPoint;
        public IPEndPoint DataPort;

        public ServerPoint( IPEndPoint commandPoint, IPEndPoint dataPort )
        {
            CommandPoint = commandPoint ?? throw new ArgumentNullException( nameof( commandPoint ) );
            DataPort = dataPort ?? throw new ArgumentNullException( nameof( dataPort ) );
        }
    }

    public struct Config
    {
        public ServerPoint Server;
        public ServerPoint? Forward;
        public bool SuppressSocketError;

        public Config( ServerPoint server, ServerPoint? forward, bool suppressSocketError )
        {
            Server = server;
            Forward = forward;
            SuppressSocketError = suppressSocketError;
        }
    }
}