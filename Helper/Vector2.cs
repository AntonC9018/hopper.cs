using System;

namespace Vector
{
    public class Vector2
    {
        public double x, y;

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2
            {
                x = a.x + b.x,
                y = a.y + b.y
            };
        }

        // public static Vector2 operator -(Vector2 a, Vector2 b)
        // {
        //     return new Vector2
        //     {
        //         x = a.x - b.x,
        //         y = a.y - b.y
        //     };
        // }

        public static Vector2 operator -(Vector2 a)
        {
            return new Vector2
            {
                x = -a.x,
                y = -a.y
            };
        }

        public static Vector2 operator *(Vector2 a, double c)
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

        public double Cross(Vector2 v)
        {
            return x * v.y - y * v.x;
        }


        public double Dot(Vector2 v)
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

        public double SqMag()
        {
            return x * x + y * y;
        }

        public double Mag()
        {
            return Math.Sqrt(SqMag());
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
                    x = Math.Cos(angle_in_rads),
                    y = -Math.Sin(angle_in_rads)
                },
                new Vector2
                {
                    x = Math.Sin(angle_in_rads),
                    y = Math.Cos(angle_in_rads)
                }
            );
        }
    }
}