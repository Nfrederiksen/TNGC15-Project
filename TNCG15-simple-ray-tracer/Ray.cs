using System;
using Accord;
using GlmNet;
using DIRECTION = GlmNet.vec3;
using COLOR_DOUBLE = GlmNet.vec3;
using UTIL_ = TNCG15_simple_ray_tracer.Utilities;

namespace TNCG15_simple_ray_tracer
{
    public class Ray
    {
        private DIRECTION _start, _direction;

        public DIRECTION Start => _start;
        
        public vec3 Direction  => _direction;
        

        // P-less Constructor
        public Ray(): this(new DIRECTION(0.0f),new DIRECTION(0.0f) )
        {
            
        }
        // P. Constructor
        public Ray(vec3 start, DIRECTION dir)
        {
            _start = start;
            _direction = dir;
        }

        public Ray Bounce(vec3 point, DIRECTION normal)
        {
            DIRECTION newDirection = UTIL_.Reflect(_direction, normal);
            return new Ray(point, newDirection);
        }

        public Ray SampleHemisphere(DIRECTION point, DIRECTION normal)
        {
            
            var rand1 = UTIL_.UniformRand();
            var rand2 = UTIL_.UniformRand();
            float inclination = glm.acos((float)Math.Sqrt(rand1));
            float azimuth = (float)(2 * Math.PI * rand2);
            
            DIRECTION helper = normal + new DIRECTION(1);
            DIRECTION tangent = glm.normalize(UTIL_.Cross(helper, normal));
            
            // Re-direct the real vector. First rotate around 'tangent' then rotate around 'normal'.
            vec3 randomDirection = normal;
            randomDirection = glm.normalize(UTIL_.Rotate(randomDirection, inclination, tangent));
            randomDirection = glm.normalize(UTIL_.Rotate(randomDirection, azimuth, normal));
            
            return new Ray(point, randomDirection);
        }
    }
}