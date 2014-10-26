/*******************************************************************************
Copyright (c) 2013 Ian Joseph Fischer and David Williams

This software is provided 'as-is', without any express or implied
warranty. In no event will the authors be held liable for any damages
arising from the use of this software.

Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it
freely, subject to the following restrictions:

    1. The origin of this software must not be misrepresented; you must not
    claim that you wrote the original software. If you use this software
    in a product, an acknowledgment in the product documentation would be
    appreciated but is not required.

    2. Altered source versions must be plainly marked as such, and must not be
    misrepresented as being the original software.

    3. This notice may not be removed or altered from any source
    distribution. 	
*******************************************************************************/

using UnityEngine;

namespace Voxel.MathUtil
{
    [System.Serializable]
    public struct Vector3i
    {
        public int X, Y, Z;
        public static readonly Vector3i zero = new Vector3i(0, 0, 0);
        public static readonly Vector3i one = new Vector3i(1, 1, 1);
        public static readonly Vector3i forward = new Vector3i(0, 0, 1);
        public static readonly Vector3i back = new Vector3i(0, 0, -1);
        public static readonly Vector3i up = new Vector3i(0, 1, 0);
        public static readonly Vector3i down = new Vector3i(0, -1, 0);
        public static readonly Vector3i left = new Vector3i(-1, 0, 0);
        public static readonly Vector3i right = new Vector3i(1, 0, 0);


        public Vector3i(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }


        public Vector3i(int x, int y)
        {
            X = x;
            Y = y;
            Z = 0;
        }


        public Vector3i(Vector3i a)
        {
            X = a.X;
            Y = a.Y;
            Z = a.Z;
        }


        public Vector3i(Vector3 a)
        {
            X = (int)a.x;
            Y = (int)a.y;
            Z = (int)a.z;
        }


        public int this[int key]
        {
            get
            {
                switch (key)
                {
                    case 0:
                    {
                        return X;
                    }
                    case 1:
                    {
                        return Y;
                    }
                    case 2:
                    {
                        return Z;
                    }
                    default:
                    {
                        Debug.LogError("Invalid Vector3i index value of: " + key);
                        return 0;
                    }
                }
            }
            set
            {
                switch (key)
                {
                    case 0:
                    {
                        X = value;
                        return;
                    }
                    case 1:
                    {
                        Y = value;
                        return;
                    }
                    case 2:
                    {
                        Z = value;
                        return;
                    }
                    default:
                    {
                        Debug.LogError("Invalid Vector3i index value of: " + key);
                        return;
                    }
                }
            }
        }

        public static int Dot(Vector3i a, Vector3i b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public static Vector3i Scale(Vector3i a, Vector3i b)
        {
            return new Vector3i(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static float Distance(Vector3i a, Vector3i b)
        {
            return Mathf.Sqrt(DistanceSquared(a, b));
        }

        public static int DistanceSquared(Vector3i a, Vector3i b)
        {
            int dx = b.X - a.X;
            int dy = b.Y - a.Y;
            int dz = b.Z - a.Z;
            return dx * dx + dy * dy + dz * dz;
        }


        public override int GetHashCode()
        {
            // Microsoft use XOR in their example here: http://msdn.microsoft.com/en-us/library/ms173147.aspx
            // We also use shifting, as the compoents are typically small and this should reduce collisions.
            return X ^ (Y << 8) ^ (Z << 16);
        }

        public override bool Equals(object other)
        {
            if (!(other is Vector3i))
            {
                return false;
            }
            Vector3i vector = (Vector3i)other;
            return X == vector.X &&
                   Y == vector.Y &&
                   Z == vector.Z;
        }

        public override string ToString()
        {
            return string.Format("Vector3i({0} {1} {2})", X, Y, Z);
        }

        public static bool operator ==(Vector3i a, Vector3i b)
        {
            return a.X == b.X &&
                   a.Y == b.Y &&
                   a.Z == b.Z;
        }

        public static bool operator !=(Vector3i a, Vector3i b)
        {
            return a.X != b.X ||
                   a.Y != b.Y ||
                   a.Z != b.Z;
        }

        public static Vector3i operator -(Vector3i a, Vector3i b)
        {
            return new Vector3i(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector3i operator +(Vector3i a, Vector3i b)
        {
            return new Vector3i(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3i operator *(Vector3i a, int d)
        {
            return new Vector3i(a.X * d, a.Y * d, a.Z * d);
        }

        public static Vector3i operator *(int d, Vector3i a)
        {
            return new Vector3i(a.X * d, a.Y * d, a.Z * d);
        }

        public static implicit operator Vector3(Vector3i v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static explicit operator Vector3i(Vector3 v)
        {
            return new Vector3i(v);
        }

        public static Vector3i Min(Vector3i lhs, Vector3i rhs)
        {
            return new Vector3i(Mathf.Min(lhs.X, rhs.X), Mathf.Min(lhs.Y, rhs.Y), Mathf.Min(lhs.Z, rhs.Z));
        }

        public static Vector3i Max(Vector3i lhs, Vector3i rhs)
        {
            return new Vector3i(Mathf.Max(lhs.X, rhs.X), Mathf.Max(lhs.Y, rhs.Y), Mathf.Max(lhs.Z, rhs.Z));
        }

        public static Vector3i Clamp(Vector3i value, Vector3i min, Vector3i max)
        {
            return new Vector3i(Mathf.Clamp(value.X, min.X, max.X), Mathf.Clamp(value.Y, min.Y, max.Y), Mathf.Clamp(value.Z, min.Z, max.Z));
        }
    }
}

