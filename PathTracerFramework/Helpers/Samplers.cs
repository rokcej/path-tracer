﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathTracer
{
  public static class Samplers
  {
    public static double UniformConePdf(double cosTheta) {
      return 1 / (2 * Math.PI * (1 - cosTheta));
    }

    public static Vector3 UniformSampleSphere()
    {
      // http://corysimon.github.io/articles/uniformdistn-on-sphere/
      double theta = Math.Acos(1.0 - 2.0 * ThreadSafeRandom.NextDouble());
      double phi = 2.0 * Math.PI * ThreadSafeRandom.NextDouble();
      return new Vector3(
          Math.Sin(theta) * Math.Cos(phi),
          Math.Sin(theta) * Math.Sin(phi),
          Math.Cos(theta)
      );
    }

    public static (double, double) UniformSampleSquare()
    {
      var u = ThreadSafeRandom.NextDouble();
      var v = ThreadSafeRandom.NextDouble();
      return (u, v);
    }

    public static (double, double) UniformSampleDisk()
    {
      var u = ThreadSafeRandom.NextDouble();
      var v = ThreadSafeRandom.NextDouble();
      var r = Math.Sqrt(u);
      var theta = 2 * Math.PI * v;
      return (r * Math.Cos(theta), r * Math.Sin(theta));
    }

    /// <summary>
    /// Cosine sample hemisphere with projection of uniform-sampled disk. Returns local coords.
    /// </summary>
    /// <returns></returns>
    public static Vector3 CosineSampleHemisphere()
    {
      (double x, double y) = UniformSampleDisk();
      var z = Math.Sqrt(Math.Max(0, 1 - x * x - y * y));
      return new Vector3(x, y, z);
    }

    /// <summary>
    /// Thread safe random function implementation
    /// </summary>
    public static class ThreadSafeRandom
    {
      private static Random _global = new Random(12);
      [ThreadStatic]
      private static Random _local;
      [ThreadStatic]
      public static List<double> buffer;
      [ThreadStatic]
      public static int cnt = -1;
      public static double NextDouble()
      {
        Random inst = _local;
        if (inst == null)
        {
          int seed;
          lock (_global) seed = _global.Next();
          _local = inst = new Random(seed);
          buffer = new List<double>(2000);
          cnt = -1;
        }
        if (cnt == -1)
        {
          for (int i = 0; i < 1000; i++)
            buffer.Add(inst.NextDouble());
        }
        else if (cnt >= 999)
        {
          buffer.RemoveRange(0, 500);
          for (int i = 0; i < 500; i++)
            buffer.Add(inst.NextDouble());
          cnt = 499;
        }
        //var r = inst.NextDouble();
        cnt++;
        var r = buffer[cnt];
        return r;
      }
    }
  }
}
