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
    }
}