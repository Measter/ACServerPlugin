using AcPluginLib.Protocol;

namespace AcPluginLib
{
    public class ACEventHandler
    {
        public virtual void OnProtocolVersion( Commander cmdr, byte version )
        {
        }

        public virtual void OnSessionInfo( Commander cmdr, SessionInfo info )
        {
        }

        public virtual void OnNewSession( Commander cmdr, SessionInfo info )
        {
        }

        public virtual void OnError( Commander cmdr, string error )
        {
        }

        public virtual void OnClientLoaded( Commander cmdr, byte carId )
        {
        }

        public virtual void OnChatMessage( Commander cmdr, ChatMessage info )
        {
        }

        public virtual void OnEndSession( Commander cmdr, string path )
        {
        }

        public virtual void OnClientEvent( Commander cmdr, ClientEventInfo info )
        {
        }

        public virtual void OnCarInfo( Commander cmdr, CarInfo info )
        {
        }

        public virtual void OnCarUpdate( Commander cmdr, CarUpdateInfo info )
        {
        }

        public virtual void OnNewConnection( Commander cmdr, ConnectionInfo info )
        {
        }

        public virtual void OnConnectionClosed( Commander cmdr, ConnectionInfo info )
        {
        }

        public virtual void OnLapCompleted( Commander cmdr, LapCompletedInfo info )
        {
        }
    }
}