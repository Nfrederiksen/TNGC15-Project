using System;
using GlmNet;
using CONST_ = TNCG15_simple_ray_tracer.Constants;

namespace TNCG15_simple_ray_tracer
{
    public class Sphere
    {
        // Fields
        private readonly vec3 _position;
        private readonly float _radius;
        private readonly Surface _surface;
        
        // Property
        public Surface Surface => _surface;
        
        // Parameterised Constructor
        public Sphere(vec3 position, float radius, Surface surface)
        {
            _position = position;
            _radius = radius;
            _surface = surface;
        }
        // Parameter-less Constructor
        public Sphere()
        : this(new vec3(0), 0.0f, new Surface()){
            
        }

        public vec3 CalcNormal(vec3 point)
        {
            return glm.normalize(point - _position);
        }

        public (int, vec3) Intersection(Ray ray)
        {
            //Ray offset from sphere centre. All as specified in the Lecture slides.
            var intersection = new vec3(0.0f);
            vec3 rayDirectionNormalized = glm.normalize(ray.Direction);
            vec3 rayOrigin = ray.Start;
            vec3 sphereCenter = _position;
            float sphereRadius = _radius;
            var centerToOrigin = rayOrigin - sphereCenter;

            float a = glm.dot(rayDirectionNormalized, rayDirectionNormalized);
            float b = glm.dot(2 * rayDirectionNormalized, centerToOrigin);
            float c = glm.dot(centerToOrigin, centerToOrigin) - (float)Math.Pow(sphereRadius, 2);
            float d1 = -b / 2.0f;
            float d2 = -b / 2.0f;
            float underSqrt = (float)Math.Pow(b/2f, 2) - a * c; 
            float sqrtTerm = (float)Math.Sqrt(underSqrt);
            d1 += sqrtTerm;
            d2 -= sqrtTerm;
            // INFO: If the value under the square root is less than zero,
            // then there is no intersection between ray and sphere.
            if (underSqrt < CONST_.EPSILON)
            {
                return (CONST_.NOT_INTERSECTION, intersection);
            }
            // INFO: Additionally, if both terms d1 and d2 are
            // equal to or less than zero, then there is also no intersection.
            if (d1 <= 0f && d2 <= 0f)
            {
                return (CONST_.NOT_INTERSECTION, intersection);
            }
            intersection = rayOrigin + Math.Min(d1, d2) * rayDirectionNormalized;
            return (CONST_.INTERSECTION, intersection);
        }
    }
}