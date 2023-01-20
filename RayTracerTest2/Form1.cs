using System.Diagnostics;
using System.Drawing.Imaging;
using System.Numerics;

namespace RayTracerTest2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            double aspectRatio = 16.0 / 9.0;
            Width = 1200;
            Height = (int)(Width / aspectRatio);

            this.Text = "Rendering Programm";
        }
        
        /// <summary>
        /// Generates a random scene with random spheres
        /// </summary>
        /// <returns></returns>
        HittableList RandomScene()
        {
            HittableList world = new();
            Random rnd = new();

            Materials ground = new Lambertian(new Vector3(0.5f, 0.5f, 0.5f));
            world.Add(new Sphere(new Vector3(0f, -1000f, 0f), 1000, ground));

            for (int a = -11; a < 11; a++)
            {
                for (int b = -11; b < 11; b++)
                {
                    float chooseMaterial = rnd.NextSingle();
                    Vector3 center = new Vector3(a + 0.9f * rnd.NextSingle(), 0.2f, b + 0.9f * rnd.NextSingle());
                    if ((center - new Vector3(4f, 0.2f, 0f)).Length() > 0.9f)
                    {
                        Materials sphereMaterial;

                        if (chooseMaterial < 0.8f)
                        {
                            //Diffuse
                            Vector3 color = Mathematics.Random() * Mathematics.Random();
                            sphereMaterial = new Lambertian(color);
                            world.Add(new Sphere(center, 0.2f, sphereMaterial));
                        }
                        else if (chooseMaterial < 0.95f)
                        {
                            //Metal
                            Vector3 color = Mathematics.Random(0.5f, 1f);
                            float fuzz = Mathematics.RandomFloat(0f, 0.5f);
                            sphereMaterial = new Metal(color, fuzz);
                            world.Add(new Sphere(center, 0.2f, sphereMaterial));
                        }
                        else
                        {
                            //Glass
                            sphereMaterial = new Dielectric(1.5f);
                            world.Add(new Sphere(center, 0.2f, sphereMaterial));
                        }
                    }
                }
            }

            Materials material1 = new Dielectric(1.5f);
            world.Add(new Sphere(new Vector3(0f, 1f, 0f), 1f, material1));

            Materials material2 = new Lambertian(new Vector3(0.4f, 0.2f, 0.1f));
            world.Add(new Sphere(new Vector3(-4f, 1f, 0f), 1f, material2));

            Materials material3 = new Metal(new Vector3(0.7f, 0.6f, 0.5f), 0f);
            world.Add(new Sphere(new Vector3(4f, 1f, 0f), 1f, material3));

            return world;
        }

        /// <summary>
        ///     Renders the image
        /// </summary>
        /// <returns>Bitmap</returns>
        unsafe Bitmap Render()
        {
            // IMPORTANT VAR'S
            float samplesPerPixel = 1; // The higher the Samples per Pixel, the higher the render time

            Random rnd = new();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            // Image
            const float aspectRatio = 16.0f / 9.0f;
            const int imageWidth = 1200;
            const int imageHeight = (int)(imageWidth / aspectRatio);

            // World
            float radiant = (float)Math.Cos(Math.PI / 4f);
            HittableList world = RandomScene();
            
            // Camera
            float viewportHeight = 2.0f;
            float viewportWidth = aspectRatio * viewportHeight;
            float focalLength = 1.0f;

            Vector3 origin = new Vector3(0, 0, 0);
            Vector3 horizontal = new Vector3(viewportWidth, 0, 0);
            Vector3 vertical = new Vector3(0, viewportHeight, 0);
            Vector3 lowerLeftCorner = origin - horizontal / 2 - vertical / 2 - new Vector3(0, 0, focalLength);

            // Camera
            Vector3 lookFrom = new Vector3(13f, 2f, 3f);
            Vector3 lookAt = Vector3.Zero;
            Vector3 vup = new Vector3(0f, 1f, 0f);
            float distToFocus = 10; //(lookFrom - lookAt).Length();
            float aperture = 0.1f;

            Camera cam = new(20f, lookFrom, lookAt, vup, aperture, distToFocus);
            
            byte bytesPerPixel = 24;
            int maxDepth = 50;
            Bitmap img = new(imageWidth, imageHeight, PixelFormat.Format24bppRgb);

            BitmapData imgData = img.LockBits(
                new Rectangle(0, 0, imageWidth, imageHeight),
                ImageLockMode.WriteOnly,
                img.PixelFormat);

            int count = 0;
            // Scan0 => determine/calls the first address of the bitmap
            byte* imgPointer = (byte*)imgData.Scan0.ToPointer();

            for (int x = 0; x < imageWidth; x++)
            {
                for (int y = 0; y < imageHeight; y++)
                {
                    // Image Stride expl. Germ. =>
                    // Die Stride ist die Breite einer einzelnen Pixelzeile (eine Scanlinie),
                    // die auf eine Vier-Byte-Grenze gerundet ist.
                    // Wenn der Schritt positiv ist, ist die Bitmap oben unten.
                    // Wenn der Schritt negativ ist, ist die Bitmap unten nach oben.
                    byte* data = imgPointer + y * imgData.Stride + x * bytesPerPixel / 8;

                    Vector3 color = Vector3.Zero;
                    for (int s = 0; s < samplesPerPixel; s++)
                    {
                        float u = (x + (float)rnd.NextDouble()) / (imageWidth - 1);
                        float v = (y + (float)rnd.NextDouble()) / (imageHeight - 1);
                        Ray r = cam.GetRay(u, v);
                        color += RayColor(r, world, maxDepth);
                    }

                    // Divide the color by the number of samples and gamma-correction for gamma=2.0
                    float scale = 1.0f / samplesPerPixel;
                    color.X = (float)Math.Sqrt(scale * color.X);
                    color.Y = (float)Math.Sqrt(scale * color.Y);
                    color.Z = (float)Math.Sqrt(scale * color.Z);
                    
                    Mathematics mth = new();
                    color = new Vector3(
                        255f * mth.Clamp(color.X, 0.0f, 0.999f),
                        255f * mth.Clamp(color.Y, 0.0f, 0.999f),
                        255f * mth.Clamp(color.Z, 0.0f, 0.999f)
                    );

                    data[2] = (byte)color.X;
                    data[1] = (byte)color.Y;
                    data[0] = (byte)color.Z;
                    count = x + y;
                }
            }

            
            img.UnlockBits(imgData);
            img.RotateFlip(RotateFlipType.Rotate180FlipX);
            stopwatch.Stop();
#if DEBUG
            Console.WriteLine($"{count} Itterations in Render"); //For Debug
#endif
            return img;
        }

        Vector3 RayColor(Ray r, Hittable world, int depth)
        {
            HitRecord rec = world.Hit(r, 0.0001f, float.MaxValue);
            if (depth <= 0) return Vector3.Zero;

            if (rec.DidHit)
            {
                Scattered scatter = rec.Material.Scatter(r, rec);
                if (scatter.DidScatter)
                {
                    return scatter.Attenuation * RayColor(scatter.ScatteredRay, world, depth - 1);
                }

                return Vector3.Zero;
            }

            Vector3 unitDirection = Vector3.Normalize(r.Direction);
            float t = 0.5f * (unitDirection.Y + 1.0f);
            return (1.0f - t) * new Vector3(1, 1, 1) + t * new Vector3(0.5f, 0.7f, 1.0f);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Bitmap img = Render();

            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(img, new Rectangle(0, 0, Width, Height), 0, 0, img.Width, img.Height,
                GraphicsUnit.Pixel);
        }

        private void Form1_Load(object sender, EventArgs e) { }
        
    }
}


