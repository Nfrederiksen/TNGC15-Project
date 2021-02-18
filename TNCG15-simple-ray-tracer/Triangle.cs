using System;
using GlmNet;
using DIRECTION = GlmNet.vec3;
using COLOR_DOUBLE = GlmNet.vec3;
using UTIL_ = TNCG15_simple_ray_tracer.Utilities;
using CONST_ = TNCG15_simple_ray_tracer.Constants;

namespace TNCG15_simple_ray_tracer
{
    public class Triangle
    {
        // Fields       
        private readonly vec3[] _posArray = new vec3[3];
        private readonly Surface _surface;
        private readonly vec3 _normal;
        
        // Properties.
        public vec3 Normal => _normal;

        public Surface Surface => _surface;

        // Param. Constructors
        public Triangle(vec3 p0, vec3 p1, vec3 p2, Surface surface)
        {
            _posArray[0] = p0;
            _posArray[1] = p1;
            _posArray[2] = p2;
            _surface = surface;
            
            // Set the normal.
            _normal = CalcTriangleNormal();
        }

        // Some Extra Utility functions.
        private vec3 Edge1()
        {
            return _posArray[1] - _posArray[0];
        }
        
        private vec3 Edge2()
        {
            return _posArray[2] - _posArray[0];
        }

        public vec3 CalcTriangleNormal()
        {
            return  glm.normalize(UTIL_.Cross(Edge1(), Edge2()));
        }
        
        /// <summary>
        /// Find intersection of ray and triangle using Möller-Trumbore. Sources for the Theory
        /// can be found in the report.
        /// </summary>
        /// <param name="ray">  </param>
        /// <returns> INTERSECTION if found, NOT_INTERSECTION otherwise</returns>
        public (int, vec3) Intersection(Ray ray)
        {
            // Position of the ray.
            vec3 rayStart = ray.Start;
            vec3 direction = glm.normalize(ray.Direction);
            vec3 intersection = new vec3(0.0f);
            
            // Triangle edges
            vec3 edge1 = Edge1();
            vec3 edge2 = Edge2();
            // Calculate determinant.
            vec3 edgeNormal = UTIL_.Cross(direction, edge2);
            float determinant = glm.dot(edge1, edgeNormal);
            // If det. is equal (near) 0, Ray lies in the triangle plane OR is parallel to triangle plane.
            if (Math.Abs(determinant) < CONST_.EPSILON)
            {
                return (CONST_.NOT_INTERSECTION, intersection);
            }

            float invDeterminant = 1.0f / determinant;
            // Calculate ray->vertex from first vertex to ray origin.
            vec3 rayToVertex = rayStart - _posArray[0];
            
            // Calculate U- parameter and test bound, if less than 0 or greater than 1,
            // intersection lies outside of the triangle.
            double U = glm.dot(rayToVertex, edgeNormal) * invDeterminant;
            if (U < 0.0f || U > 1.0f)
            {
                return (CONST_.NOT_INTERSECTION, intersection);;
            }
            
            // Prepare to test V parameter
            vec3 Q = UTIL_.Cross(rayToVertex, edge1);
            // Calculate V parameter and test bound
            double V = glm.dot(direction, Q) * invDeterminant;
            // If the intersection lies outside of the triangle.
            if (V < 0.0f || U + V > 1.0f)
            {
                return (CONST_.NOT_INTERSECTION, intersection);;
            }

            double T = glm.dot(edge2, Q) * invDeterminant;
            if (T > CONST_.EPSILON && T < CONST_.t1)
            {  
                // TODO erase -> Console.WriteLine(" INTERSECTION FOUND! in triangle.");
                intersection = rayStart + direction * (float) T;
                return (CONST_.INTERSECTION, intersection);
            }

            return (CONST_.NOT_INTERSECTION, intersection);;
        }

        public double Area()
        {// Area of a right-triangle is equal to half the magnitude of the triangle normal.
            return 0.5f * UTIL_.Length(UTIL_.Cross(Edge1(), Edge2()));
        }

        private vec3 FromBarycentric(float a, float b)
        {
            vec3 pos = (1.0f - a - b) * _posArray[0] + a * _posArray[1] + b * _posArray[2];
            return pos + 0.001f * new vec3(0.0f, 0.0f, -0.1f);
        }
        
        public vec3 GetRandomPoint()
        {
            var triangleArea = Area();
            var a = UTIL_.RandMinMax(0.0, 1.0 / triangleArea);
            var b = UTIL_.RandMinMax(0.0, 1.0 / triangleArea);
            if (a + b > 1.0)
            {
                return GetRandomPoint();
            }

            return FromBarycentric((float) a, (float) b);
        }
    }
}