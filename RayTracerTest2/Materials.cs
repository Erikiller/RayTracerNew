using System.Numerics;

namespace RayTracerTest2;
/*
public abstract class Materials
{
    public Vector3 Albedo { get; set; }

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
*/

public abstract class Materials
{
    public abstract Scattered Scatter(Ray r, HitRecord rec);
}
public struct Scattered
{
    public Vector3 Attenuation { get; set; }
    public Ray ScatteredRay { get; set; }
    public bool DidScatter { get; set; }
    public Scattered(Vector3 _attenuation, Ray _scatteredRay, bool _didScatter)
    {
        Attenuation = _attenuation;
        ScatteredRay = _scatteredRay;
        DidScatter = _didScatter;
    }
}

public class Lambertian : Materials
{
    public Vector3 Color;

    Lambertian(Vector3 color)
    {
        Color = color;
    }
    public override Scattered Scatter(Ray r, HitRecord rec)
    {
        Scattered scattered = new();
        Vector3 scatterDir = rec.normal + Mathematics.RandomUnitVector(); // Could possibly be a bug
        
        if (Mathematics.NearZero(scatterDir))
            scatterDir = rec.normal;
        
        scattered.ScatteredRay = new Ray(rec.p, scatterDir);
        scattered.Attenuation = Color;
        scattered.DidScatter = true;
        return scattered;
    }
}
/*

public abstract class Metal : Materials
{
    
    protected Metal(Vector3 a) { Albedo = a; }

    public override bool Scatter(Ray rIn, HitRecord rec, Vector3 _attenuation, Ray _scattered)
    {
        Vector3 reflected = Vector3.Reflect(Vector3.Normalize(rIn.Direction), rec.normal);
        _scattered = new Ray(rec.p, reflected);
        _attenuation = Albedo;
        return (Vector3.Dot(_scattered.Direction, rec.normal) > 0);
    }
}
*/