using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FBXAnimationImporter
{
    public class FBXRecordNode
    {
        public uint EndOffset;
        public uint NumProperties;
        public uint PropertyListLen;
        public byte NameLen;
        public string Name;
        public List<FBXProperty> PropertyList;
        public List<FBXRecordNode> NestedRecords;

        public FBXRecordNode(ref List<byte> fullList, int position)
        {
            EndOffset = BitConverter.ToUInt32(fullList.GetRange(position, 4).ToArray(), 0);
            List<byte> byteAsList;

            if (EndOffset != 0) //
                byteAsList = fullList.GetRange(position + 4, (int)EndOffset - position - 4);
            else
                byteAsList = fullList.GetRange(position+ 4, 13 - 4);

            FBXPacketReader reader = new FBXPacketReader(byteAsList.ToArray());
            NumProperties = reader.ReadUInt32();
            PropertyListLen = reader.ReadUInt32();

            try
            {
                NameLen = reader.ReadByte();
                Name = "";
                for (int i = 0; i < NameLen; i++)
                    Name += reader.ReadChar();
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(EndOfStreamException))
                    throw new FBXRecordNodeNotEnoughBytesException("No 25th byte, may be the end of a nested record node");
                else
                    throw ex;
            }

            NestedRecords = new List<FBXRecordNode>();
            PropertyList = new List<FBXProperty>();
            //if (EndOffset == 0) //Because all the nested nodes are reading from the same reader, we need to read even if it is an empty node
            //    return;
            
            //PropertyList
            for (uint i = 0; i < NumProperties; i++)
                PropertyList.Add(reader.ReadProperty()); //Something aint working here, the size of the properties shoudn't be smaller then the number of properties
            
            //Nested Records
            if (byteAsList.Count > 13 && EndOffset != 0)
                if (byteAsList.GetRange(byteAsList.Count - 13, 13).TrueForAll(new Predicate<byte>(IsNull)))
                {
                    //the 13(25) bytes are there for a reason, there needs to be ONE empty node AND a NOT completed node (13 + 13 = 26, but only 25 bytes...)

                    //new position = old position + 13 bytes that are always read + the amount of bytes in the name +the size in bytes of the PropList
                    NestedRecords.Add(new FBXRecordNode(ref fullList, position + 13 + NameLen + (int)PropertyListLen));
                    while (NestedRecords[NestedRecords.Count - 1].EndOffset < EndOffset - 13 && (position + 26 + NameLen + (int)PropertyListLen < EndOffset)) //Find a better way to generate nested nodes (Nested Nested nodes aren't appearing :/)
                        NestedRecords.Add(new FBXRecordNode(ref fullList, (int)NestedRecords[NestedRecords.Count-1].EndOffset));
                }
        }
        private bool IsNull(byte b)
        {
            return b == 0;
        }

        public override string ToString()
        {
            string s = "";            
            foreach (FBXProperty property in PropertyList)
                s += property.ToString() + '\n';

            string sRecords = "";
            foreach (FBXRecordNode record in NestedRecords)
                sRecords += record.ToString() + '\n';

            return $"{Name}: EndOffset - {EndOffset} | Size of Properties - {PropertyListLen} | Num of Properties - {NumProperties} :" + s
                + $" Size of Nested Records - {NestedRecords.Count}< " + sRecords + "> ";
        }
    }
}
