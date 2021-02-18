using System;
using System.Collections.Generic;
using System.Linq;
using GlmNet;
using COLOR_DOUBLE = GlmNet.vec3;
using UTIL_ = TNCG15_simple_ray_tracer.Utilities;
using CONST_ = TNCG15_simple_ray_tracer.Constants;

namespace TNCG15_simple_ray_tracer
{
    public struct TriangleIntersection
    {
        public Triangle Triangle;
        public vec3 Point;
    }

    public struct SphereIntersection
    {
        public Sphere Sphere;
        public vec3 Point;
    }
    
    public class Scene
    {
        // Fields
        private readonly List<Triangle> _triangles;
        private readonly List<Sphere> _spheres;
        private readonly List<Light> _lights;

        // Parameter-less Constructor
        public Scene()
        {
            _triangles = new List<Triangle>();
            _spheres = new List<Sphere>();
            _lights = new List<Light>();

            CreateMyRoom();
        }
        
        private void CreateMyRoom()
        {
            // View from above:
            // ^ = camera 1 and viewing direction (origin)
            //           a
            //          / \               <-- front
            // LEFT    c   b     RIGHT
            //         |   |              <-- center
            //         e ^ d
            //          \ /               <-- back
            //           f
            //
            //           ^ z
            //           |
            //    x<-----|
        
            // All vertices in the "Hexagonal" Room.
            vec3 aBottom = new vec3(0.0f, -5.0f, 13.0f);
            vec3 bBottom = new vec3(-6.0f, -5.0f, 10.0f);
            vec3 cBottom = new vec3(6.0f, -5.0f, 10.0f);
            vec3 dBottom = new vec3(-6.0f, -5.0f, 0.0f);
            vec3 eBottom = new vec3(6.0f, -5.0f, 0.0f);
            vec3 fBottom = new vec3(0.0f, -5.0f, -3.0f);
            
            vec3 aTop = new vec3(0.0f, 5.0f, 13.0f);
            vec3 bTop = new vec3(-6.0f, 5.0f, 10.0f);
            vec3 cTop = new vec3(6.0f, 5.0f, 10.0f);
            vec3 dTop = new vec3(-6.0f, 5.0f, 0.0f);
            vec3 eTop = new vec3(6.0f, 5.0f, 0.0f);
            vec3 fTop = new vec3(0.0f, 5.0f, -3.0f);
            
            // Define All Our Surface Colors
            Surface colWhite = new Surface(new COLOR_DOUBLE(1.0f), CONST_.LAMBERTIAN);
            Surface colGrey = new Surface(new COLOR_DOUBLE(0.5f), CONST_.LAMBERTIAN);
            Surface colRed = new Surface(new COLOR_DOUBLE(1.0f,0.0f,0.0f), CONST_.LAMBERTIAN); 
            Surface colGreen = new Surface(new COLOR_DOUBLE(0.0f,1.0f,0.0f), CONST_.LAMBERTIAN);
            Surface colBlue = new Surface(new COLOR_DOUBLE(0.0f,0.0f,1.0f), CONST_.LAMBERTIAN); 
            Surface colYellow = new Surface(new COLOR_DOUBLE(1.0f,1.0f,0.0f), CONST_.LAMBERTIAN); 
            Surface colMagenta = new Surface(new COLOR_DOUBLE(1.0f,0.0f,1.0f), CONST_.LAMBERTIAN); 
            Surface colTeal = new Surface(new COLOR_DOUBLE(0.0f,1.0f,1.0f), CONST_.LAMBERTIAN); 
            Surface colMirror = new Surface(new COLOR_DOUBLE(0.0f,0.0f,0.0f), CONST_.SPECULAR); 
            
            /* Here we assemble all the triangles in the room
              ...floor, walls and such. */
            
            // The Floor
            _triangles.Add(new Triangle(aBottom, cBottom, bBottom, colWhite));
            _triangles.Add(new Triangle(bBottom,cBottom, dBottom, colWhite));
            _triangles.Add(new Triangle(dBottom,cBottom,eBottom, colWhite));
            _triangles.Add(new Triangle(dBottom,eBottom,fBottom, colWhite));
            // The Ceiling
            _triangles.Add(new Triangle(bTop,cTop,aTop, colWhite));
            _triangles.Add(new Triangle(bTop,dTop,cTop, colWhite));
            _triangles.Add(new Triangle(dTop,eTop,cTop, colWhite));
            _triangles.Add(new Triangle(dTop,fTop,eTop, colWhite));
            // Wall[1/6]: Back (Left)
            _triangles.Add(new Triangle(fBottom,eTop,fTop, colBlue));
            _triangles.Add(new Triangle(fBottom,eBottom,eTop, colBlue));
            // Wall[2/6]: Back (Right)
            _triangles.Add(new Triangle(dBottom,fTop,dTop, colRed));
            _triangles.Add(new Triangle(dBottom,fBottom,fTop, colRed));
            // Wall[3/6]: Center (Left)
            _triangles.Add(new Triangle(eBottom,cTop,eTop, colGreen));
            _triangles.Add(new Triangle(eBottom,cBottom,cTop, colGreen));
            // Wall[4/6]: Center (Right)
            _triangles.Add(new Triangle(bBottom,dTop,bTop, colMagenta));
            _triangles.Add(new Triangle(bBottom,dBottom,dTop, colMagenta));
            // Wall[5/6]: Front (Left)
            _triangles.Add(new Triangle(cBottom,aTop,cTop, colMirror));
            _triangles.Add(new Triangle(cBottom,aBottom,aTop, colMirror));
            // Wall[6/6]: Front (Right)
            _triangles.Add(new Triangle(aBottom,bTop,aTop, colYellow));
            _triangles.Add(new Triangle(aBottom,bBottom,bTop, colYellow));
        }
        
