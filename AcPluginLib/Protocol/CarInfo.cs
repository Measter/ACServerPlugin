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
    }
}