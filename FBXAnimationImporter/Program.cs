using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace FBXAnimationImporter
{
    //https://code.blender.org/2013/08/fbx-binary-file-format-specification/ Referencia dessa loucura
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = "";

            while (!filePath.EndsWith(".fbx"))
            {
                Console.Write("Caminho do arquivo (.fbx): ");
                filePath = Console.ReadLine();
            }

            List<byte> fbxFile;
            try{
                fbxFile = new List<byte>(File.ReadAllBytes(filePath));
            }
            catch{
                Console.WriteLine("Nao deu para ler o arquivo");
                Console.Read();
                return;
            }

            uint fileVersion = BitConverter.ToUInt32(fbxFile.GetRange(23,26).ToArray(),0);
            Console.WriteLine("Versao do Arquivo: " + fileVersion);
            Console.WriteLine("Tamanho do Arquivo: " + fbxFile.Count);

            const ulong firstByteAfterTheHeader = 27; //O Header vai de 0 - 26, então começamos no 27

            List<FBXRecordNode> AllRecordNodes = new List<FBXRecordNode>() {new FBXRecordNode(ref fbxFile,(int)firstByteAfterTheHeader) };

            while (AllRecordNodes[AllRecordNodes.Count - 1].EndOffset < fbxFile.Count)
                try
                {
                    AllRecordNodes.Add(new FBXRecordNode(ref fbxFile, (int)AllRecordNodes[AllRecordNodes.Count - 1].EndOffset));
                }
                catch(Exception ex)
                {
                    if (ex.GetType() == typeof(FBXRecordNodeException))
                        break;
                }
            
            foreach (var node in AllRecordNodes)
            {
                Console.Write("/\n/\n/\n"); //Parede de / / /
                Console.WriteLine(node);
            }
            Console.Read();
        }
    }
    public struct FBXProperty
    {
        public object obj;
        public Type objType;

        public FBXProperty(object obj, Type objType)
        {
            this.obj = obj;
            this.objType = objType;
        }

        override public string ToString()
        {
			/*if(objType.IsArray) ideia de converter array em string
			{
				string s = "";
                foreach(object a in (Array)Convert.ChangeType(obj,objType))
                    s += " " a.ToString();
				return objType.ToString() +" : " + s;
			}*/			
            return objType.ToString() +" : " + Convert.ChangeType(obj, objType).ToString();
        }
    }
    
    public class FBXPacketReader : BinaryReader
    {
        public FBXPacketReader(byte[] data) : base(new MemoryStream(data))
        {
        }

        private byte[] ReadDataArrayBytes(byte arrayDataTypeSize)
        {
            uint arrayLength = ReadUInt32();
            uint encoding = ReadUInt32();
            uint compressedLength = ReadUInt32();
            if (encoding == 0)
                return  ReadBytes(checked((int)(arrayDataTypeSize * arrayLength)));

            else if (encoding == 1)
                throw new Exception("Encoding ainda nao e suportado :(");
            //return new ReadBytes(checked((int)compressedLength))); Descomprimir com zlib, mas no vou fazer isso agora ;)
            else
                throw new Exception("Tal tipo de codificacao nunca foi observado normalmente: " + encoding);
        }

        public FBXProperty ReadProperty()
        {
            char caractere = ReadChar();
            byte[] dataInBytes;
            switch (caractere)
            {
                //Simples
                case 'C':
                    return new FBXProperty(ReadBoolean(), typeof(bool));

                case 'Y':
                    return new FBXProperty(ReadInt16(), typeof(short));
                case 'I':
                    return new FBXProperty(ReadInt32(), typeof(int));
                case 'L':
                    return new FBXProperty(ReadInt64(), typeof(long));
                //IEEE 754
                case 'F':
                    return new FBXProperty(ReadSingle(), typeof(float));
                case 'D':
                    return new FBXProperty(ReadDouble(), typeof(double));

                //Complexos (Ahhhh eu n queria copiar tudo em todos, mas n sei como faria isso dentro de uma função ;-;)
                case 'b':
                    dataInBytes = ReadDataArrayBytes(1);
                    bool[] bools = new bool[dataInBytes.Length];
                    for (int index = 0; index < bools.Length; index++)
                        bools[index] = BitConverter.ToBoolean(dataInBytes,index);

                    return new FBXProperty(bools, typeof(bool[]));

                case 'l':
                    dataInBytes = ReadDataArrayBytes(1);
                    long[] longs = new long[dataInBytes.Length/8];
                    for (int index = 0; index < longs.Length; index++)
                        longs[index] = BitConverter.ToInt64(dataInBytes, index*8);

                    return new FBXProperty(longs, typeof(long[]));

                case 'i':
                    dataInBytes = ReadDataArrayBytes(1);
                    int[] ints = new int[dataInBytes.Length / 4];
                    for (int index = 0; index < ints.Length; index++)
                        ints[index] = BitConverter.ToInt32(dataInBytes, index * 4);

                    return new FBXProperty(ints, typeof(int[]));

                case 'f':
                    dataInBytes = ReadDataArrayBytes(1);
                    float[] floats = new float[dataInBytes.Length / 4];
                    for (int index = 0; index < floats.Length; index++)
                        floats[index] = BitConverter.ToSingle(dataInBytes, index * 4);

                    return new FBXProperty(floats, typeof(float[]));

                case 'd':
                    dataInBytes = ReadDataArrayBytes(1);
                    double[] doubles = new double[dataInBytes.Length / 4];
                    for (int index = 0; index < doubles.Length; index++)
                        doubles[index] = BitConverter.ToDouble(dataInBytes, index * 8);

                    return new FBXProperty(doubles, typeof(double[]));

                //Tipos mais "exóticos"
                case 'S':
                    dataInBytes = ReadBytes(checked((int)ReadUInt32()));
                    return new FBXProperty(BitConverter.ToString(dataInBytes), typeof(string));

                case 'R': //Raw Bytes
                    return new FBXProperty(ReadBytes(checked((int)ReadUInt32())), typeof(byte[]));
            }

            throw new Exception("Nao foi possivel ler o tipo dessa propiedade, o char lido fora: "+ caractere);
        }
    }

    public class FBXRecordNodeException : Exception
    {
        public FBXRecordNodeException(string Message) : base(Message)
        { }
    }

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
            if (EndOffset < 1)
                throw new FBXRecordNodeException("EndOffset deu valor igual ou menor que 0");
            List<byte> byteAsList = fullList.GetRange(position, (int)EndOffset - position);
            FBXPacketReader reader = new FBXPacketReader(byteAsList.ToArray());

            reader.ReadUInt32(); //EndOffset
            NumProperties = reader.ReadUInt32();
            PropertyListLen = reader.ReadUInt32();

            NameLen = reader.ReadByte();
            Name = "";
            for (int i = 0; i < NameLen; i++)
                Name += reader.ReadChar();

            PropertyList = new List<FBXProperty>();
            for (uint i = 0; i < NumProperties; i++)
                PropertyList.Add(reader.ReadProperty());

            NestedRecords = new List<FBXRecordNode>();

            if (byteAsList.Count > 13)
                if (byteAsList.GetRange(byteAsList.Count - 13, 13).TrueForAll(new Predicate<byte>(IsNull)))
                {
                    uint lastEndOffset = 0;
                    do
                    {
                        try
                        {
                            var node = new FBXRecordNode(ref reader, ref fullList);
                            NestedRecords.Add(node);
                            lastEndOffset = node.EndOffset;
                        }
                        catch { break; }
                    } while (lastEndOffset < EndOffset - 13);
                }
        }
        public FBXRecordNode(ref FBXPacketReader reader, ref List<byte> fullList)
        {
            EndOffset = reader.ReadUInt32();
            NumProperties = reader.ReadUInt32();
            PropertyListLen = reader.ReadUInt32();
            NameLen = reader.ReadByte();

            Name = "";
            for (int i = 0; i < NameLen; i++)
                Name += reader.ReadChar();

            PropertyList = new List<FBXProperty>();
            for (uint i = 0; i < NumProperties; i++)
                PropertyList.Add(reader.ReadProperty());

            NestedRecords = new List<FBXRecordNode>();
            if (EndOffset > 13)
            {
                List<byte> possibleNullTrail = fullList.GetRange((int)EndOffset - 13, 13);
                if (possibleNullTrail.TrueForAll(new Predicate<byte>(IsNull)))
                {
                    uint lastEndOffset = 0;
                    do
                    {
                        NestedRecords.Add(new FBXRecordNode(ref reader, ref fullList));
                        lastEndOffset = NestedRecords[NestedRecords.Count - 1].EndOffset;
                    } while (lastEndOffset < EndOffset - 13);
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
                + $" Size of Nested Records - {NestedRecords.Count}< " + sRecords +"> ";
        }
    }
}
