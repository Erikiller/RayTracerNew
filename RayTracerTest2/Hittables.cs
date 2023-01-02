using System.Numerics;
using System.Runtime.CompilerServices;

namespace RayTracerTest2;


abstract class Sphere : Hittable
{
    public Vector3 center;
    public float radius;

    public Sphere()
    {
    }

    public Sphere(Vector3 cen, float r)
    {
        center = cen;
        radius = r;
    }

    public override HitRecord hit(Ray r, float tMin, float tMax, HitRecord rec)
    {
        Vector3 oc = r.Origin - center;
        float a = r.Direction.LengthSquared();
        float halfB = Vector3.Dot(oc, r.Direction);
        float c = oc.LengthSquared() - radius * radius;

        float discriminant = halfB * halfB - a * c;
        if (discriminant < 0) return false;

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

        rec.t = root;
        rec.p = r.At(rec.t);
        Vector3 outwardNormal = (rec.p - center) / radius;
        rec.setFaceToNormal(r, outwardNormal);

        return true;
    }
}

public abstract class Hittable
{
    public abstract HitRecord hit(Ray r, float t_min, float t_max, HitRecord rec);
}
public struct HitRecord
{
    public Vector3 p;
    public Vector3 normal;
    public float t;
    public bool FrontFace;
    public bool didHit;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void setFaceToNormal(Ray r, Vector3 outwardNormal)
    {
        FrontFace = Vector3.Dot(r.Direction, outwardNormal) < 0;
        normal = FrontFace ? outwardNormal : -outwardNormal;
    }
}

public class HittableList : Hittable
{
    public List<Hittable> objects = new();

    HittableList()
    {
    }

    public HittableList(Hittable _object)
    {
        add(_object);
    }

    public void clear()
    {
        objects.Clear();
    }

    public void add(Hittable h)
    {
        objects.Add(h);
    }

    public override HitRecord hit(Ray r, double t_min, double t_max)
    {
        HitRecord rec = new HitRecord();
        rec.didHit = false;
        double closest = t_max;

        foreach (Hittable o in objects)
        {
            HitRecord hit = o.hit(r, t_min, closest);
            if (hit.didHit)
            {
                closest = hit.t;
                rec = hit;
            }
        }

        return rec;
    }
}