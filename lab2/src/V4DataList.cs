using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;


namespace lab2
{
    class V4DataList : V4Data
    {
        public List<DataItem> Items {get;}

        public V4DataList(string objectType, DateTime date)
            : base(objectType, date)
        {
            this.Items = new List<DataItem>();
        }

        public override IEnumerator<DataItem> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public static bool SaveAsText(string filename, V4DataList v4)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    writer.WriteLine(v4.ObjectType);
                    writer.WriteLine(v4.LastChangeDate.ToString(DATA_FORMAT));
                    writer.Write(JsonSerializer.Serialize<List<DataItem>>(v4.Items));
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool LoadAsText(string filename, ref V4DataList v4)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    string objType = reader.ReadLine();
                    DateTime date = DateTime.ParseExact(reader.ReadLine(), DATA_FORMAT, System.Globalization.CultureInfo.InvariantCulture);
                    List<DataItem> items = JsonSerializer.Deserialize<List<DataItem>>(reader.ReadToEnd());
                    v4 = new V4DataList(objType, date);
                    foreach (var x in items)
                    {
                        v4.Add(x);
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
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

        private float MaxDistanceFromOrigin{get; set;}
    }
}