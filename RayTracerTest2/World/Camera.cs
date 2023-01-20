using System.Numerics;

namespace RayTracerTest2;

public class Camera
{
    public readonly float AspectRatio;
    public readonly float ViewportHeight;
    public readonly float ViewportWidth;


    private readonly Vector3 _origin;
    private readonly Vector3 _lowerLeftCorner;
    private readonly Vector3 _horizontal;
    private readonly Vector3 _vertical;
    private readonly Vector3 _u, _w, _v;
    private readonly float _lenseRadius;
    
    public Camera(float verticalFov, Vector3 lookFrom, Vector3 lookAt, Vector3 vup, float aperture, float focusDistance)
    {
        float theta = Mathematics.DegreesToRadiant(verticalFov);
        float h = (float)Math.Tan(theta / 2f);
        AspectRatio = 16f / 9f;
        ViewportHeight = 2.0f*h;
        ViewportWidth = AspectRatio * ViewportHeight;

        _w = Vector3.Normalize(lookFrom - lookAt);
        _u = Vector3.Normalize(Vector3.Cross(vup, _w));
        _v = Vector3.Cross(_w, _u);

        _origin = lookFrom;
        _horizontal = focusDistance * ViewportWidth * _u;
        _vertical = focusDistance * ViewportHeight * _v;
        _lowerLeftCorner = _origin - _horizontal / 2 - _vertical / 2 - focusDistance * _w;

        _lenseRadius = aperture / 2;
    }

    public Ray GetRay(float u, float v)
    {
        Vector3 rd = _lenseRadius * Mathematics.RandomInUnitDisk();
        Vector3 offset = _u * rd.X + _v * rd.Y;

        return new Ray(_origin + offset, _lowerLeftCorner + u * _horizontal + v * _vertical - _origin - offset);
    }
}