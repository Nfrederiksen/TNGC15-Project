using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GlmNet;
using COLOR_DOUBLE = GlmNet.vec3;
using UTIL_ = TNCG15_simple_ray_tracer.Utilities;
using CONST_ = TNCG15_simple_ray_tracer.Constants;

namespace TNCG15_simple_ray_tracer
{
    public class Camera
    {
        // Fields
        // Key: Where camera is positioned.
        // Value: Camera's looking direction.
        private readonly KeyValuePair<vec3, vec3> _cameraPos1 = new KeyValuePair<vec3, vec3>(
            new vec3(0.0f),
            new vec3(0.0f, 0.0f, 1.0f));
        private readonly KeyValuePair<vec3, vec3> _cameraPos2 = new KeyValuePair<vec3, vec3>(
            new vec3(1.0f, 2.5f, 2.5f),
            new vec3(-0.5f, -0.6f, 1.0f));
        // --2D width x height Array of Pixels.
        private readonly Pixel[,] _pixelArray = new Pixel[CONST_.WIDTH,CONST_.HEIGHT];
        // --FOV in Radians.
        private float _fov = ((float) Math.PI) / 1.5f;
        // --A boolean to flip between cameras.
        private bool _CamBeingUsed;
        // --Sample Per Pixel.
        private int _spp;
        
        // --Sub-pixels
        private int _subPixels = 3;
        
        // Properties
        public float Fov
        {
            get => _fov;
            set => _fov = value;
        }

        public int Spp
        {
            get => _spp;
            set => _spp = value;
        }

        public int SubPixels
        {
            get => _subPixels;
            set => _subPixels = value;
        }

        public KeyValuePair<vec3, vec3> GetCamera()
        {
            return _CamBeingUsed ? _cameraPos1 : _cameraPos2;
        }
            
        // Constructor
        public Camera()
        {
            _CamBeingUsed = true;
        }
        // Methods
        public void SwitchCamera()
        {
            if (_CamBeingUsed)
            {
                _CamBeingUsed = false;
            }
            else
            {
                _CamBeingUsed = true;
            }
        }
        
