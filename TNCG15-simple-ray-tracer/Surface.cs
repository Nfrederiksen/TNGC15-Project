using System;
using GlmNet;
using DIRECTION = GlmNet.vec3;
using COLOR_DOUBLE = GlmNet.vec3;
using CONST_ = TNCG15_simple_ray_tracer.Constants;

namespace TNCG15_simple_ray_tracer
{
    public class Surface
    {
        private readonly COLOR_DOUBLE _color;
        private readonly int _reflectionModel;
        private float _reflectionCoefficient = 0.65f;
        private readonly COLOR_DOUBLE _emission = new COLOR_DOUBLE(0.0f);
        // Property
        public float ReflectionCoefficient => _reflectionCoefficient;
        
        public vec3 Emission => _emission;

        public vec3 GetColor()
        {
            return HasReflectionModel(CONST_.SPECULAR) ? new vec3(0.0f) : _color;
        }

        // Param. Constructor
        public Surface(COLOR_DOUBLE color, int refModel)
        {
            _color = color;
            _reflectionModel = refModel;
            if (refModel == CONST_.LIGHTSOURCE)
            {
                _emission = new vec3(80.0f);
            }
        }
        // Param-less Constructor
        public Surface()
        : this(new vec3(0), CONST_.LAMBERTIAN ) {
            
        }
        
        // Reflection Models
        private COLOR_DOUBLE LambertianReflection()
        {
            return _color * (float)(_reflectionCoefficient / Math.PI);
        }

        private COLOR_DOUBLE SpecularReflection()
        {
            return _color * 1.0f;
        }

        public bool HasReflectionModel(int model)
        {
            return model == _reflectionModel;
        }
        /// <summary>
        /// Reflects the surface's color according to its reflection model.
        /// </summary>
        public COLOR_DOUBLE Reflect()
        {
            switch (_reflectionModel)
            {
                case CONST_.LAMBERTIAN:
                    return LambertianReflection();
                case CONST_.SPECULAR:
                    return SpecularReflection();
                default:
                    Console.WriteLine("STOP! Invalid Reflection Model: " + _reflectionModel);
                    return new vec3(0.0f);
            }
        }

        public Ray BounceRay(Ray inRay, vec3 point, DIRECTION normal)
        {
            switch (_reflectionModel)
            {
                case CONST_.LAMBERTIAN:
                    return inRay.SampleHemisphere(point,  normal); 
                case CONST_.SPECULAR:
                    return inRay.Bounce(point, normal);
                default:
                    Console.WriteLine("STOP!! Invalid Reflection Model: " + _reflectionModel);
                    return new Ray();
            }
        }
    }
}