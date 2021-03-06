using DarkRift;

namespace FinalCommon.Data
{
    // Stores 3 dimensions of floating point values
    public class Vector3 : IDarkRiftSerializable
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3() { }

        public Vector3(float x = 0, float y = 0, float z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public void Deserialize(DeserializeEvent e)
        {
            X = e.Reader.ReadSingle();
            Y = e.Reader.ReadSingle();
            Z = e.Reader.ReadSingle();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(X);
            e.Writer.Write(Y);
            e.Writer.Write(Z);
        }

        public static implicit operator Vector3(Vector2 v2) => new Vector3 { X = v2.X, Y = v2.Y, Z = 0 };
        public static Vector3 operator+(Vector3 lhs, Vector3 rhs) => new Vector3 { X = lhs.X + rhs.X, Y = lhs.Y + rhs.Y, Z = lhs.Z + rhs.Z};
    }
}
