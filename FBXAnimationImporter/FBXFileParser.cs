using System;
using System.Collections.Generic;
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
    }
}
