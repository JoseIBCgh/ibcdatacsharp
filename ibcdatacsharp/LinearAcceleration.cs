//#define struct
#define class
using System;
using System.Diagnostics;
using System.Linq;

namespace ibcdatacsharp
{
#if struct
    public struct Quaternion
    {
        public Vector v;
        public float w;
        public Quaternion(float w, float x, float y, float z)
        {
            this.v = new Vector(x, y, z);
            this.w = w;
        }
        public Quaternion(Vector v, float w)
        {
            this.v = v;
            this.w = w;
        }
        public Quaternion(Vector v)
        {
            this.v = v;
            this.w = 0;
        }
        public static bool operator ==(Quaternion q1, Quaternion q2) => q1.v == q2.v && q1.w == q2.w;
        public static bool operator !=(Quaternion q1, Quaternion q2) => q1.v != q2.v || q1.w != q2.w;
        public override string ToString() => w.ToString() + " + " + v.ToString();
    }
    public struct Vector
    {
        public float x;
        public float y;
        public float z;
        public Vector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public static Vector operator /(Vector v, float f) => new Vector(v.x / f, v.y / f, v.z / f);
        public static Vector operator -(Vector v) => new Vector(-v.x, -v.y, -v.z);
        public static Vector operator -(Vector v1, Vector v2) => new Vector(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        public static Vector operator +(Vector v1, Vector v2) => new Vector(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        public static Vector operator *(float f, Vector v) => new Vector(f * v.x, f * v.y, f * v.z);
        public static bool operator ==(Vector v1, Vector v2) => v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
        public static bool operator !=(Vector v1, Vector v2) => v1.x != v2.x || v1.y != v2.y || v1.z != v2.z;
        public override string ToString() => x.ToString() + " + " +  y.ToString() + " + " +  z.ToString();
    }
    public class LinearAcceleration
    {
        private Vector cross(Vector v1, Vector v2)
        {
            return new Vector(v1.y * v2.z - v1.z * v2.y, v1.z * v2.x - v1.x * v2.z, v1.x * v2.y - v1.y * v2.x);
        }
        private float dot(Vector v1, Vector v2)
        {
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }
        private float dot(Quaternion q1, Quaternion q2)
        {
            return dot(q1.v, q2.v) + q1.w * q2.w;
        }
        private Quaternion normalizeQuaternion(Quaternion q)
        {
            float m = (float)Math.Sqrt(dot(q, q));
            return new Quaternion(q.v/m, q.w/m);
        }
        private Quaternion quaternionMult(Quaternion q1, Quaternion q2)
        {
            return new Quaternion(cross(q1.v, q2.v) + q1.w * q2.v + q2.w * q1.v, q1.w * q2.w - dot(q1.v, q2.v));
        }
        private Quaternion quaternionConjugate(Quaternion q)
        {
            return new Quaternion(-q.v, q.w);
        }
        private Vector quaternionRotateVector(Quaternion q, Vector v)
        {
            Quaternion qv = quaternionMult(quaternionConjugate(q), new Quaternion(v));
            Quaternion qvq = quaternionMult(qv, q);
            return qvq.v;
        }
        private Vector vectorSubtraction(Vector v1, Vector v2)
        {
            return v1 - v2;
        }
        public Vector calcLinAcc(Quaternion q, Vector acc)
        {
            Vector g = new(0 , 0, -1);
            Vector gRot = quaternionRotateVector(q, g);
            return gRot - acc;
        }
#endif
#if class
    public class QuatLinear
    {
        public qVector v;
        public float w;
        public QuatLinear(float w, float x, float y, float z)
        {
            this.v = new qVector(x, y, z);
            this.w = w;
        }
        public QuatLinear(qVector v, float w)
        {
            this.v = v;
            this.w = w;
        }
        public QuatLinear(qVector v)
        {
            this.v = v;
            this.w = 0;
        }
        public static bool operator ==(QuatLinear q1, QuatLinear q2) => q1.v == q2.v && q1.w == q2.w;
        public static bool operator !=(QuatLinear q1, QuatLinear q2) => q1.v != q2.v || q1.w != q2.w;
        public override string ToString() => w.ToString() + " + " + v.ToString();
    }
    public class qVector
    {
        public float x;
        public float y;
        public float z;
        public qVector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public static qVector operator /(qVector v, float f) => new qVector(v.x / f, v.y / f, v.z / f);
        public static qVector operator -(qVector v) => new qVector(-v.x, -v.y, -v.z);
        public static qVector operator -(qVector v1, qVector v2) => new qVector(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        public static qVector operator +(qVector v1, qVector v2) => new qVector(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        public static qVector operator *(float f, qVector v) => new qVector(f * v.x, f * v.y, f * v.z);
        public static bool operator ==(qVector v1, qVector v2) => v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
        public static bool operator !=(qVector v1, qVector v2) => v1.x != v2.x || v1.y != v2.y || v1.z != v2.z;
        public override string ToString() => x.ToString() + " + " + y.ToString() + " + " + z.ToString();
    }
    public class LinearAcceleration
    {
        private qVector cross(qVector v1, qVector v2)
        {
            return new qVector(v1.y * v2.z - v1.z * v2.y, v1.z * v2.x - v1.x * v2.z, v1.x * v2.y - v1.y * v2.x);
        }
        private float dot(qVector v1, qVector v2)
        {
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }
        private float dot(QuatLinear q1, QuatLinear q2)
        {
            return dot(q1.v, q2.v) + q1.w * q2.w;
        }
        private QuatLinear normalizeQuaternion(QuatLinear q)
        {
            float m = (float)Math.Sqrt(dot(q, q));
            return new QuatLinear(q.v / m, q.w / m);
        }
        private QuatLinear quaternionMult(QuatLinear q1, QuatLinear q2)
        {
            return new QuatLinear(cross(q1.v, q2.v) + q1.w * q2.v + q2.w * q1.v, q1.w * q2.w - dot(q1.v, q2.v));
        }
        private QuatLinear quaternionConjugate(QuatLinear q)
        {
            return new QuatLinear(-q.v, q.w);
        }
        private qVector quaternionRotateVector(QuatLinear q, qVector v)
        {
            QuatLinear qv = quaternionMult(quaternionConjugate(q), new QuatLinear(v));
            QuatLinear qvq = quaternionMult(qv, q);
            return qvq.v;
        }
        private qVector vectorSubtraction(qVector v1, qVector v2)
        {
            return v1 - v2;
        }
        public qVector calcLinAcc(QuatLinear q, qVector acc)
        {
            qVector g = new(0, 0, -1);
            qVector gRot = quaternionRotateVector(q, g);
            return gRot - acc;
        }
#endif
        private const int NUM_TESTS = 10;
        private const int NUM_OPERATIONS = 1000000;
        public void testQuaternionMult()
        {
            QuatLinear q1 = new QuatLinear(1.3f, 3.5f, 4.3f, 2.1f);
            QuatLinear q2 = new QuatLinear(2.4f, 5.7f, 8.1f, 6.8f);
            QuatLinear r = quaternionMult(q1, q2);
            QuatLinear tr = new QuatLinear(-65.94f, 28.039999999999996f, 9.019999999999998f, 17.72f);
            Trace.WriteLine("teoric " + tr);
            Trace.WriteLine("calculated " + r);
        }
        public void testSpeed()
        {
            qVector getRandomVector(Random random)
            {
                return new qVector(random.NextSingle(), random.NextSingle(), random.NextSingle());
            }
            QuatLinear getRandomQuaternion(Random random)
            {
                return new QuatLinear(getRandomVector(random), random.NextSingle());
            }
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Random random = new Random();
            float[] times = new float[NUM_TESTS];
            for (int j = 0; j < NUM_TESTS; j++)
            {
                for (int i = 0; i < NUM_OPERATIONS; i++)
                {
                    QuatLinear q = getRandomQuaternion(random);
                    qVector v = getRandomVector(random);
                    qVector r = calcLinAcc(q, v);
                }
                times[j] = stopwatch.ElapsedMilliseconds;
                stopwatch.Restart();
            }
            Trace.Write('[');
            foreach (float time in times)
            {
                Trace.Write(time);
                Trace.Write(", ");
            }
            Trace.WriteLine("]");
            Trace.WriteLine(times.Sum() / times.Length);
        }
    }
}
