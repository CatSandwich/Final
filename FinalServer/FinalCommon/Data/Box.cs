using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DarkRift;

namespace FinalCommon.Data
{
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

        public bool CheckYAxisCollision(Box other)
        {
            return !(Position.Y + Size.Y / 2 < other.Position.Y - other.Size.Y / 2 ||
                Position.Y - Size.Y / 2 > other.Position.Y + other.Size.Y / 2);
        }
    }
}
