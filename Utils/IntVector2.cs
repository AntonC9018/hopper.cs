using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hopper.Utils.Vector
{
    public struct IntVector2
    {
        public int x, y;

        public static IntVector2 Zero = new IntVector2(0, 0);
        public static IntVector2 Right = new IntVector2(1, 0);
        public static IntVector2 Left = new IntVector2(-1, 0);
        public static IntVector2 Up = new IntVector2(0, -1);
        public static IntVector2 Down = new IntVector2(0, 1);

        public static IntVector2 UnitX
        {
            get => new IntVector2(1, 0);
        }

        public static IntVector2 UnitY
        {
            get => new IntVector2(0, 1);
        }

        public IEnumerable<IntVector2> CircleAround
        {
            get
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        yield return new IntVector2(this.x + x, this.y + y);
                    }
                }
            }
        }

        public IEnumerable<IntVector2> OrthogonallyAdjacent
        {
            get
            {
                yield return new IntVector2(this.x + 1, this.y);
                yield return new IntVector2(this.x, this.y - 1);
                yield return new IntVector2(this.x - 1, this.y);
                yield return new IntVector2(this.x, this.y + 1);
            }
        }

        public IntVector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static IntVector2 operator +(IntVector2 a, IntVector2 b)
        {
            return new IntVector2
            {
                x = a.x + b.x,
                y = a.y + b.y
            };
        }

        public static IntVector2 operator -(IntVector2 a, IntVector2 b)
        {
            return new IntVector2
            {
                x = a.x - b.x,
                y = a.y - b.y
            };
        }

        public static IntVector2 operator -(IntVector2 a)
        {
            return new IntVector2
            {
                x = -a.x,
                y = -a.y
            };
        }

        public static IntVector2 operator *(IntVector2 a, int c)
        {
            return new IntVector2
            {
                x = a.x * c,
                y = a.y * c
            };
        }

        public static IntVector2 operator /(IntVector2 a, int c)
        {
            return new IntVector2
            {
                x = a.x / c,
                y = a.y / c
            };
        }

        public IntVector2 Copy()
        {
            return new IntVector2
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

            return x == ((IntVector2)obj).x && y == ((IntVector2)obj).y;
        }

        public override int GetHashCode()
        {
            return (int)x ^ (int)y;
        }

        public int Cross(IntVector2 v)
        {
            return x * v.y - y * v.x;
        }


        public int Dot(IntVector2 v)
        {
            return x * v.x + y * v.y;
        }

        public IntVector2 MatMul(IntVector2 i_hat, IntVector2 j_hat)
        {
            return new IntVector2
            {
                x = i_hat.x * x + j_hat.x * y,
                y = i_hat.y * x + j_hat.y * y
            };
        }

        [JsonIgnore] public int SqMag => x * x + y * y;

        [JsonIgnore] public float Mag => (float)Math.Sqrt(SqMag);

        public double AngleTo(IntVector2 v)
        {
            return Math.Atan2(Cross(v), Dot(v));
        }

        public IntVector2 Rotate(double angle_in_rads)
        {
            return MatMul(
                new IntVector2
                {
                    x = (int)Math.Cos(angle_in_rads),
                    y = (int)Math.Sin(angle_in_rads)
                },
                new IntVector2
                {
                    x = -(int)Math.Sin(angle_in_rads),
                    y = (int)Math.Cos(angle_in_rads)
                }
            );
        }

        public IntVector2 RotateAndRound(double angle_in_rads)
        {
            return MatMul(
                new IntVector2
                {
                    x = (int)Math.Round(Math.Cos(angle_in_rads)),
                    y = (int)Math.Round(Math.Sin(angle_in_rads))
                },
                new IntVector2
                {
                    x = -(int)Math.Round(Math.Sin(angle_in_rads)),
                    y = (int)Math.Round(Math.Cos(angle_in_rads))
                }
            );
        }

        public IntVector2 RotateHalfPi()
        {
            return MatMul(
                new IntVector2
                {
                    x = 0,
                    y = 1
                },
                new IntVector2
                {
                    x = -1,
                    y = 0
                }
            );
        }

        public IntVector2 Sign()
        {
            return new IntVector2
            {
                x = x == 0 ? 0 : (x > 0 ? 1 : -1),
                y = y == 0 ? 0 : (y > 0 ? 1 : -1)
            };
        }

        public override string ToString()
        {
            return $"<{x}, {y}>";
        }

        public static implicit operator Vector2(IntVector2 v)
            => new Vector2(v.x, v.y);

        public IntVector2 HadamardProduct(IntVector2 vector2)
        {
            return new IntVector2
            {
                x = x * vector2.x,
                y = y * vector2.y
            };
        }

        public int ComponentSum()
        {
            return x + y;
        }

        public IntVector2 Abs()
        {
            return new IntVector2
            {
                x = x > 0 ? x : -x,
                y = y > 0 ? y : -y
            };
        }

        public static bool operator ==(IntVector2 rhs, IntVector2 lhs)
        {
            return rhs.Equals(lhs);
        }

        public static bool operator !=(IntVector2 rhs, IntVector2 lhs)
        {
            return !(rhs == lhs);
        }
    }
}