using System.Numerics;

namespace RayTracerTest2;

public abstract class Materials
{
    public Vector3 Albedo;

    public virtual void Lambertian(Vector3 a)
    {
        Albedo = a;
    }

    public virtual bool Scatter(Ray rIn, HitRecord rec, Vector3 attenuation, Ray scattered)
    {
        Vector3 scatterDirection = rec.normal + Mathematics.RandomUnitVector();
        if (Mathematics.NearZero(scatterDirection))
        {
            scatterDirection = rec.normal;
        }
        
        scattered = new Ray(rec.p, scatterDirection);
        attenuation = Albedo;
        return true;
    }
}

public abstract class Metal : Materials
{
    Metal(Vector3 a) { Albedo = a; }

    public override bool Scatter(Ray rIn, HitRecord rec, Vector3 _attenuation, Ray _scattered)
    {
        Vector3 reflected = Vector3.Reflect(Vector3.Normalize(rIn.Direction), rec.normal);
        _scattered = new Ray(rec.p, reflected);
        _attenuation = Albedo;
        return (Vector3.Dot(_scattered.Direction, rec.normal) > 0);
    }
}