using System;
using System.Collections.Generic;
using System.Text;

namespace FBXAnimationImporter
{
    public class FBXRecordNodeCreationException : Exception
    {
        public FBXRecordNodeCreationException(string Message) : base(Message){}
    }
    public class FBXRecordNodeNotEnoughBytesException : Exception
    {
        public FBXRecordNodeNotEnoughBytesException(string Message) : base(Message) { }
    }
}
