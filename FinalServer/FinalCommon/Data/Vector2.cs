using DarkRift;

namespace FinalCommon.Data
{
    // Stores 2 dimensions of floating point values
    public class Vector2 : IDarkRiftSerializable
    {
        public float X;
        public float Y;

        public Vector2() { }

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public void Deserialize(DeserializeEvent e)
        {
            X = e.Reader.ReadSingle();
            Y = e.Reader.ReadSingle();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(X);
            e.Writer.Write(Y);
        }

        public static Vector2 operator *(Vector2 lhs, int rhs) => new Vector2(lhs.X * rhs, lhs.Y * rhs);
        public static Vector2 operator *(Vector2 lhs, float rhs) => new Vector2(lhs.X * rhs, lhs.Y * rhs);
        public static Vector2 operator /(Vector2 lhs, int rhs) => new Vector2(lhs.X / rhs, lhs.Y / rhs);
        public static Vector2 operator /(Vector2 lhs, float rhs) => new Vector2(lhs.X / rhs, lhs.Y / rhs);
    }
}
