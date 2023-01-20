using System.Numerics;
namespace RayTracerTest2;

public abstract class Materials
{
    public abstract Scattered Scatter(Ray r, HitRecord rec);
}
public struct Scattered
{
    public Vector3 Attenuation { get; set; }
    public Ray ScatteredRay { get; set; }
    public bool DidScatter { get; set; }
    
    /// <summary>
    /// Set value for <see cref="Attenuation"/>, <see cref="ScatteredRay"/> and <see cref="DidScatter"/>
    /// </summary>
    /// <param name="attenuation"></param>
    /// <param name="scatteredRay"></param>
    /// <param name="didScatter"></param>
    public Scattered(Vector3 attenuation, Ray scatteredRay, bool didScatter)
    {
        Attenuation = attenuation;
        ScatteredRay = scatteredRay;
        DidScatter = didScatter;
    }
}

public class Lambertian : Materials
{
    public Vector3 Color;

    public Lambertian(Vector3 color)
    {
        Color = color;
    }
    public override Scattered Scatter(Ray r, HitRecord rec)
    {
        Scattered scattered = new();
        Vector3 scatterDir = rec.Normal + Mathematics.RandomUnitVector();
        
        if (Mathematics.NearZero(scatterDir))
            scatterDir = rec.Normal;
        
        scattered.ScatteredRay = new Ray(rec.P, scatterDir);
        scattered.Attenuation = Color;
        scattered.DidScatter = true;
        return scattered;
    }
}

public class Metal : Materials
{
    public Vector3 Color;
    public float Fuzz;

    public Metal(Vector3 color, float f)
    {
        Color = color;
        Fuzz = f < 1 ? f : 1;
    }
    public override Scattered Scatter(Ray r, HitRecord rec)
    {
        Scattered scattered = new();
        Vector3 reflected = Vector3.Reflect(Vector3.Normalize(r.Direction), rec.Normal);
        scattered.ScatteredRay = new Ray(rec.P, reflected + Fuzz * Mathematics.RandomInUnitSphere());
        scattered.Attenuation = Vector3.One;
        scattered.DidScatter = Vector3.Dot(scattered.ScatteredRay.Direction, rec.Normal) > 0f;
        
        return scattered;
    }
}

public class Dielectric : Materials
{
    public float Ir; //Index of reflection

    public Dielectric(float indexOfReflection)
    {
        Ir = indexOfReflection;
    }
    public override Scattered Scatter(Ray r, HitRecord rec)
    {
        Scattered scatter = new();
        scatter.Attenuation = Vector3.One;
        float refractionRatio = rec.FrontFace ? (1f / Ir) : Ir;

        Vector3 unitDirection = Vector3.Normalize(r.Direction);
        float cosTheta = Math.Min(Vector3.Dot(-unitDirection, rec.Normal),1f);
        float sinTheta = (float)Math.Sqrt(1f - cosTheta * cosTheta);

        bool cannotRefract = refractionRatio * sinTheta > 1f;
        Vector3 direction;

        if (cannotRefract)
            direction = Mathematics.Reflect(unitDirection, rec.Normal);
        else
            direction = Mathematics.Refract(unitDirection, rec.Normal, refractionRatio);

        scatter.ScatteredRay = new Ray(rec.P, direction);
            
        //Vector3 refracted = Mathematics.Refract(unitDirection, rec.normal, refractionRatio);
        //scatter.ScatteredRay = new Ray(rec.p, refracted);
        scatter.DidScatter = true;
        return scatter;
    }

    private static float Reflactance(float cosine, float refIdx)
    {
        // Use Schlik's approximation for reflectance
        float r0 = (1 - refIdx) / (1f + refIdx);
        r0 = r0 * r0;
        return r0 + (1 - r0) * (float)Math.Pow((1f - cosine), 5f);
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