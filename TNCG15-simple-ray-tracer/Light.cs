using System.Collections.Generic;
using GlmNet;

namespace TNCG15_simple_ray_tracer
{
    public class Light
    {
        // Field
        private List<Triangle> _triangles = new List<Triangle>();
        // Property
        public List<Triangle> Triangles => _triangles;
        // Constructor
        public Light()
        {
            //         V cam
            //    Seen from below light:
            //    p3\----p2        x<----.
            //    |  \   |               |
            //    |   \  |               v z
            //    p1---\p0
            //____________________wall Normals are -y axis
            
            vec3 p0 = new vec3(-1.0f, 4.99f, 8.0f);
            vec3 p1 = new vec3(1.0f, 4.99f, 8.0f);
            vec3 p2 = new vec3(-1.0f, 4.99f, 6.0f);
            vec3 p3 = new vec3(1.0f, 4.99f, 6.0f);
            Surface whiteSurface = new Surface(new vec3(1.0f), Constants.LIGHTSOURCE );
            /*
            
            var light1 = new Triangle(p0, p3, p1, whiteSurface);
            var light2 = new Triangle(p0, p2, p3, whiteSurface);

            _triangles.Add(light1);
            _triangles.Add(light2);
            Console.WriteLine("light normal:" + light1.Normal + light2.Normal );
*/
            _triangles.Add(new Triangle(p0,p1,p3, whiteSurface));
            _triangles.Add(new Triangle(p0,p3,p2,whiteSurface));
            
            
        }
    }
}