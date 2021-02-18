using System;
using System.Collections.Generic;
using System.Linq;
using GlmNet;
using COLOR_DOUBLE = GlmNet.vec3;
using UTIL_ = TNCG15_simple_ray_tracer.Utilities;
using CONST_ = TNCG15_simple_ray_tracer.Constants;

namespace TNCG15_simple_ray_tracer
{
    public class Pixel
    {
        // Fields
        private List<Ray> _rayList;
        private COLOR_DOUBLE _colorDouble;
        // Property
        public vec3 ColorDouble
        {
            get => _colorDouble;
            set => _colorDouble = value;
        }
        
        public List<Ray> RayList => _rayList;
        // Param-less Constructor
        public Pixel()
        {   
            _rayList = new List<Ray>();
            _colorDouble = new vec3(0.0f);
        }
        // Methods
        public void AddRay(Ray ray)
        {
            _rayList.Add(ray);
        }

        public Ray GetFirstRay()
        {
            return _rayList.First();
        }
    }
}