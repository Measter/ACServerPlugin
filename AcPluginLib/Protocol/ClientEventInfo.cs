using System;
using System.IO;

namespace AcPluginLib.Protocol
{
    public class ClientEventInfo
    {
        public CollisionType CollisionType { get; }
        public byte CarId { get; }
        public byte? OtherCarId { get; }
        public float Speed { get; }
        public Vector3F WorldPosition { get; }
        public Vector3F RelativePosition { get; }

        internal ClientEventInfo( CollisionType collisionType, byte carId, byte? otherCarId, float speed, Vector3F worldPosition, Vector3F relativePosition )
        {
            CollisionType = collisionType;
            CarId = carId;
            OtherCarId = otherCarId;
            Speed = speed;
            WorldPosition = worldPosition;
            RelativePosition = relativePosition;
        }

        internal static ClientEventInfo Parse( BinaryReader br )
        {
            var type = br.ReadByte();

            CollisionType colType;
            switch( type )
            {
                case 10:
                    colType = CollisionType.CollisionWithCar;
                    break;
                case 11:
                    colType = CollisionType.CollisionWithEnv;
                    break;
                default:
                    throw new Exception($"Unknown Client Event Type: {type}");
            }
            
            return new ClientEventInfo( 
                colType,
                br.ReadByte(),
                colType == CollisionType.CollisionWithCar ? br.ReadByte() : (byte?) null,
                br.ReadSingle(),
                Vector3F.Parse( br ), 
                Vector3F.Parse( br )
            );
        }

        public override string ToString()
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendFormat( "{0} {{", nameof( ClientEventInfo ) ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( CollisionType ), CollisionType.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( CarId ), CarId.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( OtherCarId ), OtherCarId.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( Speed ), Speed.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( WorldPosition ), WorldPosition.ToString() ).AppendLine();
            builder.AppendFormat( "    {0} = {1}", nameof( RelativePosition ), RelativePosition.ToString() ).AppendLine();
            builder.AppendFormat( "}}" ).AppendLine();
            return builder.ToString();
        }
    }
}