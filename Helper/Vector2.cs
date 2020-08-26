using System;

namespace Vector
{
    public class Vector2
    {
        public float x, y;

        public static Vector2 Right = new Vector2(1, 0);
        public static Vector2 Left = new Vector2(-1, 0);
        public static Vector2 Up = new Vector2(0, 1);
        public static Vector2 Down = new Vector2(0, -1);

        public static Vector2 UnitX
        {
            get => new Vector2(1, 0);
        }

        public static Vector2 UnitY
        {
            get => new Vector2(0, 1);
        }

        public Vector2() { }
        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2
            {
                x = a.x + b.x,
                y = a.y + b.y
            };
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2
            {
                x = a.x - b.x,
                y = a.y - b.y
            };
        }

        public static Vector2 operator -(Vector2 a)
        {
            return new Vector2
            {
                x = -a.x,
                y = -a.y
            };
        }

        public static Vector2 operator *(Vector2 a, float c)
        {
            return new Vector2
            {
                x = a.x * c,
                y = a.y * c
            };
        }

        public Vector2 Copy()
        {
            return new Vector2
            {
                x = x,
                y = y
            };
        }

        public override bool Equals(object obj)
        {

            if (obj == null)
            {
                return false;
            }

            return x == ((Vector2)obj).x && y == ((Vector2)obj).y;
        }

        public override int GetHashCode()
        {
            return (int)x ^ (int)y;
        }

        public float Cross(Vector2 v)
        {
            return x * v.y - y * v.x;
        }


        public float Dot(Vector2 v)
        {
            return x * v.x + y * v.y;
        }

        public Vector2 MatMul(Vector2 i_hat, Vector2 j_hat)
        {
            return new Vector2
            {
                x = i_hat.x * x + j_hat.x * y,
                y = i_hat.y * x + j_hat.y * y
            };
        }

        public float SqMag()
        {
            return x * x + y * y;
        }

        public float Mag()
        {
            return (float)Math.Sqrt(SqMag());
        }

        public double AngleTo(Vector2 v)
        {
            return Math.Atan2(Cross(v), Dot(v));
        }

        public Vector2 Rotate(double angle_in_rads)
        {
            return MatMul(
                new Vector2
                {
                    x = (float)Math.Cos(angle_in_rads),
                    y = (float)-Math.Sin(angle_in_rads)
                },
                new Vector2
                {
                    x = (float)Math.Sin(angle_in_rads),
                    y = (float)Math.Cos(angle_in_rads)
                }
            );
        }

        public override string ToString()
        {
            return $"<{x}, {y}>";
        }
    }
}