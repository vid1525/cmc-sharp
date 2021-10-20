namespace lab2
{
    class Vec2 {
        private System.Numerics.Vector2 values;
        public System.Single X {
            get => values.X;
            set
            {
                values.X = value;
            }
        }
        public System.Single Y {
            get => values.Y;
            set
            {
                values.Y = value;
            }
        }

        public System.Single Length() => values.Length();
        public override string ToString() => values.ToString();

        public Vec2(System.Single x = 0f, System.Single y = 0f)
        {
            values = new System.Numerics.Vector2(x, y);
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(other, null)) return false;

            if (ReferenceEquals(this, other)) return true;

            return values.Equals(((Vec2) other).values);
        }

        public override int GetHashCode() => values.GetHashCode();

        public static bool operator ==(Vec2 x, Vec2 y) => x.values == y.values;

        public static bool operator !=(Vec2 x, Vec2 y) => x.values != y.values;
    }
    delegate Vec2 Fv2Vector2(Vec2 v2);
}