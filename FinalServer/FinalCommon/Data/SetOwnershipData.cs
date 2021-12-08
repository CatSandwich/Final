using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkRift;

namespace FinalCommon.Data
{
    public class SetOwnershipData : IDarkRiftSerializable
    {
        public ObjectIds Id;

        public void Deserialize(DeserializeEvent e)
        {
            Id = (ObjectIds)e.Reader.ReadUInt16();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write((ushort)Id);
        }
    }
}
