using System;
using System.IO;

namespace AcPluginLib
{
    public class CarInfo
    {
        public byte CarId { get; }
        public bool IsConnected { get; }
        public string CarModel { get; }
        public string CarSkin { get; }
        public string DriverName { get; }
        public string DriverTeam { get; }
        public string DriverGuid { get; }

        internal CarInfo( byte carId, bool isConnected, string carModel, string carSkin, string driverName, string driverTeam, string driverGuid )
        {
            CarId = carId;
            IsConnected = isConnected;
            CarModel = carModel ?? throw new ArgumentNullException( nameof( carModel ) );
            CarSkin = carSkin ?? throw new ArgumentNullException( nameof( carSkin ) );
            DriverName = driverName ?? throw new ArgumentNullException( nameof( driverName ) );
            DriverTeam = driverTeam ?? throw new ArgumentNullException( nameof( driverTeam ) );
            DriverGuid = driverGuid ?? throw new ArgumentNullException( nameof( driverGuid ) );
        }

        internal static CarInfo Parse( BinaryReader br )
        {
            return new CarInfo(
                br.ReadByte(),
                br.ReadBoolean(),
                Parsing.ReadUnicodeString( br ),
                Parsing.ReadUnicodeString( br ),
                Parsing.ReadUnicodeString( br ),
                Parsing.ReadUnicodeString( br ),
                Parsing.ReadUnicodeString( br )
            );
        }

        public override string ToString()
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendFormat( "{0} {{", nameof( CarInfo ) ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( CarId ), CarId.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( IsConnected ), IsConnected.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( CarModel ), CarModel.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( CarSkin ), CarSkin.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( DriverName ), DriverName.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( DriverTeam ), DriverTeam.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( DriverGuid ), DriverGuid.ToString() ).AppendLine();
            builder.AppendFormat( "}}" ).AppendLine();
            return builder.ToString();
        }
    }
}