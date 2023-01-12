using System.Numerics;
namespace RayTracerTest2;

public class Mathematics
{
    private static readonly Random rnd = new();
    public float Clamp(float x, float min, float max)
    {
        if (x < min) return min;
        if (x > max) return max;
        return x;
    }

    public static float RandomFloat(float min, float max)
    {
        return min + (max - min) * rnd.NextSingle();
    }
    public static Vector3 Random()
    {
        return new Vector3(rnd.NextSingle(), rnd.NextSingle(), rnd.NextSingle());
    }

    public static Vector3 Random(float min, float max)
    {
        return new Vector3(RandomFloat(min, max), RandomFloat(min, max), RandomFloat(min, max));
    }

    public static Vector3 RandomInUnitSphere()
    {
        while (true)
        {
            Vector3 p = Random(-1, 1);
            if (p.LengthSquared() >=1) continue;
            return p;
        }
    }

    public static Vector3 RandomUnitVector()
    {
        return Vector3.Normalize(RandomInUnitSphere());
    }

    public static float DegreesToRadiant(float angle)
    {
        return (float)(Math.PI / 180) * angle;
    }

    public static Vector3 RandomInHeimisphere(Vector3 normal)
    {
        Vector3 inUnitSphere = RandomInUnitSphere();
        if (Vector3.Dot(inUnitSphere, normal) > 0f)
        {
            return inUnitSphere;
        }

        return -inUnitSphere;
    }
    /// <summary>
    /// Returns true if the Vector is near to Zero in all dimensions
    /// </summary>
    /// <returns></returns>
    public static bool NearZero(Vector3 vec)
    {
        double s = 1e-8;
        return ((Math.Abs(vec.X) < s) && (Math.Abs(vec.Y) < s) && (Math.Abs(vec.Z) < s));
    }

    public static Vector3 Reflect(Vector3 v, Vector3 n)
    {
        return v - 2 * Vector3.Dot(v, n) * n;
    }

    public static Vector3 Refract(Vector3 uv, Vector3 n, float etaiOverEtat)
    {
        float cosTheta = Math.Min(Vector3.Dot(-uv, n), 1f);
        Vector3 rOutPerp = etaiOverEtat * (uv + cosTheta * n);
        Vector3 rOutParallel = (float)-Math.Sqrt(Math.Abs(1f - rOutPerp.LengthSquared())) * n;
        return rOutPerp + rOutParallel;
    }

    public static Vector3 RandomInUnitDisk()
    {
        while (true)
        {
            Vector3 p = new Vector3(RandomFloat(-1f, 1f), RandomFloat(-1f, 1f), 0f);
            if (p.LengthSquared() >=1) continue;
            return p;
        }
    }
}