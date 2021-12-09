using System.Text;
using DarkRift;

namespace FinalCommon.Data
{
    // Stores a position, Position, and a size, Size, making a box
    public class Box : IDarkRiftSerializable
    {
        public Vector3 Position;
        public Vector2 Size;

        public virtual void Deserialize(DeserializeEvent e)
        {
            Position = e.Reader.ReadSerializable<Vector3>();
            Size = e.Reader.ReadSerializable<Vector2>();
        }

        public virtual void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Position);
            e.Writer.Write(Size);
        }

        public bool CheckCollision(Box other)
        {
            if (other == null) return false;
            if (Position.Y - Size.Y / 2 > other.Position.Y + other.Size.Y / 2) return false;
            if (Position.Y + Size.Y / 2 < other.Position.Y - other.Size.Y / 2) return false;
            if (Position.X - Size.X / 2 > other.Position.X + other.Size.X / 2) return false;
            if (Position.X + Size.X / 2 < other.Position.X - other.Size.X / 2) return false;
            return true;
        }
    }
}
