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
    
    public Camera()
    {
        AspectRatio = 16f / 9f;
        ViewportHeight = 2;
        ViewportWidth = AspectRatio * ViewportHeight;

        _origin = Vector3.Zero;
        _horizontal = new Vector3(ViewportWidth, 0f, 0f);
        _vertical = new Vector3(0f, ViewportHeight, 0f);
        _lowerLeftCorner = _origin - _horizontal / 2 - _vertical / 2 - new Vector3(0, 0, FocalLength);
    }

    public Ray GetRay(float u, float v)
    {
        return new Ray(_origin, _lowerLeftCorner + u * _horizontal + v * _vertical - _origin);
    }
}