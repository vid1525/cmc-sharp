using System;


using Vec2 = System.Numerics.Vector2;
using Collections = System.Collections.Generic;


namespace lab1
{
    struct DataItem
    {
        public Vec2 Coordinates {get; set;}
        public Vec2 Values {get; set;}

        public DataItem(Vec2 coordinates, Vec2 values)
        {
            this.Coordinates = coordinates;
            this.Values = values;
        }

        public string ToLongString(string format) =>
            String.Format(String.Format("<{{0:{0}}}, {{1:{0}}}> <{{2:{0}}}, {{3:{0}}}>", format),
            Coordinates.X, Coordinates.Y, Values.X, Values.Y);

        public override string ToString() => $"{Coordinates} {Values}";
    }
    
    delegate Vec2 Fv2Vector2(Vec2 v2);

    abstract class V4Data
    {
        public string ObjectType {get;}
        public DateTime LastChangeDate {get;}

        public V4Data(string objectType, DateTime date)
        {
            this.ObjectType = objectType;
            this.LastChangeDate = date;
        }

        public override string ToString() => $"type: {ObjectType}; last change date: {LastChangeDate}";
        public abstract int Count {get;}
        public abstract float MaxFromOrigin {get;}
        public abstract string ToLongString(string format);
    }

    class V4DataList : V4Data
    {
        public Collections.List<DataItem> Items {get;}

        public V4DataList(string objectType, DateTime date)
            : base(objectType, date)
        {
            this.Items = new Collections.List<DataItem>();
        }

        public bool Add(DataItem newItem)
        {
            foreach (var x in Items)
            {
                if (newItem.Coordinates == x.Coordinates)
                {
                    return false;
                }
            }
            MaxDistanceFromOrigin = Math.Max(MaxDistanceFromOrigin, newItem.Coordinates.Length());
            Items.Add(newItem);
            return true;
        }

        public int AddDefaults(int nItems, Fv2Vector2 func)
        {
            Random rand = new Random(nItems);
            int result = 0;
            for (int i = 0; i < nItems; ++i)
            {
                Vec2 coordinates = new Vec2((float) rand.NextDouble() * 100, (float) rand.NextDouble() * 100);
                if (Add(new DataItem(coordinates, func(coordinates))))
                {
                    ++result;
                    MaxDistanceFromOrigin = Math.Max(MaxDistanceFromOrigin, coordinates.Length());
                }
            }
            return result;
        }

        public override int Count {get => Items.Count;}
        public override float MaxFromOrigin {get => MaxDistanceFromOrigin;}
        public override string ToString() => $"{base.ToString()}\n{Count}";
        public override string ToLongString(string format)
        {
            System.Text.StringBuilder longFormatString = new System.Text.StringBuilder(String.Join("", ToString(), "\n"), Count * 8);
            foreach (var x in Items)
            {
                longFormatString.Append(String.Format($"{x.ToLongString(format)} {{0:{format}}}\n", x.Values.Length()));
            }
            return longFormatString.ToString();
        }

        private float MaxDistanceFromOrigin;
    }

    class V4DataArray : V4Data
    {
        public int OxCount {get;}
        public int OyCount {get;}
        public Vec2 GridSteps {get;}
        public Vec2[,] Grid {get;}

        public V4DataArray(string objectType, DateTime date)
            : base(objectType, date)
        {
            Grid = new Vec2[0, 0];
        }

        public V4DataArray(string objectType, DateTime date, int xCount, int yCount, Vec2 gridSteps, Fv2Vector2 func)
            : base(objectType, date)
        {
            this.OxCount = xCount;
            this.OyCount = yCount;
            this.GridSteps = gridSteps;
            this.MaxFromOrigin = (new Vec2((xCount - 1) * gridSteps.X, (yCount - 1) * gridSteps.Y)).Length();
            Grid = new Vec2[xCount, yCount];
            for (int x = 0; x < xCount; ++x)
            {
                for (int y = 0; y < yCount; ++y)
                {
                    Grid[x, y] = func(new Vec2(x * gridSteps.X, y * gridSteps.Y));
                }
            }
        }

