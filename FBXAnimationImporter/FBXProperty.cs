using System;
using System.Collections.Generic;
using System.Text;

namespace FBXAnimationImporter
{
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
            /*if(objType.IsArray) idea to convert array to an string
			{
				string s = "";
                foreach(object a in (Array)Convert.ChangeType(obj,objType))
                    s += " " a.ToString();
				return objType.ToString() +" : " + s;
			}*/
            return objType.ToString() + " : " + Convert.ChangeType(obj, objType).ToString();
        }
    }
}
