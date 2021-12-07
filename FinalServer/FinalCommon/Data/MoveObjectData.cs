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
        public ObjectIds Id;
        public Vector3 Position;

        public MoveObjectData()
        {

        }

        public MoveObjectData(ObjectIds id, Vector3 position)
        {
            Id = id;
            Position = position;
        }

        public void Deserialize(DeserializeEvent e)
        {
            Id = (ObjectIds)e.Reader.ReadUInt16();
            Position = e.Reader.ReadSerializable<Vector3>();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write((ushort)Id);
            e.Writer.Write(Position);
        }
    }
}
