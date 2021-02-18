using System;
using System.Collections.Generic;
using GlmNet;

namespace TNCG15_simple_ray_tracer
{
    public static class Constants
    {
        public const float EPSILON = 0.00000000000000001f;
        public const float t1 = 10000000.0f;
        public const int LAMBERTIAN = 0;
        public const int SPECULAR = 1;
        public const int LIGHTSOURCE = 2;
        public const int SHADOW_RAY_COUNT = 2;
        public const float INTERSECTION_MARGIN = 0.0001f;
        public const int INTERSECTION = 1;
        public const int NOT_INTERSECTION = 0;
        public const int WIDTH = 48;
        public const int HEIGHT = 48;
        public const int MAX_DEPTH = 5;
    }
}