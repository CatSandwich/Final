using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DarkRift;

namespace FinalCommon.Data
{
    public class MoveObjectData : IDarkRiftSerializable
    {
        public int Id;
        public Vector3 Position;

        public void Deserialize(DeserializeEvent e)
        {
            Id = e.Reader.ReadInt32();
            Position = e.Reader.ReadSerializable<Vector3>();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Id);
            e.Writer.Write(Position);
        }
    }
}
