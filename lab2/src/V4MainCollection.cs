using System;
using System.Collections.Generic;


namespace lab2
{
    class V4MainCollection
    {
        public int Count {get => DataList.Count;}

        private List<V4Data> DataList;

        public V4MainCollection()
        {
            DataList = new List<V4Data>();
        }

        /// LINQ
        /// public float GetMaxAbsFieldValue {get;}
        /// public IEnumerable<DataItem> GetDecreasingValues {get;}
        /// public IEnumerable<Vec2> GetArrayAndListDifferenceValues {get;}

        public bool Contains(string id)
        {
            foreach (var x in DataList)
            {
                if (x.ObjectType == id)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Add(V4Data v4Data)
        {
            if (Contains(v4Data.ObjectType))
            {
                return false;
            }
            DataList.Add(v4Data);
            return true;
        }

        public V4Data this[int index] {get => DataList[index];}

        public string ToLongString(string format)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(Count * 100);
            foreach (var x in DataList)
            {
                sb.Append($"{x.ToLongString(format)}\n");
            }
            return sb.ToString();
        }

        public override string ToString() => String.Join("\n", DataList);
    }
}