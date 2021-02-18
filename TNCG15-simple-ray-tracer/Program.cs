using System;
using System.Diagnostics;
using GlmNet;

using COLOR_DOUBLE = GlmNet.vec3;
using UTIL_ = TNCG15_simple_ray_tracer.Utilities;
using CONST_ = TNCG15_simple_ray_tracer.Constants;

namespace TNCG15_simple_ray_tracer
{
    class Program
    {
        static void Main(string[] args)
        {
            Scene scene = new Scene();
            
            // Define: Positions, colors, and surface types for the desired objects.
            var tetraPos = new vec3(-3.0f, -4.0f, 9.0f);
            var spherePos1 = new vec3(3.0f, -3.0f, 7.0f);
            var spherePos2 = new vec3(-1.25f,-4f,10.6f);
            var spherePos4 = new vec3(2f,-4f,9f);
            var mirrorColor = new vec3(0.0f);
            var color1 = new vec3(1f,1f,1f);
            var color2 = new vec3(0f,0f,1f);
            var surface1 = new Surface(mirrorColor, CONST_.SPECULAR);
            var surface2 = new Surface(color1, CONST_.LAMBERTIAN);
            var surface3 = new Surface(color2, CONST_.LAMBERTIAN);
            
            // Create: Objects and Lights
            Sphere mirrorBall = new Sphere(spherePos1, 1.0f, surface1);
            Sphere whiteBall = new Sphere(spherePos2, 0.75f, surface2);
            Sphere blueBall = new Sphere(spherePos4, 0.75f, surface3);
            Box myTetra = new Box(tetraPos, 2.0f);
            Light areaLight = new Light();
            
            // Assemble: Add objects and light source to scene.
            scene.AddSphere(mirrorBall);
            scene.AddSphere(whiteBall);
            scene.AddSphere(blueBall);
            scene.AddLight(areaLight);
            scene.AddBox(myTetra);

            Camera camera =  new Camera();
            //camera.SwitchCamera();
            
            // Set the desired field of view.
            camera.Fov = (float)Math.PI / 1.5f;
            // Set the desired samples-per-pixel & sub-pixels.
            camera.Spp = 10;
            camera.SubPixels = 3;
            
            var stopWatch = Stopwatch.StartNew();  
            // ...Let's render a ray-traced image of the scene!
            camera.CreateImage(scene, "rayTracedImage.ppm");
            Console.WriteLine("Execution time = {0} seconds\n", stopWatch.Elapsed.TotalSeconds);  
            stopWatch.Stop();
        }
    }
}