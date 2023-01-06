using System.Numerics;
using System.Runtime.CompilerServices;

namespace RayTracerTest2;


public class Sphere : Hittable
{
    public Vector3 center;
    public float radius;
    public Materials Material;

    public Sphere() { }

    public Sphere(Vector3 cen, float r, Materials mat)
    {
        center = cen;
        radius = r;
        Material = mat;
    }

    public override HitRecord hit(Ray r, float tMin, float tMax)
    {
        Vector3 oc = r.Origin - center;
        float a = r.Direction.LengthSquared();
        float halfB = Vector3.Dot(oc, r.Direction);
        float c = oc.LengthSquared() - radius * radius;

        float discriminant = halfB * halfB - a * c;
        if (discriminant < 0) return new HitRecord();

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
        rec.t = root;
        rec.p = r.At(rec.t);
        Vector3 outwardNormal = (rec.p - center) / radius;
        rec.setFaceToNormal(r, outwardNormal);
        rec.didHit = true;
        rec.Material = Material;
        
        return rec;
    }
}

public abstract class Hittable
{
    public abstract HitRecord hit(Ray r, float t_min, float t_max);
}
public class HitRecord
{
    public Vector3 p { get; set; }
    public Vector3 normal { get; set; }
    public float t { get; set; }
    public bool FrontFace { get; set; }
    public bool didHit { get; set; }
    public Materials Material { get; set; }

    public HitRecord(Vector3 _p, Vector3 _normal, float _t, bool _frontFace, bool _didHit, Materials _material)
    {
        p = _p;
        normal = _normal;
        t = _t;
        FrontFace = _frontFace;
        didHit = _didHit;
        Material = _material;
    }

    public HitRecord()
    {
        didHit = false;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void setFaceToNormal(Ray r, Vector3 outwardNormal)
    {
        FrontFace = Vector3.Dot(r.Direction, outwardNormal) < 0;
        normal = FrontFace ? outwardNormal : -(outwardNormal);
    }
}

public class HittableList : Hittable
{
    public List<Hittable> objects = new();
    public HittableList() { }

    public HittableList(Hittable _object)
    {
        Add(_object);
    }

    public void Clear()
    {
        objects.Clear();
    }

    public void Add(Hittable h)
    {
        objects.Add(h);
    }

    public override HitRecord hit(Ray r, float t_min, float t_max)
    {
        HitRecord rec = new HitRecord();
        rec.didHit = false;
        float closest = t_max;

        foreach (Hittable h in objects)
        {
            HitRecord hit = h.hit(r, t_min, closest);
            if (hit.didHit)
            {
                closest = hit.t;
                rec = hit;
            }
        }

        return rec;
    }
}