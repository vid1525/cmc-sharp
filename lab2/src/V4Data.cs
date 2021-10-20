using System;
using System.Collections;
using System.Collections.Generic;


namespace lab2
{
    abstract class V4Data : IEnumerable<DataItem>
    {
        protected const string DATA_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";
        public string ObjectType {get; protected set;}
        public DateTime LastChangeDate {get; protected set;}

        public V4Data(string objectType, DateTime date)
        {
            this.ObjectType = objectType;
            this.LastChangeDate = date;
        }

        public abstract IEnumerator<DataItem> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public override string ToString() => $"type: {ObjectType}; last change date: {LastChangeDate}";
        public abstract int Count {get;}
        public abstract float MaxFromOrigin {get;}
        public abstract string ToLongString(string format);
    }
}