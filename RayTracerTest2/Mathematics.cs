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

    public Vector3 RandomInUnitSphere()
    {
        while (true)
        {
            Vector3 p = Random(-1, 1);
            if (p.LengthSquared() >=1) continue;
            return p;
        }
    }
}