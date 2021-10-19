using Vec2 = System.Numerics.Vector2;


namespace lab2
{
    static class Fv2Methods
    {
        public static Vec2 LinearFunc(Vec2 v2) => new Vec2(v2.X * 3 + v2.Y * 2, v2.Y / 3);

        public static Vec2 MultiFunc(Vec2 v2) => new Vec2(3 * v2.X * v2.Y, v2.X + 10);
    }
}