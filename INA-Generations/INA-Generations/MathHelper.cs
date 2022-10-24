using System;

namespace INA_Generations
{
	public static class MathHelper
	{
		public static double ConvertToRadians(double angle)
		{
			return (Math.PI / 180.0) * angle;
		}

		public static double SinAngle(double angle)
		{
			return Math.Sin(ConvertToRadians(angle));
		}
		
		public static double CosAngle(double angle)
		{
			return Math.Cos(ConvertToRadians(angle));
		}
	}
}