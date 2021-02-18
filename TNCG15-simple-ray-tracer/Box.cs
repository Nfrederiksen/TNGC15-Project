using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using GlmNet;
using COLOR_DOUBLE = GlmNet.vec3;
using UTIL_ = TNCG15_simple_ray_tracer.Utilities;
using CONST_ = TNCG15_simple_ray_tracer.Constants;

namespace TNCG15_simple_ray_tracer
{
    public class Box
    {
        // Field
        private List<Triangle> _triangles = new List<Triangle>();
        // Property
        public List<Triangle> Triangles => _triangles;
        // Constructor
        public Box(vec3 position, float length)
        {
            // Chosen Surface Color
            Surface colTeal = new Surface(new COLOR_DOUBLE(0.0f,1.0f,1.0f), CONST_.LAMBERTIAN);
            // Triangles form a Tetrahedron.
            var pp = position;
            var ex = length;
            // Box vertices.
            var a = new vec3(pp.x,pp.y,pp.z);
            var b = new vec3(pp.x-ex,pp.y,pp.z);
            var c = new vec3(pp.x,pp.y+ex,pp.z);
            var d = new vec3(pp.x,pp.y,pp.z-ex);
            // Box triangles appended to list.        
            _triangles.Add(new Triangle(d, b, c, colTeal));
            _triangles.Add(new Triangle(a,b,d,colTeal));
            _triangles.Add(new Triangle(c,a,d,colTeal));
            _triangles.Add(new Triangle(c,b,a,colTeal));
        }
    }
}