        private void AddTriangles(List<Triangle> triangles)
        {
            foreach (var t in triangles)
            {
                _triangles.Add(t);
            }
        }
        public void AddLight(Light light)
        {
            _lights.Add(light);
            AddTriangles(light.Triangles);
        }
        public void AddSphere(Sphere sphere)
        {
            _spheres.Add(sphere);
        }

        public void AddBox(Box b)
        {
            // We don't need to use a 'boxList'.
            // Simply append the box's triangles to triangleList. 
            AddTriangles(b.Triangles);
        }

        /// <summary>
        /// Going through all triangles in the scene, saving only those who intersect with the input ray, 
        /// in a custom list who's items hold info such as; Intersected triangle & 3D-coord of intersection.
        /// </summary>
        /// <param name="ray"> A point of origin and a direction</param>
        /// <returns> a List of triangles the ray has pierced </returns>
        public List<TriangleIntersection> DetectTriangleIntersections(Ray ray)
        {
            var tIntersections = new List<TriangleIntersection>();
            foreach (var triangle in _triangles)
            {
                // Check if a triangle intersects ray path.
                var (results, intersectionPoint) = triangle.Intersection(ray);
                if (results == CONST_.INTERSECTION )
                {
                    TriangleIntersection tI;
                    tI.Triangle = triangle;
                    // INFO: An intersection point must actually be defined to be just above the real point.
                    tI.Point = intersectionPoint + CONST_.INTERSECTION_MARGIN * triangle.Normal;//_---------------------------
                    tIntersections.Add(tI);
                }
            }
            
            tIntersections.Sort((i1,i2) => 
                UTIL_.Length(i1.Point - ray.Start).CompareTo(UTIL_.Length(i2.Point - ray.Start)));
            
            return tIntersections;
        }

        public List<SphereIntersection> DetectSphereIntersections(Ray ray)
        {
            var sIntersections = new List<SphereIntersection>();
            foreach (var sphere in _spheres)
            {
                var (results, intersectionPoint) = sphere.Intersection(ray);
                if (results == CONST_.INTERSECTION)
                {
                    SphereIntersection sI;
                    sI.Sphere = sphere;
                    sI.Point = intersectionPoint + CONST_.INTERSECTION_MARGIN * sphere.CalcNormal(intersectionPoint);
                    sIntersections.Add(sI);
                }
            }
            
            return sIntersections;
        }

        public COLOR_DOUBLE GetLightContribution(vec3 mPoint, vec3 mNormal)
        {    // THIS FUNCTION: Decides how much extra 'light juice'
             // the light source is to contribute onto a point in the scene.
            var color = new vec3(0.0f);
            var lightCount = 0;
            var lightArea = 0.0;

            foreach (var light in _lights)
            {
                foreach (var lightTriangle in light.Triangles)
                {
                    lightArea += lightTriangle.Area();
                    for (int i = 0; i < CONST_.SHADOW_RAY_COUNT; ++i)
                    {
                        ++lightCount;
                        // Create shadow rays from point to light
                        var lightPoint = lightTriangle.GetRandomPoint();
                        Ray rayToLight = new Ray(mPoint, glm.normalize(lightPoint - mPoint));
                        
                        // Check for Visibility, are there any triangles or spheres intersecting on the way to
                        // light source from inputted intersection point? If there are we will find them here.
                        var sphereIntersections = DetectSphereIntersections(rayToLight);
                        var triangleIntersections = DetectTriangleIntersections(rayToLight);

                        var lightDistance = UTIL_.Distance(mPoint, lightPoint);
                        
                        if (sphereIntersections.Any())
                        {
                            var sIntersection = sphereIntersections.First();
                            var sIntersectionDistance = UTIL_.Distance(mPoint, sIntersection.Point);

                            if (sIntersectionDistance < lightDistance)
                            {// NOT VISIBLE! If it's shorter then the point is occluded from light.
                                continue;
                            }
                        }

                        if (triangleIntersections.Any())
                        {
                            var tIntersection = triangleIntersections.First();
                            var tIntersectionDistance = UTIL_.Distance(mPoint, tIntersection.Point);
                            if (tIntersectionDistance < lightDistance)
                            {// NOT VISIBLE! If it's shorter then the point is occluded from light.
                                continue;
                            }
                
                            // Calculate Geometric Term!
                            // Works Exactly like below. Advantage here being: We skip computing with cos(), its faster?
                            var alpha = glm.dot(mNormal, rayToLight.Direction);
                            var beta = UTIL_.Clamp(glm.dot(glm.normalize(lightTriangle.Normal), 
                                    glm.normalize(rayToLight.Direction)), 0.0, 1.0);
                            var geometric = alpha * beta / Math.Pow(lightDistance*1f, 2);

                            var lightSurface = lightTriangle.Surface;
                            color += lightSurface.GetColor() * lightSurface.Emission * (float)geometric;

                            //color = UTIL_.TruncateColorDouble(color);

                            /*
                            // Calculate Geometric Term!
                            var alpha2 = UTIL_.Angle(mNormal, rayToLight.Direction);
                            var beta2 = UTIL_.Angle(lightTriangle.Normal, rayToLight.Direction);
                            var geometric2 =
                            glm.cos(alpha2) * glm.cos((float)beta2) / (float)Math.Pow(lightDistance, 2);
                            */

                        }
                    }
                }
            }
            
            return color * (float)lightArea / lightCount;
        }
    }
}
