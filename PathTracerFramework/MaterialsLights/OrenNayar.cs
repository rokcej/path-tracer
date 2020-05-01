using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathTracer {
	public class OrenNayar : BxDF {
		private Spectrum kd; // Albedo
		private double sigma; // Roughness
		private double A, B;

		public OrenNayar(Spectrum kd, double sigma) {
			this.kd = kd;
			this.sigma = sigma;

			this.A = 1.0 - sigma * sigma / (2.0 * (sigma * sigma + 0.33));
			this.B = 0.45 * sigma * sigma / (sigma * sigma + 0.09);
		}

		public override Spectrum f(Vector3 wo, Vector3 wi) {
			if (!Utils.SameHemisphere(wo, wi))
				return Spectrum.ZeroSpectrum;

			(double oPhi, double oTheta) = cartesianToPolar(wo);
			(double iPhi, double iTheta) = cartesianToPolar(wi);

			double alpha = Math.Max(iTheta, oTheta);
			double beta = Math.Min(iTheta, oTheta);

			return kd * Utils.PiInv * (A + B * Math.Max(0, Math.Cos(iPhi - oPhi)) * Math.Sin(alpha) * Math.Sin(beta));
		}

		public override (Spectrum, Vector3, double) Sample_f(Vector3 wo) {
			Vector3 wi = Samplers.CosineSampleHemisphere();
			if (wo.z < 0.0)
				wi.z *= 1.0;

			return (f(wo, wi), wi, Pdf(wo, wi));
		}

		public override double Pdf(Vector3 wo, Vector3 wi) {
			if (!Utils.SameHemisphere(wo, wi))
				return 0.0;

			return Math.Abs(wi.z) * Utils.PiInv;
		}

		private (double, double) cartesianToPolar(Vector3 w) {
			double phi = Math.Atan(w.y / w.x);
			double theta = Math.Acos(w.z); // / Math.Sqrt(w.x * w.x + w.y * w.y + w.z * w.z)
			return (phi, theta);
		}
	}
}
