using System;
using System.IO;

namespace AcPluginLib.Protocol
{
    public class CarUpdateInfo
    {
        public byte CarId { get; }
        public Vector3F Position { get; }
        public Vector3F Velocity { get; }
        public byte Gear { get; }
        public UInt16 EngineRpm { get; }
        public float SplinePos { get; }

        internal CarUpdateInfo( byte carId, Vector3F position, Vector3F velocity, byte gear, ushort engineRpm, float splinePos )
        {
            CarId = carId;
            Position = position;
            Velocity = velocity;
            Gear = gear;
            EngineRpm = engineRpm;
            SplinePos = splinePos;
        }

        internal static CarUpdateInfo Parse( BinaryReader br )
        {
            return new CarUpdateInfo(
                br.ReadByte(),
                Vector3F.Parse( br ), 
                Vector3F.Parse( br ), 
                br.ReadByte(),
                br.ReadUInt16(),
                br.ReadSingle()
            );
        }

        public override string ToString()
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendFormat( "{0} {{", nameof( CarUpdateInfo ) ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( CarId ), CarId.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( Position ), Position.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( Velocity ), Velocity.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( Gear ), Gear.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( EngineRpm ), EngineRpm.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( SplinePos ), SplinePos.ToString() ).AppendLine();
            builder.AppendFormat( "}}" ).AppendLine();
            return builder.ToString();
        }
    }
}