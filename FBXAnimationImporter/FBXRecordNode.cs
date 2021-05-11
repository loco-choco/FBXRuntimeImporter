using System;
using System.Collections.Generic;
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
            
            List<byte> byteAsList = fullList.GetRange(position, (int)EndOffset - position);
            FBXPacketReader reader = new FBXPacketReader(byteAsList.ToArray());

            reader.ReadUInt32(); //EndOffset
            NumProperties = reader.ReadUInt32();
            PropertyListLen = reader.ReadUInt32();

            NameLen = reader.ReadByte();
            Name = "";
            for (int i = 0; i < NameLen; i++)
                Name += reader.ReadChar();


            NestedRecords = new List<FBXRecordNode>();
            PropertyList = new List<FBXProperty>();
            if (EndOffset == 0) //Beacuse all the nested nodes are reading from the same reader, we need to read even if it is an empty node
                return;
            
            //PropertyList
            for (uint i = 0; i < NumProperties; i++)
                PropertyList.Add(reader.ReadProperty()); //Something aint working here, the size of the properties shoudn't be smaller then the number of properties
            
            //Nested Records
            if (byteAsList.Count > 13 && EndOffset != 0)
                if (byteAsList.GetRange(byteAsList.Count - 13, 13).TrueForAll(new Predicate<byte>(IsNull)))
                {
                    //the 13 bytes are there for a reason, there needs to be ONE empty node AND a NOT completed node (7 + 7 = 14, but only 13 bytes...)
                    while (true)
                        try
                        {
                            NestedRecords.Add(new FBXRecordNode(ref reader, ref byteAsList, position));
                        }
                        catch { break; }                    
                }
        }
        public FBXRecordNode(ref FBXPacketReader reader, ref List<byte> partialList, int positionOfList)
        {
            EndOffset = reader.ReadUInt32();
            NumProperties = reader.ReadUInt32();
            PropertyListLen = reader.ReadUInt32();
            NameLen = reader.ReadByte();

            Name = "";
            for (int i = 0; i < NameLen; i++)
                Name += reader.ReadChar();


            PropertyList = new List<FBXProperty>();
            NestedRecords = new List<FBXRecordNode>();
            if (EndOffset == 0)
                return;

            for (uint i = 0; i < NumProperties; i++)
                PropertyList.Add(reader.ReadProperty());

            if (EndOffset > positionOfList)
            {
                List<byte> possibleNullTrail = partialList.GetRange((int)EndOffset-positionOfList - 13, 13);
                if (possibleNullTrail.TrueForAll(new Predicate<byte>(IsNull)))
                {
                    while (true)
                        try
                        {
                            NestedRecords.Add(new FBXRecordNode(ref reader, ref partialList, positionOfList));
                        }
                        catch
                        {
                            break;
                        }                 
                }
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
