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

			// Cos term
			// Adicijski izrek: cos(x +- y) = cos(x) * cos(y) -+ sin(x) * sin(y)
			double cosDiff = Utils.CosPhi(wi) * Utils.CosPhi(wo) + Utils.SinPhi(wi) * Utils.SinPhi(wo);

			// Sin and tan terms
			double sinAlpha, tanBeta;
			if (Utils.AbsCosTheta(wi) > Utils.AbsCosTheta(wo)) { // oTheta > iTheta
				sinAlpha = Utils.SinTheta(wo);
				tanBeta = Utils.SinTheta(wi) / Utils.AbsCosTheta(wi);
			} else { // iTheta >= oTheta
				sinAlpha = Utils.SinTheta(wi);
				tanBeta = Utils.SinTheta(wo) / Utils.AbsCosTheta(wo);
			}

			return kd * Utils.PiInv * (A + B * Math.Max(0, cosDiff) * sinAlpha * tanBeta);
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
	}
}
