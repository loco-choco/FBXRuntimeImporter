using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FBXRuntimeImporter
{
    public class FBXFileParser
    {
        //Main nodes
        public FBXRecordNode FBXHeaderExtension;
        public FBXRecordNode FileId;
        public FBXRecordNode CreationTime;
        public FBXRecordNode Creator;
        public FBXRecordNode GlobalSettings;
        public FBXRecordNode Documents;
        public FBXRecordNode References;
        public FBXRecordNode Definitions;
        public FBXRecordNode Objects;
        public FBXRecordNode Connections;
        public FBXRecordNode Takes;

        //EX: 7400 (7.4)
        public int FileVersion;

        public FBXFileParser(string filePath)
        {
            if (!filePath.EndsWith(".fbx"))
                throw new Exception("Incorrect File Type");

            List<byte> fbxFile = new List<byte>(File.ReadAllBytes(filePath));
            FileVersion = BitConverter.ToInt32(fbxFile.GetRange(23, 26).ToArray(), 0);

            FBXHeaderExtension = new FBXRecordNode(ref fbxFile, 27);
            FileId = new FBXRecordNode(ref fbxFile, (int)FBXHeaderExtension.EndOffset);
            CreationTime = new FBXRecordNode(ref fbxFile, (int)FileId.EndOffset);
            Creator = new FBXRecordNode(ref fbxFile, (int)CreationTime.EndOffset);
            GlobalSettings = new FBXRecordNode(ref fbxFile, (int)Creator.EndOffset);
            Documents = new FBXRecordNode(ref fbxFile, (int)GlobalSettings.EndOffset);
            References = new FBXRecordNode(ref fbxFile, (int)Documents.EndOffset);
            Definitions = new FBXRecordNode(ref fbxFile, (int)References.EndOffset);
            Objects = new FBXRecordNode(ref fbxFile, (int)Definitions.EndOffset);
            Connections = new FBXRecordNode(ref fbxFile, (int)Objects.EndOffset);
            Takes = new FBXRecordNode(ref fbxFile, (int)Connections.EndOffset);
        }

        public string AllNodesToString()
        {
            return FBXHeaderExtension.ToString()
                    + "\n\n" + FileId.ToString()
                    + "\n\n" + CreationTime.ToString()
                    + "\n\n" + Creator.ToString()
                    + "\n\n" + GlobalSettings.ToString()
                    + "\n\n" + Documents.ToString()
                    + "\n\n" + References.ToString()
                    + "\n\n" + Definitions.ToString()
                    + "\n\n" + Objects.ToString()
                    + "\n\n" + Connections.ToString()
                    + "\n\n" + Takes.ToString();
        }
    }
}
