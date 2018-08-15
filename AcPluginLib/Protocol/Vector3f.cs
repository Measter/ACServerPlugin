using System;
using System.IO;

namespace AcPluginLib.Protocol
{
    public struct Vector3F
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        public Vector3F( float x, float y, float z )
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double Length() => Math.Sqrt( X * X + Y * Y + Z * Z );

        public static Vector3F operator -( Vector3F v, Vector3F w ) => new Vector3F( v.X - w.X, v.Y - w.Y, v.Z - w.Z );
        public static Vector3F operator +( Vector3F v, Vector3F w ) => new Vector3F( v.X + w.X, v.Y + w.Y, v.Z + w.Z );

        public override string ToString() => $"[{X}, {Y}, {Z}]";

        internal static Vector3F Parse( BinaryReader reader )
        {
            return new Vector3F( reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle() );
        }
    }
}