// MATH EXPLANATION:

/*
 * SPHERE (Ray-Sphere Intersection):
 * 
 * Origin of radius:
 * R => x^2+y^2+z^2 = R^2 | (x,y,z)
 * 
 * Inside the sphere:
 * R => x^2+y^2+z^2 < R^2 | (x,y,z)
 *
 * Outside the sphere
 * R => x^2+y^2+z^2 > R^2 | (x,y,z)
 *
 * EXCEPTION:
 * if sphere center is at (C_x, C_y, C_z)
 * => (x-C_x)^2+(y-C_y)^2+(z-C_z)^2 = r^2
 *
 * Vector C=(C_x, C_y, C_z) to the point P=(x,y,z) is (P-C)
 * => (P-C)*(P-C) = (x-C_x)^2+(y-C_y)^2+(z-C_z)^2 = r^2
 * => (P-C)*(P-C) = r^2
 *
 * any Point "P"
 * want to know if ray P(t) = A+tB hits the sphere
 *
 * look for any t if it is true:
 * => (P(t)-C)*(P(t)-C) = r^2   or
 * => (A+tb-C)*(A+tb-C) = r^2
 *
 * rules of algebra applied:
 * => t^2*b*b+2tb*(A-C)+(A-C)*(A-C) = r^2
 *
 * vectors and r in that equation are all constant and known:
 * unknown is "t"
 *
 * t square root part:
 * if positive => 2 solutions
 * if negative => no solution
 * if zero => 1 solution
 *
 * Chapter 6
 * n is unit length of vector /TODO: Write some more of that stuff
 */