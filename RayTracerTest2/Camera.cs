using System.Numerics;

namespace RayTracerTest2;

public class Camera
{
    public readonly float AspectRatio;
    public readonly float ViewportHeight;
    public readonly float ViewportWidth;
    public float FocalLength;


    private readonly Vector3 _origin;
    private readonly Vector3 _lowerLeftCorner;
    private readonly Vector3 _horizontal;
    private readonly Vector3 _vertical;
    
    public Camera(float verticalFov, Vector3 lookFrom, Vector3 lookAt, Vector3 vup)
    {
        float theta = Mathematics.DegreesToRadiant(verticalFov);
        float h = (float)Math.Tan(theta / 2f);
        AspectRatio = 16f / 9f;
        ViewportHeight = 2.0f*h;
        ViewportWidth = AspectRatio * ViewportHeight;
        FocalLength = 1f;

        Vector3 w = Vector3.Normalize(lookFrom - lookAt);
        Vector3 u = Vector3.Normalize(Vector3.Cross(vup, w));
        Vector3 v = Vector3.Cross(w, u);

        _origin = lookFrom;
        _horizontal = ViewportWidth * u;
        _vertical = ViewportHeight * v;
        _lowerLeftCorner = _origin - _horizontal / 2 - _vertical / 2 - w;
    }

    public Ray GetRay(float u, float v)
    {
        return new Ray(_origin, _lowerLeftCorner + u * _horizontal + v * _vertical - _origin);
    }
}