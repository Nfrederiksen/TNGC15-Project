using System;
using Accord.Math;
using Accord.Statistics.Distributions.Univariate;
using GlmNet;

namespace TNCG15_simple_ray_tracer
{
    /* Class can Generate uniform random doubles */
    public class Utilities
    {
        public static double UniformRand()
        {
            UniformContinuousDistribution _UCDRandom = new UniformContinuousDistribution(0.0,1.0);
            return _UCDRandom.Generate();
        }

        public static double RandMinMax(double min, double max)
        {
            return min + UniformRand() * (max - min);
        }

        /// For the incident vector I and surface orientation N,
        /// returns the reflection direction : result = I - 2.0 * dot(N, I) * N.
        public static vec3 Reflect(vec3 i, vec3 n)
        {
            return i - 2.0f * glm.dot(n, i) * n;
        }
        
        public static double Distance(vec3 p0, vec3 p1)
        {
            return Length(p0 - p1);
        }
        
        // Clamp returns the value of x constrained to the range minVal to maxVal.
        public static double Clamp(double x, double minVal, double maxVal)
        {
            return Math.Min(Math.Max(x, minVal), maxVal);
        }
        // Overloaded Clamp for Floats.
        private static float Clamp(float x, float minVal, float maxVal)
        {
            return Math.Min(Math.Max(x, minVal), maxVal);
        }

        public static float Angle(vec3 x, vec3 y)
        {
            return glm.acos(Clamp(glm.dot(x, y), -1f, 1f));
        }
        
        /// <summary>
        ///  Rodrigues' rotation formula
        /// </summary>
        /// <param name="v"> Vector to be rotated</param>
        /// <param name="angle"> How much to be rotated</param>
        /// <param name="k"> Rotated around this vector</param>
        /// <returns></returns>
        public static vec3 Rotate(vec3 v, float angle, vec3 k)
        {
            return v * glm.cos(angle) + (glm.cross(k, v)
                * glm.sin(angle) + k * (glm.dot(k, v) * (1 - glm.cos(angle))));
        }

        public static double Length(vec3 v)
        {// Returns length of a vector. 
            return Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
        }
        
        public static vec3 Cross(vec3 v1, vec3 v2)
        {
            vec3 v3;
            v3.x = v1.y * v2.z - v1.z * v2.y;
            v3.y = v1.z * v2.x - v1.x * v2.z;
            v3.z = v1.x * v2.y - v1.y * v2.x;
            
            return v3;
        }

        public static float Truncate(float val)
        {
            if (val < 0 )
            {
                val = 0;
            }

            if (val > 255)
            {
                val = 255;
            }

            return val;
        }
        
    }
}