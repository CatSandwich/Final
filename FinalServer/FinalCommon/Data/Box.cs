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

        public void Deserialize(DeserializeEvent e)
        {
            Position = e.Reader.ReadSerializable<Vector3>();
            Size = e.Reader.ReadSerializable<Vector2>();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Position);
            e.Writer.Write(Size);
        }
    }
}
