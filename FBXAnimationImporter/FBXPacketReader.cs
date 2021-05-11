using System;
using System.IO;

namespace FBXAnimationImporter
{
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
                return ReadBytes(checked((int)(arrayDataTypeSize * arrayLength)));

            else if (encoding == 1)
                throw new Exception("Encoding 1 still not supported :(");
            //return new ReadBytes(checked((int)compressedLength))); Descompress with com zlib, but not going to do it now ;)
            else
                throw new Exception("This type of enconding wasn't normally observerd, here is the number that represents it: " + encoding);
        }

        public FBXProperty ReadProperty()
        {
            char caractere = ReadChar();
            byte[] dataInBytes;
            switch (caractere)
            {
                //Simples
                case 'C':
                    return new FBXProperty(ReadBoolean());

                case 'Y':
                    return new FBXProperty(ReadInt16());
                case 'I':
                    return new FBXProperty(ReadInt32());
                case 'L':
                    return new FBXProperty(ReadInt64());
                //IEEE 754
                case 'F':
                    return new FBXProperty(ReadSingle());
                case 'D':
                    return new FBXProperty(ReadDouble());

                //Complexos (Ahhhh eu n queria copiar tudo em todos, mas n sei como faria isso dentro de uma função ;-;)
                case 'b':
                    dataInBytes = ReadDataArrayBytes(1);
                    bool[] bools = new bool[dataInBytes.Length];
                    for (int index = 0; index < bools.Length; index++)
                        bools[index] = BitConverter.ToBoolean(dataInBytes, index);

                    return new FBXProperty(bools);

                case 'l':
                    dataInBytes = ReadDataArrayBytes(1);
                    long[] longs = new long[dataInBytes.Length / 8];
                    for (int index = 0; index < longs.Length; index++)
                        longs[index] = BitConverter.ToInt64(dataInBytes, index * 8);

                    return new FBXProperty(longs);

                case 'i':
                    dataInBytes = ReadDataArrayBytes(1);
                    int[] ints = new int[dataInBytes.Length / 4];
                    for (int index = 0; index < ints.Length; index++)
                        ints[index] = BitConverter.ToInt32(dataInBytes, index * 4);

                    return new FBXProperty(ints);

                case 'f':
                    dataInBytes = ReadDataArrayBytes(1);
                    float[] floats = new float[dataInBytes.Length / 4];
                    for (int index = 0; index < floats.Length; index++)
                        floats[index] = BitConverter.ToSingle(dataInBytes, index * 4);

                    return new FBXProperty(floats);

                case 'd':
                    dataInBytes = ReadDataArrayBytes(1);
                    double[] doubles = new double[dataInBytes.Length / 4];
                    for (int index = 0; index < doubles.Length; index++)
                        doubles[index] = BitConverter.ToDouble(dataInBytes, index * 8);

                    return new FBXProperty(doubles);

                //Tipos mais "exóticos"
                case 'S':
                    dataInBytes = ReadBytes(checked((int)ReadUInt32()));
                    return new FBXProperty(BitConverter.ToString(dataInBytes));

                case 'R': //Raw Bytes
                    return new FBXProperty(ReadBytes(checked((int)ReadUInt32())));
            }

            throw new Exception("Nao foi possivel ler o tipo dessa propiedade, o char lido fora: " + caractere);
        }
    }
}
