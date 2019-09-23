using System;
using System.IO;
using System.Text;

namespace AcPluginLib
{
    public enum ACSMessage : byte
    {
        NewSession = 50,
        NewConnection = 51,
        ConnectionClosed = 52,
        CarUpdate = 53,
        CarInfo = 54, // Sent as response to ACSP_GET_CAR_INFO command
        EndSession = 55,
        LapCompleted = 73,
        Version = 56,
        Chat = 57,
        ClientLoaded = 58,
        SessionInfo = 59,
        Error = 60,

        ClientEvent = 130,
    }

    public enum CollisionType : byte
    {
        CollisionWithCar = 10,
        CollisionWithEnv = 11
    }

    public enum ACSCommand : byte
    {
        // COMMANDS
        RealtimeposInterval = 200,
        GetCarInfo = 201,
        SendChat = 202, // Sends chat to one car
        BroadcastChat = 203, // Sends chat to everybody
        GetSessionInfo = 204,
        SetSessionInfo = 205,
        KickUser = 206,
        NextSession = 207,
        RestartSession = 208,
        AdminCommand = 209, // Send message plus a stringW with the command
    }

    public enum SessionType : byte
    {
        None,
        Practice = 1,
        Qualifying = 2,
        Race = 3
    }

    public enum ForwardHandling
    {
        Forward,
        Block,
    }

    public class Parsing
    {
        internal static string ReadAsciiString( BinaryReader br )
        {
            byte length = br.ReadByte();
            return new String( br.ReadChars( length ) );
        }

        internal static string ReadUnicodeString( BinaryReader br )
        {
            byte length = br.ReadByte();
            return Encoding.UTF32.GetString( br.ReadBytes( length * 4 ) );
        }

        public static ACSCommand ReadCommandId( byte id )
        {
            switch( id )
            {
                case 200:
                    return ACSCommand.RealtimeposInterval;
                case 201:
                    return ACSCommand.GetCarInfo;
                case 202:
                    return ACSCommand.SendChat;
                case 203:
                    return ACSCommand.BroadcastChat;
                case 204:
                    return ACSCommand.GetSessionInfo;
                case 205:
                    return ACSCommand.SetSessionInfo;
                case 206:
                    return ACSCommand.KickUser;
                case 207:
                    return ACSCommand.NextSession;
                case 208:
                    return ACSCommand.RestartSession;
                case 209:
                    return ACSCommand.AdminCommand;

                default:
                    throw new ArgumentOutOfRangeException( nameof( id ) );
            }
        }
    }
}
