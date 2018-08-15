using System;
using System.IO;
using System.Text;
using AcPluginLib.Protocol;

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

    internal class Parsing
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
    }
}
