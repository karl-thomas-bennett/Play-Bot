using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.scripts
{
    class Vector
    {
        public float x, y, z;

        public Vector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector(Vector v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }
        public Vector(System.Numerics.Vector3 v)
        {
            x = v.X;
            y = v.Y;
            z = v.Z;
        }

        public System.Numerics.Vector3 ToVector3()
        {
            return new System.Numerics.Vector3((float)x, (float)y, (float)z);
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.x - b.x, a.y - b.y, a.z - b.z);
        }
        public static Vector operator -(Vector b)
        {
            return new Vector(-b.x, -b.y, -b.z);
        }
        public static Vector operator *(Vector v, float n)
        {
            return new Vector(v.x * n, v.y * n, v.z * n);
        }
        public static Vector operator /(Vector v, float n)
        {
            return new Vector(v.x / n, v.y / n, v.z / n);
        }

        public float Mag()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }

        public float Dot(Vector other)
        {
            return x * other.x + y * other.y + z * other.z;
        }

        public Vector Cross(Vector other)
        {
            return new Vector(y * other.z - z * other.y, z * other.x - z * other.z, x * other.y - y * other.x);
        }

        public float AngleTo(Vector other)
        {
            return (float)Math.Acos(Dot(other) / (Mag() * other.Mag()));
        }

        public float SignedAngleTo(Vector other)
        {
            float angle = (float)(Math.Atan2(other.y, other.x) - Math.Atan2(y, x));
            float PI = (float)Math.PI;
            if (angle > Math.PI)
            {
                angle -= 2 * PI;
            }
            else if(angle <= -PI)
            {
                angle += 2 * PI;
            }
            return angle;
        }

        public float AngleOfDirection()
        {
            Vector forward = new Vector(0, 1, 0);
            return (float)Math.Atan2(-x, Dot(forward));
        }

        public Vector Normalise()
        {
            return this / Mag();
        }

        public Vector Flatten()
        {
            return new Vector(x, y, 0);
        }

        public Vector SetLength(float length)
        {
            return Normalise() * length;
        }
    }
}
