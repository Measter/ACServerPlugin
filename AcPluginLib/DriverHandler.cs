using System.Collections.Generic;
using AcPluginLib.Protocol;

namespace AcPluginLib
{
    public class DriverDB: ACEventHandler
    {
        private readonly DriverHandler m_handler;

        internal DriverDB( DriverHandler handler )
        {
            m_handler = handler;
        }

        public bool TryGetDriverByGUID( string guid, out Driver driver )
        {
            return m_handler.TryGetDriverByGUID( guid, out driver );
        }

        public bool TryGetDriverByID( byte id, out Driver driver )
        {
            return m_handler.TryGetDriverByID( id, out driver );
        }
    }

    internal class DriverHandler : ACEventHandler
    {
        private static readonly NLog.Logger m_logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, Driver> m_driversFromGUID = new Dictionary<string, Driver>();
        private readonly Dictionary<byte, Driver> m_driversFromID = new Dictionary<byte, Driver>();

        public override void OnCarInfo( Commander cmdr, CarInfo info )
        {
            if( info.DriverGuid.Length == 0 )
                return;

            var driver = GetFromGUID( info.DriverGuid );

            var oldID = driver.CarId;

            driver.Name = info.DriverName;
            driver.Team = info.DriverTeam;
            driver.IsConnected = info.IsConnected;
            driver.CarId = info.CarId;
            driver.CarModel = info.CarModel;
            driver.CarSkin = info.CarSkin;

            if( oldID.HasValue && oldID.Value != info.CarId && m_driversFromID.TryGetValue( oldID.Value, out Driver oldDriver ) )
            {
                m_logger.Debug( "Moving driver from ID {0} to ID  {1}", oldID.Value, info.CarId );
                m_logger.Trace( "Old driver info: {0}", oldDriver );
                m_driversFromID.Remove( oldID.Value );

                if( oldDriver.GUID != info.DriverGuid )
                {
                    m_logger.Warn( "Old driver ({0}) is not the same as new driver ({1})", oldDriver.GUID, driver.GUID );
                    oldDriver.CarId = null;
                }
            }

            m_driversFromID[info.CarId] = driver;
        }

        public override void OnCarUpdate( Commander cmdr, CarUpdateInfo info )
        {
            if( !TryGetDriverByID( info.CarId, out Driver driver ) )
            {
                m_logger.Info( "Unknown driver {0}, requesting info", info.CarId );
                cmdr.GetCarInfo( info.CarId );
                return;
            }

            driver.SetPositionAndSpeed( info.Position, info.Velocity );
            driver.SplinePosition = info.SplinePos;
            driver.Gear = info.Gear;
            driver.EngineRPM = info.EngineRpm;
        }

        public override void OnClientLoaded( Commander cmdr, byte carId )
        {
            if( !m_driversFromID.TryGetValue( carId, out Driver driver ) )
            {
                m_logger.Info( "Unknown driver {0}, requesting info", carId );
                cmdr.GetCarInfo( carId );
                return;
            }

            driver.IsConnected = true;
        }

        public override void OnConnectionClosed( Commander cmdr, ConnectionInfo info )
        {
            if( info.DriverGuid.Length == 0 )
                return;

            m_logger.Debug( "Driver disconnected ({0})", info.DriverGuid );

            var driver = GetFromGUID( info.DriverGuid );

            var oldID = driver.CarId;

            driver.Name = info.DriverName;
            driver.IsConnected = false;
            driver.CarId = null;
            driver.CarModel = info.CarModel;
            driver.CarSkin = info.CarSkin;

            if( m_driversFromID.ContainsKey( info.CarId ) )
            {
                m_logger.Info( "Driver {0} disconnected, removing from ID map", info.CarId );
                m_driversFromID.Remove( info.CarId );
            }

            if( oldID.HasValue && oldID.Value != info.CarId && m_driversFromID.TryGetValue( oldID.Value, out Driver oldDriver ) )
            {
                m_logger.Warn( "Something dodgy going on with ID, same driver had ID {0} and ID {1}", oldID.Value, info.CarId );
                m_logger.Trace( "Old driver info: {0}", oldDriver );
                m_driversFromID.Remove( oldID.Value );

                if( oldDriver.GUID != info.DriverGuid )
                {
                    m_logger.Warn( "Old driver ({0}) is not the same as new driver ({1})", oldDriver.GUID, driver.GUID );
                    oldDriver.CarId = null;
                }
            }
        }

        public override void OnNewConnection( Commander cmdr, ConnectionInfo info )
        {
            if( info.DriverGuid.Length == 0 )
                return;

            m_logger.Debug( "Driver connected ({0})", info.DriverGuid );

            var driver = GetFromGUID( info.DriverGuid );

            var oldID = driver.CarId;

            driver.Name = info.DriverName;
            driver.IsConnected = false;
            driver.CarId = info.CarId;
            driver.CarModel = info.CarModel;
            driver.CarSkin = info.CarSkin;

            if( m_driversFromID.TryGetValue( info.CarId, out Driver oldDriverNewID ) )
            {
                m_logger.Warn( "Driver with new ID ({0}) already connected!", info.CarId );
                m_logger.Trace( "Old driver info {0}", oldDriverNewID );
                m_driversFromID.Remove( info.CarId );
                oldDriverNewID.CarId = null;
            }

            if( oldID.HasValue && oldID.Value != info.CarId && m_driversFromID.TryGetValue( oldID.Value, out Driver oldDriverOldID ) )
            {
                m_logger.Warn( "Driver had old ID {0} on new connection", oldID.Value );
                m_logger.Trace( "Old driver info: {0}", oldDriverOldID );
                m_driversFromID.Remove( oldID.Value );

                if( oldDriverOldID.GUID != info.DriverGuid )
                {
                    m_logger.Warn( "Old driver ({0}) is not the same as new driver ({1})", oldDriverOldID.GUID, driver.GUID );
                    oldDriverOldID.CarId = null;
                }
            }

            m_logger.Debug( "Adding driver to ID DB with ID {0}", info.CarId );
            m_driversFromID[info.CarId] = driver;
        }


        private Driver GetFromGUID( string guid )
        {
            Driver curDriver;
            if( m_driversFromGUID.ContainsKey( guid ) )
            {
                m_logger.Debug( "Driver {0} exists in GUID dictionary", guid );
                curDriver = m_driversFromGUID[guid];
            }
            else
            {
                m_logger.Debug( "Creating driver from GUID {0}", guid );
                m_driversFromGUID[guid] = curDriver = new Driver( guid );
            }

            return curDriver;
        }

        public bool TryGetDriverByGUID( string guid, out Driver driver )
        {
            return m_driversFromGUID.TryGetValue( guid, out driver );
        }

        public bool TryGetDriverByID( byte id, out Driver driver )
        {
            return m_driversFromID.TryGetValue( id, out driver );
        }
    }
}