using System;
using System.Collections.Generic;

namespace Hopper.Utils.Vector
{
    public readonly struct IntVector2 : IEquatable<IntVector2>
    {
        public readonly int x;
        public readonly int y;

        public static readonly IntVector2 Zero = new IntVector2(0, 0);
        public static readonly IntVector2 Right = new IntVector2(1, 0);
        public static readonly IntVector2 Left = new IntVector2(-1, 0);
        public static readonly IntVector2 Up = new IntVector2(0, -1);
        public static readonly IntVector2 Down = new IntVector2(0, 1);

        public static readonly IntVector2 UnitX = new IntVector2(1, 0);
        public static readonly IntVector2 UnitY = new IntVector2(0, 1);

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
        
        public static IEnumerable<IntVector2> OrthogonallyAdjacentToOrigin
        {
            get
            {
                yield return new IntVector2( 1,  0);
                yield return new IntVector2( 0, -1);
                yield return new IntVector2(-1,  0);
                yield return new IntVector2( 0,  1);
            }
        }

        public static IEnumerable<IntVector2> Spiral(int start_x, int start_y, int end_x, int end_y)
        {
            while (start_x <= end_x && start_y <= end_y)
            {
                // the first row from the remaining rows
                for (int i = start_y; i <= end_y; ++i)
                {
                    yield return new IntVector2(start_x, i);
                }
                start_x++;

                // the last column from the remaining columns
                for (int i = start_x; i <= end_x; ++i)
                {
                    yield return new IntVector2(i, end_y);
                }
                end_y--;

                // the last row from the remaining rows
                if (start_x <= end_x)
                {
                    for (int i = end_y; i >= start_y; --i)
                    {
                        yield return new IntVector2(end_x, i);
                    }
                    end_x--;
                }

                // the first column from the remaining columns
                if (start_y <= end_y)
                {
                    for (int i = end_x; i >= start_x; --i)
                    {
                        yield return new IntVector2(i, start_y);
                    }
                    start_y++;
                }
            }
        }

        public IntVector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static IntVector2 operator +(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.x + b.x, a.y + b.y);
        }

        public static IntVector2 operator -(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.x - b.x, a.y - b.y);
        }

        public static IntVector2 operator -(IntVector2 a)
        {
            return new IntVector2(-a.x, -a.y);
        }

        public static IntVector2 operator *(IntVector2 a, int c)
        {
            return new IntVector2(a.x * c, a.y * c);
        }

        public static IntVector2 operator /(IntVector2 a, int c)
        {
            return new IntVector2(a.x / c, a.y /c);
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
            return x ^ (y >> 16);
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
            return new IntVector2(
                i_hat.x * x + j_hat.x * y,
                i_hat.y * x + j_hat.y * y
            );
        }

        public int SqMag => x * x + y * y;

        public float Mag => (float)Math.Sqrt(SqMag);

        public double AngleTo(IntVector2 v)
        {
            return Math.Atan2(Cross(v), Dot(v));
        }

        public IntVector2 Rotate(double angle_in_rads)
        {
            return MatMul(
                new IntVector2
                (
                    x : (int)Math.Cos(angle_in_rads),
                    y : (int)Math.Sin(angle_in_rads)
                ),
                new IntVector2
                (
                    x : -(int)Math.Sin(angle_in_rads),
                    y : (int)Math.Cos(angle_in_rads)
                )
            );
        }

        public IntVector2 RotateAndRound(double angle_in_rads)
        {
            return MatMul(
                new IntVector2
                (
                    x : (int)Math.Round(Math.Cos(angle_in_rads)),
                    y : (int)Math.Round(Math.Sin(angle_in_rads))
                ),
                new IntVector2
                (
                    x : -(int)Math.Round(Math.Sin(angle_in_rads)),
                    y : (int)Math.Round(Math.Cos(angle_in_rads))
                )
            );
        }

        public IntVector2 RotateHalfPi()
        {
            return MatMul(
                new IntVector2(0, 1),
                new IntVector2(-1, 0)
            );
        }

        /// <summary>
        /// Returns a new vector, where each component is the sign of the corresponding component.
        /// E.g. (0, 5) -> (0, 1);  (-1, -9) -> (-1, -1)
        /// </summary>
        public IntVector2 Sign()
        {
            return new IntVector2
            (
                x : x == 0 ? 0 : (x > 0 ? 1 : -1),
                y : y == 0 ? 0 : (y > 0 ? 1 : -1)
            );
        }

        public override string ToString()
        {
            return $"<{x}, {y}>";
        }

        public static implicit operator Vector2(IntVector2 v)
            => new Vector2(v.x, v.y);

        public IntVector2 HadamardProduct(IntVector2 vector2)
        {
            return new IntVector2(x * vector2.x, y * vector2.y);
        }

        public int ComponentSum()
        {
            return x + y;
        }

        public IntVector2 Abs()
        {
            return new IntVector2
            (
                x : x > 0 ? x : -x,
                y : y > 0 ? y : -y
            );
        }

        public bool Equals(IntVector2 other)
        {
            return x == other.x && y == other.y;
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