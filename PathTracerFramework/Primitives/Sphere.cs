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

    public override (double?, SurfaceInteraction) Intersect(Ray ray)
    {
      Ray r = WorldToObject.Apply(ray);

      // TODO: Compute quadratic sphere coefficients

      // TODO: Initialize _double_ ray coordinate values

      // TODO: Solve quadratic equation for _t_ values

      // TODO: Check quadric shape _t0_ and _t1_ for nearest intersection

      // TODO: Compute sphere hit position and $\phi$
      
      // TODO: Return shape hit and surface interaction
      
      // A dummy return example
      double dummyHit = 0.0;
      Vector3 dummyVector = new Vector3(0,0,0);
      SurfaceInteraction dummySurfaceInteraction = new SurfaceInteraction(dummyVector, dummyVector, dummyVector, dummyVector, this);
      return (dummyHit, dummySurfaceInteraction);
    }

    public override (SurfaceInteraction, double) Sample()
    {
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
