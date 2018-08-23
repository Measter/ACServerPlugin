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

        public override string ToString()
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendFormat( "{0} {{", nameof( ChatMessage ) ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( CarId ), CarId.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( Message ), Message.ToString() ).AppendLine();
            builder.AppendFormat( "}}" ).AppendLine();
            return builder.ToString();
        }
    }
}