        public void CreateImage(Scene scene, string filename)
        {
            Console.Write(@"
.-----------------------.
|      RENDER TIME!     |
'-----------------------'
");
            Console.WriteLine("Creating a {0}x{1} image...", CONST_.WIDTH, CONST_.HEIGHT);
            Console.WriteLine("SPP: {0}, Sub-Pixels: {1}, Using Camera: {2}", _spp, _subPixels, 1);
            // 1) Get rays going through pixels.
            CreatePixels();
            // 2) Get color for pixel based on the rays path. AND max intensity value for a RGB channel.
            var maxIntensity = CastRays(scene);
            // 3) Put pixel values in file 
            WriteToFile(filename, maxIntensity);
            Console.WriteLine("DONE!");
        }

        private void CreatePixels()
        {
            for (int h = 0; h < CONST_.HEIGHT; h++)
            {
                for (int w = 0; w < CONST_.WIDTH; w++)
                {
                    var pixel = new Pixel();
                    var diff = 1.0f / _subPixels;
                    for (int subX = 0; subX < _subPixels; ++subX)
                    {
                        for (int subY = 0; subY < _subPixels; ++subY)
                        {
                            var x = UTIL_.RandMinMax(w + subX * diff, w + (subX + 1) * diff);
                            var y = UTIL_.RandMinMax(h + subY * diff, h + (subY + 1) * diff);
                            Ray ray = GetRayFromPixelCoords(x, y);
                            pixel.AddRay(ray);
                        }
                    }
                    _pixelArray[h, w] = pixel;
                }
            }
        }

        public Ray GetRayFromPixelCoords(double w, double h)
        {
            var c = GetCamera();
            const double aspectRatio = (double)CONST_.HEIGHT / (double)CONST_.WIDTH;
            var pw = w / CONST_.WIDTH;
            var ph = h / CONST_.HEIGHT;
            var fovH = _fov * aspectRatio;
            var radW = pw * _fov - _fov / 2;
            var radH = ph * fovH - fovH / 2;
            var diffW = -1 * glm.sin((float)radW);
            var diffH = -1 * glm.sin((float) radH);
            var diff = new vec3(diffW,diffH,0.0f);
            var lookAt = glm.normalize(c.Value + diff);
            var ray = new Ray(c.Key, lookAt);
            
            return ray;
        }

        public double CastRays(Scene scene)
        {
            double maximum = 0.0;
            Console.WriteLine("Casting rays onto the scene...");
            var count = 0;
            var percent = 10.0f;
            var total = CONST_.WIDTH * CONST_.HEIGHT;

            // SOME THREADING TO BOOST RENDER TIMES.
            Parallel.For(0, _pixelArray.GetLength(0), i =>
            {
             for (int j = 0; j < _pixelArray.GetLength(1); j++)
             {
                    var progressBar = 100.0f * (float) count / total;
                    // Printing the PRogress.
                    if ((int)Math.Floor(progressBar / percent) == 1)
                    {
                        Console.WriteLine((int)progressBar + "%");
                        percent += 10.0f;
                    }
                    var color = new vec3(0);
                    var rays = _pixelArray[i, j].RayList;
                    foreach (var ray in rays)
                    {
                       
                        for(int k = 0; k < _spp; k++)
                        {    // Colors the pixel w/ each sample.
                            color += CastRay(scene, ray, 0);
                        }
                    }
                    color /= _spp * _subPixels * _subPixels;
                    _pixelArray[i, j].ColorDouble = color;
                    maximum = Math.Max(maximum, Math.Max(color.x, Math.Max(color.y, color.z)));
                    count += 1; // This is for the progress bar.
             }
            });
            
            return maximum;
        }
        
        /// <summary>
        /// Recursive function that collects the total color emitted from a contact point by
        /// traveling with a bouncing ray collecting color from other points along the way.
        /// </summary>
        /// <param name="mScene">Scene containing all triangles and parametric spheres</param>
        /// <param name="mRay">Ray traveling towards eventual intersection point.</param>
        /// <param name="mDepth">Ray bouncing limit.</param>
        /// <returns>The color a ray brings to a surface point.</returns>
        public COLOR_DOUBLE CastRay(Scene mScene, Ray mRay, int mDepth)
        {
            // Fetch all intersecting triangles/spheres in scene.
            // + single out the ones that intersect with mRay's trajectory.
            var tIntersections = mScene.DetectTriangleIntersections(mRay);
            var sIntersections = mScene.DetectSphereIntersections(mRay);
            var color = new vec3(0f); // Default color for intersection is pitch black.
            float distToSphere;
            float distToTriangle;

            if (sIntersections.Count > 0)
            {
                distToSphere = (float)UTIL_.Distance(sIntersections.First().Point, mRay.Start);
            }
            else
            {
                distToSphere = float.PositiveInfinity;
            }

            if (tIntersections.Count > 0)
            {
                distToTriangle = (float)UTIL_.Distance(tIntersections.First().Point, mRay.Start);
            }
            else
            {
                distToTriangle = float.PositiveInfinity;
            }

            // --( CASE: Ray intersects w/ a triangle before intersects w/ a sphere )--
            //     TRUE: Bounce off the [triangle] and form new ray path. Give value to 'color'.
            //    FALSE: Bounce off the [sphere] and form new ray path. Give value to 'color'.
            if (distToTriangle < distToSphere)
            {
                // List is sorted. First() should be closest intersection point for ray.
                // No need to care about the second or third intersection points, as they
                // are getting occluded from the first one.
                var tI = tIntersections.First();
                var triangle = tI.Triangle;
                var surface = triangle.Surface;
                var pointNormal = triangle.Normal;
            
                // --[ Base Case #1 ]-- Triangle in question is a light source triangle.
                if (surface.HasReflectionModel(CONST_.LIGHTSOURCE))
                {
                    // We hit light. Color value for this point is bright white.
                    return color = surface.GetColor();
                }
                // --( UPDATE: color )--
                // Add surface point's emitted color according to:
                // 1) Reflection model.
                // 2) Outgoing reflection incident angle.
                // 3) Direct light contribution (influenced by occlusion & incident angle).
                var outRay = surface.BounceRay(mRay, tI.Point, pointNormal);
                var theta = UTIL_.Angle(outRay.Direction, pointNormal);
                var lightContribution = mScene.GetLightContribution(tI.Point, pointNormal);
                var emittedColor = surface.Reflect() * glm.cos(theta) * lightContribution;
                color += emittedColor;

                // --( BOUNCE: Does the ray path stop or not? )--
                var threshold = Math.Max(Math.Max(emittedColor.x, emittedColor.y), emittedColor.z);
                if (mDepth < CONST_.MAX_DEPTH || UTIL_.UniformRand() < threshold)
                {
                    // Increment the depth-value, unless we're bouncing on a specular surface (mirrors).
                    var nextDepth = surface.HasReflectionModel(CONST_.SPECULAR) ? mDepth : mDepth + 1;
                    // Recursion: Until we hit the 'max_depth' or randomly get below 'threshold'.
                    color += CastRay(mScene, outRay, nextDepth) * surface.ReflectionCoefficient;
                }
                // --( TEMPORARY FIX: Points that are not in direct light get color from indir.light )--
                // Uncomment to get lighter images.
                color += surface.Reflect() * glm.cos(theta)
                                            * new vec3(0.5f); // Last term used to experiment. Can be removed.
            }
            else
            {    // --[ Base Case #2 ]-- Ray never intersected with any spheres or triangles.
                if (sIntersections.Count == 0)
                {    
                     // Return empty color (black).
                    return color;
                }

                var sI = sIntersections.First();
                var sphere = sI.Sphere;
                var surface = sphere.Surface;
                vec3 pointNormal = sphere.CalcNormal(sI.Point);
                
                var outRay = surface.BounceRay(mRay, sI.Point, pointNormal);
                var theta = UTIL_.Angle(outRay.Direction, pointNormal);
                var lightContribution = mScene.GetLightContribution(sI.Point, pointNormal);
                var emittedColor = surface.Reflect() * glm.cos(theta) * lightContribution;
                color += emittedColor;
                // HOT FIX: Spheres seem to get too much light contribution when close to the light source.
                //color *= new vec3(0.5f);
                
                var threshold = Math.Max(Math.Max(emittedColor.x, emittedColor.y), emittedColor.z);
                if (mDepth < CONST_.MAX_DEPTH || UTIL_.UniformRand() < threshold)
                {
                    var nextDepth = surface.HasReflectionModel(CONST_.SPECULAR) ? mDepth : mDepth + 1;
                    color += CastRay(mScene, outRay, nextDepth) * surface.ReflectionCoefficient;
                }
                // --( TEMPORARY FIX: Points that are not in direct light get color from indir.light )--
                // Uncomment to get lighter images.
                //color += surface.Reflect() * glm.cos(theta)
                //                         * new vec3(0.5f); // Last term for experiment. Can be removed.
            }

            return color;
        }
        
        public void WriteToFile(string filename, double max)
        {
            Console.WriteLine("\n\nWriting image... (Max Intensity:'{0}')", max);
            //Use Streamwriter to write the text part of the encoding
            var writer = new StreamWriter(filename);
            writer.WriteLine("P3");
            writer.WriteLine($"{CONST_.WIDTH}  {CONST_.HEIGHT}");
            writer.WriteLine("255");

            for (var x = 0; x < CONST_.WIDTH; x++)
            {
                for (var y = 0; y < CONST_.HEIGHT; y++)
                {
                    var color = _pixelArray[x, y].ColorDouble;
                    
                    /*                //THIS NEEDS WORK...
                    // --[Extra Image Processing]-- Set the contrast if needed.
                    int contrast = 128;
                    var factor = (255 * (contrast + 255)) / (255 * (255 - contrast));
                    var newR = (factor * (color.x*255 - 128) + 128);
                    var newG = (factor * (color.y*255 - 128) + 128);
                    var newB = (factor * (color.z*255 - 128) + 128);
                        
                    color = new vec3(UTIL_.Truncate(newR/255f), UTIL_.Truncate(newG/255), UTIL_.Truncate(newB/255));
                    */
                    
                    writer.Write((int)(255 * (color.x / max)));
                    writer.Write(" ");
                    writer.Write((int)(255 * (color.y / max)));
                    writer.Write(" ");
                    writer.Write((int)(255 * (color.z / max)));
                    writer.Write(" ");
                }
            }
            writer.Close();
            Console.WriteLine("Wrote image to '{0}'...", filename);
        }
    }
}