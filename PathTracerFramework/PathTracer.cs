using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PathTracer.Samplers;

namespace PathTracer
{
    class PathTracer
    {
        const int MAX_BOUCES = 20;
        public Spectrum Li(Ray _r, Scene s) {
            var ray = _r;
            var L = Spectrum.ZeroSpectrum;
            var beta = Spectrum.Create(1);
            int numBounces = 0;

            while (numBounces <= MAX_BOUCES) {
                // Test for intersections
                (double? t, SurfaceInteraction si) = s.Intersect(ray);

                if (si == null) { // No hit
                    break;
                } else if (si.Obj is Light) { // Light hit
                    if (numBounces == 0)
                        L = beta * si.Le(si.Wo);
                    break;
                }

                // Path reuse
                var Ld = Light.UniformSampleOneLight(si, s);
                L.AddTo(beta * Ld);

                // Diffuse
                Shape shape = si.Obj as Shape;
                (Spectrum f, Vector3 wi, double pdf, bool isSpecular) = shape.BSDF.Sample_f(si.Wo, si);

                ray = new Ray(si.Point, wi);
                beta *= f * Vector3.AbsDot(wi, si.Normal) / pdf;

                // Russian roulette
                if (numBounces > 3) {
                    double q = 1.0 - beta.Max();
                    if (ThreadSafeRandom.NextDouble() < q)
                        break;
                    beta /= 1.0 - q;
                }

                ++numBounces;
            }

            

            return L;
        }

    }
}
