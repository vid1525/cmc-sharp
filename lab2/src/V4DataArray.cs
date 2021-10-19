using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using Vec2 = System.Numerics.Vector2;


namespace lab2
{
    class V4DataArray : V4Data
    {
        private const string DATA_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";

        public int OxCount {get; protected set;}
        public int OyCount {get; protected set;}
        public Vec2 GridSteps {get; protected set;}
        public Vec2[,] Grid {get; protected set;}

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

        class V4DataArrayEnumerator : IEnumerator<DataItem>
        {
            private V4DataArray DataArray;
            private int PositionX;
            private int PositionY;

            public V4DataArrayEnumerator(V4DataArray dataArray)
            {
                DataArray = dataArray;
                PositionX = 0;
                PositionY = -1;
            }
            public DataItem Current
            {
                get
                {
                    if (PositionY < 0 || DataArray.OyCount <= PositionY || PositionX < 0 || DataArray.OxCount <= PositionX)
                    {
                        throw new InvalidOperationException();
                    }
                    return new DataItem(new Vec2(PositionX * DataArray.GridSteps.X, PositionY * DataArray.GridSteps.Y), DataArray.Grid[PositionX, PositionY]);
                }
            }

            object IEnumerator.Current => throw new NotImplementedException();

            public bool MoveNext()
            {
                PositionY++;
                if (PositionY < DataArray.OyCount) {
                    return true;
                }
                PositionX++;
                if (PositionX < DataArray.OxCount) {
                    PositionY = 0;
                    return true;
                }
                return false;
            }
            public void Reset()
            {
                PositionY = -1;
                PositionX = 0;
            }

            public void Dispose() {}
        }

        public override IEnumerator<DataItem> GetEnumerator()
        {
            return new V4DataArrayEnumerator(this);
        }

        static public bool SaveBinary(string filename, V4DataArray v4)
        {
            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.OpenOrCreate)))
                {
                    writer.Write(v4.ObjectType);
                    writer.Write(v4.LastChangeDate.ToString(V4DataArray.DATA_FORMAT));
                    writer.Write(v4.OxCount);
                    writer.Write(v4.OyCount);
                    writer.Write((double) v4.GridSteps.X);
                    writer.Write((double) v4.GridSteps.Y);
                    for (int x = 0; x < v4.OxCount; ++x)
                    {
                        for (int y = 0; y < v4.OyCount; ++y)
                        {
                            writer.Write(x);
                            writer.Write(y);
                            writer.Write((double) v4.Grid[x, y].X);
                            writer.Write((double) v4.Grid[x, y].Y);
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        static public bool LoadBinary(string filename, ref V4DataArray v4)
        {
            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open)))
                {
                    string objectType = reader.ReadString();
                    DateTime date = DateTime.ParseExact(reader.ReadString(), V4DataArray.DATA_FORMAT, System.Globalization.CultureInfo.InvariantCulture);
                    int xCount = reader.ReadInt32();
                    int yCount = reader.ReadInt32();
                    Vec2 gridSteps = new Vec2();
                    gridSteps.X = (float) reader.ReadDouble();
                    gridSteps.Y = (float) reader.ReadDouble();

                    v4 = new V4DataArray(objectType, date, xCount, yCount, gridSteps, (x) => x);

                    for (int i = 0; i < xCount; ++i)
                    {
                        for (int j = 0; j < yCount; ++j)
                        {
                            int x = reader.ReadInt32();
                            int y = reader.ReadInt32();
                            Vec2 values = new Vec2();
                            values.X = (float) reader.ReadDouble();
                            values.Y = (float) reader.ReadDouble();
                            v4.Grid[x, y] = values;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
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
}