        public static explicit operator V4DataList(V4DataArray array)
        {
            V4DataList result = new V4DataList(array.ObjectType, array.LastChangeDate);

            for (int x = 0; x < array.OxCount; ++x)
            {
                for (int y = 0; y < array.OyCount; ++y)
                {
                    result.Add(new DataItem(new Vec2(x * array.GridSteps.X, y * array.GridSteps.Y), array.Grid[x, y]));
                }
            }

            return result;
        }

        public override int Count {get => OxCount * OyCount;}
        public override float MaxFromOrigin {get;}
        public override string ToString() => $"{base.ToString()}\n{OxCount} {OyCount} {GridSteps}";
        public override string ToLongString(string format)
        {
            System.Text.StringBuilder longFormatString = new System.Text.StringBuilder(String.Join("", ToString(), "\n"), Count * 10);
            for (int x = 0; x < OxCount; ++x)
            {
                for (int y = 0; y < OyCount; ++y)
                {
                    string curFormat = String.Format("<{{0:{0}}}, {{1:{0}}}> <{{2:{0}}}, {{3:{0}}}> {{4:{0}}}\n", format);
                    longFormatString.Append(String.Format(curFormat, x * GridSteps.X, y * GridSteps.Y, Grid[x, y].X, Grid[x, y].Y, Grid[x, y].Length()));
                }
            }
            return longFormatString.ToString();
        }
    }

    class V4MainCollection
    {
        public int Count {get => DataList.Count;}

        private Collections.List<V4Data> DataList;

        public V4MainCollection()
        {
            DataList = new Collections.List<V4Data>();
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

    static class Fv2Methods
    {
        public static Vec2 LinearFunc(Vec2 v2) => new Vec2(v2.X * 3 + v2.Y * 2, v2.Y / 3);

        public static Vec2 MultiFunc(Vec2 v2) => new Vec2(3 * v2.X * v2.Y, v2.X + 10);
    }

    class Program
    {
        static void Main(string[] args)
        {
            // 1.
            V4DataArray dataArray = new V4DataArray("data array", new DateTime(), 2, 3, new Vec2(0.5f, 0.75f), Fv2Methods.LinearFunc);
            Console.WriteLine(dataArray.ToLongString("f4"));

            V4DataList dataList = (V4DataList) dataArray;
            Console.WriteLine(dataList.ToLongString("f3"));

            Console.WriteLine($"dataArray: count - {dataArray.Count} ; max from origin - {dataArray.MaxFromOrigin}");
            Console.WriteLine($"dataList: count - {dataList.Count} ; max from origin - {dataList.MaxFromOrigin}");
            Console.WriteLine();
        
            // 2.
            V4MainCollection mainCollection = new V4MainCollection();
            V4DataArray dataArray1 = new V4DataArray("1", new DateTime(), 1, 4, new Vec2(0.6f, 0.8f), Fv2Methods.LinearFunc);
            V4DataArray dataArray2 = new V4DataArray("2", new DateTime(), 4, 2, new Vec2(0.5f, 0.75f), Fv2Methods.MultiFunc);
            V4DataList dataList1 = new V4DataList("3", new DateTime());
            V4DataList dataList2 = new V4DataList("4", new DateTime());
            dataList2.AddDefaults(5, Fv2Methods.MultiFunc);
            mainCollection.Add(dataArray1);
            mainCollection.Add(dataArray2);
            mainCollection.Add(dataList1);
            mainCollection.Add(dataList2);
            Console.WriteLine(mainCollection.ToLongString("f2"));

            // 3.
            for (int i = 0; i < mainCollection.Count; ++i)
            {
                Console.WriteLine($"count - {mainCollection[i].Count}, max from origin - {mainCollection[i].MaxFromOrigin}");
            }
        }
    }
}
