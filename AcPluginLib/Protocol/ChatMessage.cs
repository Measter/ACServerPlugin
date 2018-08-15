using System;
using System.IO;

namespace AcPluginLib.Protocol
{
    public class ChatMessage
    {
        public byte CarId { get; }
        public string Message { get; }

        internal ChatMessage( byte carId, string message )
        {
            CarId = carId;
            Message = message ?? throw new ArgumentNullException( nameof( message ) );
        }

        internal static ChatMessage Parse( BinaryReader br )
        {
            return new ChatMessage( 
                br.ReadByte(),
                Parsing.ReadUnicodeString( br )
            );
        }
    }
}