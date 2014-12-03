using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace McuTools.Classes
{
    [Serializable]
    public class UsageInfo
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    [Serializable]
    public class UsageStatsDictionary : Dictionary<string, int>
    {
        public UsageStatsDictionary() : base() { }
        protected UsageStatsDictionary(SerializationInfo info, StreamingContext context): base(info, context) { }

        public new void Add(string key, int value)
        {
            if (base.ContainsKey(key)) base[key] = value;
            else base.Add(key, value);
        }

        public new int this[string key]
        {
            get 
            {
                if (base.ContainsKey(key)) return base[key];
                else return 0;
            }
            set { this.Add(key, value); }
        }

        public UsageInfo[] Pack()
        {
            var q = from i in this
                    select new UsageInfo
                    {
                        Name = i.Key,
                        Count = i.Value
                    };
            return q.ToArray();
        }

        public void Unpack(UsageInfo[] information)
        {
            this.Clear();
            foreach (var i in information) this.Add(i.Name, i.Count);
        }
    }
}
