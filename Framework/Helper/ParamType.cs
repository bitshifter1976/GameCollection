using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    [Serializable()]
    public class ParamType
    {
        #region members

        public string Name = "";
        public Type Type = typeof(int);
        public string Min = "0";
        public string Max = "1";
        public string Val = "0";
        public string StdVal = "0";
        public string[] Collection = new string[0];

        #endregion

        #region methods

        public ParamType(string name, string val, Type type, string min, string max, string stdVal)
            : this(name, val, type, min, max, stdVal, new string[0])
        {
        }

        public ParamType(string name, string val, Type type, string min, string max, string stdVal, string[] collection)
        {
            this.Name = name;
            this.Type = type;
            this.Min = min;
            this.Max = max;
            this.Val = val;
            this.StdVal = stdVal;
            this.Collection = collection;
        }

        #endregion
    }
}
