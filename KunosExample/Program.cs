using System;
using System.Net;
using System.Text;
using AcPluginLib;
using AcPluginLib.Protocol;

namespace KunosExample
{
    class Program
    {
        static void Main( string[] args )
        {
            var ip = IPAddress.Parse( "127.0.0.1" );
            var cfg = new Config(
                new ServerPoint(
                    new IPEndPoint( ip, 11001 ),
                    new IPEndPoint( ip, 12001 )
                ), 
                null,
                true
            );

            var server = new Server( cfg );
            var driverDB = server.GetDriverHandler();
            server.AddEventHandler( new PrintHandler() );
            server.AddEventHandler( new ResponseHandler() );

            server.Run();
        }
    }

    internal class ResponseHandler : ACEventHandler
    {
        public override void OnCarInfo( Commander cmdr, CarInfo info )
        {
            cmdr.SetSessionInfo( 1, "SuperCoolServer", SessionType.Race, 0, 250, 60 );
        }

        public override void OnChatMessage( Commander cmdr, ChatMessage info )
        {
            switch( info.Message )
            {
                case "ballast me": 
                    cmdr.SetBallast( info.CarId, 10 );
                    break;
                case "restrict me":
                    cmdr.SetRestrictor( info.CarId, 10 );
                    break;
                case "kick me":
                    cmdr.KickDriver( info.CarId );
                    break;
                case "ban me":
                    cmdr.BanDriver( info.CarId );
                    break;
                default:
                    break;
            }
        }

        public override void OnClientLoaded( Commander cmdr, byte carId )
        {
            cmdr.SendChat( carId, "Welcome to the Simple Plugin Example." );

            var message = new StringBuilder( "Test of long message: " );
            for( int i = 0; i < 30; i++ )
                message.Append( "Yay!" );

            cmdr.SendChat( carId, message.ToString() );
        }

        public override void OnNewConnection( Commander cmdr, ConnectionInfo info )
        {
            cmdr.GetCarInfo( info.CarId );
        }

        public override void OnNewSession( Commander cmdr, SessionInfo info )
        {
            cmdr.SetRealtimeInterval( TimeSpan.FromMilliseconds( 1000 ) );

            cmdr.GetCarInfo( 1 );

            cmdr.SendChat( 0, "CIOA BELLO!" );
            cmdr.BroadcastMessage( "E' arrivat' 'o 'pirite'" );

            // Kick user with bad index. Will trigger server error.
            cmdr.KickDriver( 230 );
        }
    }

    internal class PrintHandler : ACEventHandler
    {
        private void PrintSessionInfo( SessionInfo info )
        {
            Console.WriteLine( "--- Session Info ---" );
            Console.WriteLine( $"Protocol Version: {info.Version}" );
            Console.WriteLine( $"Session Index: {info.MessageSessionIndex}/{info.SessionCount} Current Session: {info.ServerSessionIndex}" );
            Console.WriteLine( $"Server Name: {info.ServerName}" );
            Console.WriteLine( $"Track: {info.TrackName} [{info.TrackLayout}]" );
            Console.WriteLine( $"Session Name: {info.SessionName}" );
            Console.WriteLine( $"Session Type: {info.SessionType}" );
            Console.WriteLine( $"Session Length: {info.SessionLengthLaps} Laps/{info.SessionLengthMinutes.TotalMinutes} Minutes" );
            Console.WriteLine( $"Wait Time: {info.WaitTime.TotalMinutes}" );
            Console.WriteLine( $"Weather: {info.Weather}" );
            Console.WriteLine( $"Elapsed: {info.Elapsed}" );
        }

        private void PrintConnectionInfo( ConnectionInfo info )
        {
            Console.WriteLine( $"Driver: {info.DriverName} - GUID: {info.DriverGuid}" );
            Console.WriteLine( $"Car: {info.CarModel} - Model: {info.CarModel} - Skin: {info.CarSkin}" );
        }    

        public override void OnNewSession( Commander cmdr, SessionInfo info )
        {
            Console.WriteLine( "--- New Session ---" );
            PrintSessionInfo( info );
        }

        public override void OnProtocolVersion( Commander cmdr, byte version )
        {
            Console.WriteLine( $"Protocol Version: {version}" );
        }

        public override void OnSessionInfo( Commander cmdr, SessionInfo info )
        {
            Console.WriteLine( "--- Session Info ---" );
            PrintSessionInfo( info );
        }

        public override void OnError( Commander cmdr, string error )
        {
            Console.WriteLine( $"Server Error: {error}" );
        }

        public override void OnClientLoaded( Commander cmdr, byte carId )
        {
            Console.WriteLine( $"Client Loaded: {carId}" );
        }

        public override void OnChatMessage( Commander cmdr, ChatMessage info )
        {
            Console.WriteLine( $"Chat from car {info.CarId}: {info.Message}" );
        }

        public override void OnEndSession( Commander cmdr, string path )
        {
            Console.WriteLine( $"--- End of Session ---" );
            Console.WriteLine( $"Report JSON available at {path}" );
        }

        public override void OnClientEvent( Commander cmdr, ClientEventInfo info )
        {
            switch( info.CollisionType )
            {
                case CollisionType.CollisionWithEnv:
                    Console.WriteLine( $"Collision with Env, Car: {info.CarId} - Impact Speed: {info.Speed} - World Pos.: {info.WorldPosition} - Rel. Pos.: {info.RelativePosition}" );
                    break;
                case CollisionType.CollisionWithCar:
                    Console.WriteLine( $"Collision with Car, Car: {info.CarId} - Other Car: {info.OtherCarId.Value} - Impact Speed: {info.Speed} - World Pos.: {info.WorldPosition} - Rel. Pos.: {info.RelativePosition}" );
                    break;
            }
        }

        public override void OnCarInfo( Commander cmdr, CarInfo info )
        {
            Console.WriteLine( $"--- Car Info ---" );
            Console.WriteLine( $"Car: {info.CarId} {info.CarModel} [{info.CarSkin}] - Driver: {info.DriverName} - Team: {info.DriverTeam} - GUID: {info.DriverGuid} - Connected: {info.IsConnected}" );
        }

        public override void OnCarUpdate( Commander cmdr, CarUpdateInfo info )
        {
            Console.WriteLine( $"--- Car Update ---" );
            Console.WriteLine( $"Car: {info.CarId} - Pos.: {info.Position}, - Vel.: {info.Velocity} - Gear: {info.Gear} - RPM: {info.EngineRpm} - NSP: {info.SplinePos}" );
        }

        public override void OnNewConnection( Commander cmdr, ConnectionInfo info )
        {
            Console.WriteLine( $"--- New Connection ---" );
            PrintConnectionInfo( info );
        }

        public override void OnConnectionClosed( Commander cmdr, ConnectionInfo info )
        {
            Console.WriteLine( $"--- Connection Closed ---" );
            PrintConnectionInfo( info );
        }

        public override void OnLapCompleted( Commander cmdr, LapCompletedInfo info )
        {
            Console.WriteLine( $"s--- Lap Completed ---" );
            Console.WriteLine( $"Car: {info.CarId} - Lap: {info.LapTime} - Cuts: {info.Cuts}" );

            for( int i = 0; i < info.Leaderboard.Count; i++ )
            {
                var entry = info.Leaderboard[i];
                Console.WriteLine( $"{i+1}: Car: {entry.CarId} - Time: {entry.LapTime} - Has Finished: {entry.HasFinished}" );
            }

            Console.WriteLine( $"Grip Level: {info.GripLevel}" );
        }
    }
}
