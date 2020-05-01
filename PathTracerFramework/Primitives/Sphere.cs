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

      double a = 1;
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
    }

    public override (SurfaceInteraction, double) Sample() {
      Vector3 sample = Samplers.UniformSampleSphere();
      Vector3 point = sample * Radius;
      Vector3 normal = sample;
      Vector3 dpdu = new Vector3(-normal.y, normal.x, 0.0);
      double pdf = 1.0 / Area();

      SurfaceInteraction si = new SurfaceInteraction(point, normal, Vector3.ZeroVector, dpdu, this);
      return (ObjectToWorld.Apply(si), pdf);
    }

    public override (SurfaceInteraction, double) Sample(SurfaceInteraction _si) {
      SurfaceInteraction si = WorldToObject.Apply(_si);
      double dc2 = si.Point.LengthSquared();

      if (dc2 <= Radius * Radius) {
          // Point inside sphere
          return base.Sample(_si);
      }

      // Point outside sphere
      double dc = Math.Sqrt(dc2);

      double sinThetaMax = Radius / dc;
      double cosThetaMax = Utils.SinToCos(sinThetaMax);

      // Determine theta and phi for uniform cone sampling
      double cosTheta = (cosThetaMax - 1) * Samplers.ThreadSafeRandom.NextDouble() + 1;
      double sinTheta = Utils.CosToSin(cosTheta);
      double phi = Samplers.ThreadSafeRandom.NextDouble() * 2.0 *  Math.PI;

      // Distance between reference point and sample point on sphere
      double ds = dc * cosTheta - Math.Sqrt(Math.Max(0, Radius * Radius - dc2 * sinTheta * sinTheta));

      // Kosinusni zakon
      double cosAlpha = (dc2 + Radius * Radius - ds * ds) / (2 * dc * Radius);
      double sinAlpha = Utils.CosToSin(cosAlpha);

      // Construct coordinate system and use phi and theta as spherical coordinates to get point on sphere
      Vector3 wcZ = si.Point.Clone().Normalize();
      (Vector3 wcX, Vector3 wcY) = Utils.CoordinateSystem(wcZ);

      Vector3 nObj = Utils.SphericalDirection(sinAlpha, cosAlpha, phi, wcX, wcY, wcZ);
      Vector3 pObj = nObj * Radius;

      // Surface interaction
      Vector3 dpdu = new Vector3(-nObj.y, nObj.x, 0.0);
      SurfaceInteraction siSample = new SurfaceInteraction(pObj, nObj, Vector3.ZeroVector, dpdu, this);

      // Uniform cone PDF
      double pdf = Samplers.UniformConePdf(cosThetaMax);

      return (ObjectToWorld.Apply(siSample), pdf);
    }

    public override double Area() { return 4 * Math.PI * Radius * Radius; }

    public override double Pdf(SurfaceInteraction _si, Vector3 wi)
    {
      SurfaceInteraction si = WorldToObject.Apply(_si);
      double dist2 = si.Point.LengthSquared();
      if (dist2 <= Radius * Radius) {
        // Point inside sphere
        return base.Pdf(_si, wi);
      }
      // Point outside sphere
      double sinThetaMax = Radius / Math.Sqrt(dist2);
      double cosThetaMax = Utils.SinToCos(sinThetaMax);
      return Samplers.UniformConePdf(cosThetaMax);
    }

  }
}
