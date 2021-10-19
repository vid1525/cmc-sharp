using System;

using Vec2 = System.Numerics.Vector2;


namespace lab2
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
}