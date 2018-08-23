using System;
using System.IO;

namespace AcPluginLib.Protocol
{
    public class ConnectionInfo
    {
        public string DriverName { get; }
        public string DriverGuid { get; }
        public byte CarId { get; }
        public string CarModel { get; }
        public string CarSkin { get; }

        internal ConnectionInfo( string driverName, string driverGuid, byte carId, string carModel, string carSkin )
        {
            DriverName = driverName ?? throw new ArgumentNullException( nameof( driverName ) );
            DriverGuid = driverGuid ?? throw new ArgumentNullException( nameof( driverGuid ) );
            CarId = carId;
            CarModel = carModel ?? throw new ArgumentNullException( nameof( carModel ) );
            CarSkin = carSkin ?? throw new ArgumentNullException( nameof( carSkin ) );
        }

        internal static ConnectionInfo Parse( BinaryReader br )
        {
            return new ConnectionInfo( 
                Parsing.ReadUnicodeString( br ),
                Parsing.ReadUnicodeString( br ),
                br.ReadByte(),
                Parsing.ReadAsciiString( br ),
                Parsing.ReadAsciiString( br )
            );
        }

        public override string ToString()
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendFormat( "{0} {{", nameof( ConnectionInfo ) ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( DriverName ), DriverName.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( DriverGuid ), DriverGuid.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( CarId ), CarId.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( CarModel ), CarModel.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( CarSkin ), CarSkin.ToString() ).AppendLine();
            builder.AppendFormat( "}}" ).AppendLine();
            return builder.ToString();
        }
    }
}