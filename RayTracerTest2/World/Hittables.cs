using System.Numerics;
using System.Runtime.CompilerServices;

namespace RayTracerTest2;

public class Sphere : Hittable
{
    public Vector3 Center;
    public readonly float Radius;
    public readonly Materials Material;

    /// <summary>
    /// Constructor for creating a sphere
    /// </summary>
    /// <param name="cen"></param>
    /// <param name="r"></param>
    /// <param name="mat"></param>
    public Sphere(Vector3 cen, float r, Materials mat)
    {
        Center = cen;
        Radius = r;
        Material = mat;
    }

    public override HitRecord Hit(Ray r, float tMin, float tMax)
    {
        Vector3 oc = r.Origin - Center;
        float a = r.Direction.LengthSquared();
        float halfB = Vector3.Dot(oc, r.Direction);
        float c = oc.LengthSquared() - Radius * Radius;

        float discriminant = halfB * halfB - a * c;
        if (discriminant < 0)
        {
            return new HitRecord();
#if DEBUG
            Debug.Log("new HitRecord got triggered!");
#endif
        }

        float sqrtd = (float)Math.Sqrt(discriminant);

        // Find the nearest root that lies in acceptable range
        float root = (-halfB - sqrtd) / a;
        if (root < tMin || tMax < root)
        {
            root = (-halfB + sqrtd) / a;
            if (root < tMin || root < tMax)
            {
                return new HitRecord();
            }
        }

        HitRecord rec = new();
        rec.T = root;
        rec.P = r.At(rec.T);
        Vector3 outwardNormal = (rec.P - Center) / Radius;
        rec.SetFaceToNormal(r, outwardNormal);
        rec.DidHit = true;
        rec.Material = Material;

        return rec;
    }
}

public abstract class Hittable
{
    public abstract HitRecord Hit(Ray r, float tMin, float tMax);
}

public class HitRecord
{
    public Vector3 P { get; set; }
    public Vector3 Normal { get; private set; }
    public float T { get; set; }
    public bool FrontFace { get; private set; }
    public bool DidHit { get; set; }
    public Materials Material { get; set; }

    public HitRecord(Vector3 p, Vector3 normal, float t, bool frontFace, bool didHit, Materials material)
    {
        P = p;
        Normal = normal;
        T = t;
        FrontFace = frontFace;
        DidHit = didHit;
        Material = material;
    }

    /// <summary>
    /// Set <see cref="DidHit"/> to false
    /// </summary>
    public HitRecord()
    {
        DidHit = false;
    }

    /// <summary>
    /// Determine the direction of the ray hit
    /// </summary>
    /// <param name="r"></param>
    /// <param name="outwardNormal"></param>
    public void SetFaceToNormal(Ray r, Vector3 outwardNormal)
    {
        FrontFace = Vector3.Dot(r.Direction, outwardNormal) < 0;
        Normal = FrontFace ? outwardNormal : -outwardNormal;
    }
}

public class HittableList : Hittable
{
    public readonly List<Hittable> Objects = new();

    public HittableList()
    {
    }

    /// <summary>
    /// Creates new object in the list of hittables 
    /// </summary>
    /// <param name="object"></param>
    public HittableList(Hittable @object)
    {
        Add(@object);
    }

    /// <summary>
    /// Clears the clears the list of hittables
    /// </summary>
    public void Clear()
    {
        Objects.Clear();
    }

    /// <summary>
    /// Creates a new object in the list of hittables
    /// </summary>
    /// <param name="h"></param>
    public void Add(Hittable h)
    {
        Objects.Add(h);
    }

    public override HitRecord Hit(Ray r, float tMin, float tMax)
    {
        HitRecord rec = new HitRecord();
        rec.DidHit = false;
        float closest = tMax;

        foreach (Hittable h in Objects)
        {
            HitRecord hit = h.Hit(r, tMin, closest);
            if (hit.DidHit)
            {
                closest = hit.T;
                rec = hit;
            }
        }

        return rec;
    }
}