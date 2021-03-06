using System;

namespace Hopper.Utils.Vector
{
    public struct Vector2
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

        public static Vector2 operator /(Vector2 a, float c)
        {
            return new Vector2
            {
                x = a.x / c,
                y = a.y / c
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
                    y = (float)Math.Sin(angle_in_rads)
                },
                new Vector2
                {
                    x = -(float)Math.Sin(angle_in_rads),
                    y = (float)Math.Cos(angle_in_rads)
                }
            );
        }

        public override string ToString()
        {
            return $"<{x}, {y}>";
        }

        public IntVector2 Round()
        {
            return new IntVector2
            (
                x : (int)Math.Round(x),
                y : (int)Math.Round(y)
            );
        }

        public static explicit operator IntVector2(Vector2 v)
            => new IntVector2((int)v.x, (int)v.y);

        public float ComponentSum()
        {
            return x + y;
        }

        public bool IsWhole()
        {
            return Math.Floor(x) == x && Math.Floor(y) == y;
        }

        public Vector2 Abs()
        {
            return new Vector2
            {
                x = x > 0 ? x : -x,
                y = y > 0 ? y : -y
            };
        }

        public static Vector2 Lerp(Vector2 start, Vector2 end, float proportion)
        {
            return (end - start) * proportion + start;
        }

        public static bool operator ==(Vector2 rhs, Vector2 lhs)
        {
            return rhs.Equals(lhs);
        }

        public static bool operator !=(Vector2 rhs, Vector2 lhs)
        {
            return !(rhs == lhs);
        }
    }
}