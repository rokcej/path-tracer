using System;
using MathNet.Numerics.Integration;

namespace PathTracer
{
  class Sphere : Shape
  {
    public double Radius { get; set; }
    public Sphere(double radius, Transform objectToWorld)
    {
      Radius = radius;
      ObjectToWorld = objectToWorld;
    }

        public override (double?, SurfaceInteraction) Intersect(Ray _r) {
            Ray ray = WorldToObject.Apply(_r);

            double a = Vector3.Dot(ray.d, ray.d); // = 1;
            double b = 2 * Vector3.Dot(ray.o, ray.d);
            double c = Vector3.Dot(ray.o, ray.o) - Radius * Radius;
            (bool hasSolution, double t0, double t1) = Utils.Quadratic(a, b, c);

            if (!hasSolution || (t0 < Renderer.Epsilon && t1 < Renderer.Epsilon))
                return (null, null);

            double tHit = (t0 < Renderer.Epsilon ? t1 : t0);

            Vector3 pHit = ray.Point(tHit);
            Vector3 normal = pHit * (1.0 / Radius);
            Vector3 dpdu = new Vector3(-pHit.y, pHit.x, 0);

            SurfaceInteraction si = new SurfaceInteraction(pHit, normal, -ray.d, dpdu, this);

            return (tHit, ObjectToWorld.Apply(si));

            // TODO: Compute quadratic sphere coefficients

            // TODO: Initialize _double_ ray coordinate values

            // TODO: Solve quadratic equation for _t_ values

            // TODO: Check quadric shape _t0_ and _t1_ for nearest intersection

            // TODO: Compute sphere hit position and $\phi$
      
            // TODO: Return shape hit and surface interaction
        }

    public override (SurfaceInteraction, double) Sample()
    {
            Vector3 sample = Samplers.UniformSampleSphere();
            Vector3 point = sample * Radius;
            Vector3 normal = sample;
            Vector3 dpdu = new Vector3(-point.y, point.x, 0.0);
            double pdf = 1.0 / Area();

            SurfaceInteraction si = new SurfaceInteraction(point, normal, Vector3.ZeroVector, dpdu, this);
            return (ObjectToWorld.Apply(si), pdf);



      // TODO: Implement Sphere sampling
      
      // TODO: Return surface interaction and pdf
      
      // A dummy return example
      double dummyPdf = 1.0;
      Vector3 dummyVector = new Vector3(0,0,0);
      SurfaceInteraction dummySurfaceInteraction = new SurfaceInteraction(dummyVector, dummyVector, dummyVector, dummyVector, this);
      return (dummySurfaceInteraction, dummyPdf);
    }

    public override double Area() { return 4 * Math.PI * Radius * Radius; }

    public override double Pdf(SurfaceInteraction si, Vector3 wi)
    {
      throw new NotImplementedException();
    }

  }
}
