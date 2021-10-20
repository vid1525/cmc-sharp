using System;
using System.Linq;
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

        public float GetMaxAbsFieldValue
        {
            get
            {
                try
                {
                    return DataList.Where(
                        x => x.GetType().ToString() == "lab2.V4DataArray" && x.Count > 0
                    ).Select(x => (from tmp in x select tmp.Values.Length()).Max()).Max();
                }
                catch
                {
                    return float.NaN;
                }
            }
        }

        public IEnumerable<DataItem> GetValuesInDecreasingOrderByDistance
        {
            get
            {
                try
                {
                    return DataList.Select(x => (from tmp in x select tmp)).Aggregate((x, y) => x.Concat(y)).OrderBy(x => -x.Coordinates.Length());
                }
                catch
                {
                    return null;
                }
            }
        }
        public IEnumerable<Vec2> GetArrayAndListDifferenceCoordinates
        {
            get
            {
                var dataArrayCoord = DataList.Where(x => x.GetType().ToString() == "lab2.V4DataArray").Select(
                    x => (from tmp in x select tmp.Coordinates)
                ).Aggregate((x, y) => x.Concat(y)).Distinct();
                var dataListCoord = DataList.Where(x => x.GetType().ToString() == "lab2.V4DataList").Select(
                    x => (from tmp in x select tmp.Coordinates)
                ).Aggregate((x, y) => x.Concat(y));
                return dataArrayCoord.Except(dataListCoord);
            }
        }

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