using System;
using AcPluginLib.Protocol;

namespace AcPluginLib
{
    public class Driver
    {
        public byte? CarId { get; internal set; }
        public string CarModel { get; internal set; }
        public string CarSkin { get; internal set; }

        public string Name { get; internal set; }
        public string Team { get; internal set; }
        public string GUID { get; }
        public bool IsConnected { get; internal set; }

        public Vector3F? Position { get; private set; }
        public Vector3F? VelocityVector { get; private set; }
        public double? Speed { get; private set; }
        public float? SplinePosition { get; internal set; }

        public Driver( string guid )
        {
            GUID = guid ?? throw new ArgumentNullException( nameof( guid ) );
        }

        internal void SetPositionAndSpeed( Vector3F pos, Vector3F vel )
        {
            Speed = vel.Length() * 3.6; // Km/H
            VelocityVector = vel;
            Position = pos;
        }

        public override string ToString()
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendFormat( "{0} {{", nameof( Driver ) ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( CarId ), CarId.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( CarModel ), CarModel.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( CarSkin ), CarSkin.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( Name ), Name.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( Team ), Team.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( GUID ), GUID.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( IsConnected ), IsConnected.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( Position ), Position.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( VelocityVector ), VelocityVector.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( Speed ), Speed.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( SplinePosition ), SplinePosition.ToString() ).AppendLine();
            builder.AppendFormat( "}}" ).AppendLine();
            return builder.ToString();
        }
    }
}