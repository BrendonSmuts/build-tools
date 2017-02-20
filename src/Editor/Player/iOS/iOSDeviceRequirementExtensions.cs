using System;
using UnityEditor;


namespace SweetEditor.Build
{
	public static class iOSDeviceRequirementExtensions
	{

		public static iOSDeviceRequirement SetIdiomRequirement(this iOSDeviceRequirement requirement, IdiomDeviceRequirement value)
		{
			throw new NotImplementedException();
			/*string val = string.Empty;

			switch (value)
			{
				case IdiomDeviceRequirement.Any:
					val = "universal";
					break;
				case IdiomDeviceRequirement.iPhone:
					val = "iphone";
					break;
				case IdiomDeviceRequirement.iPad:
					val = "ipad";
					break;
				case IdiomDeviceRequirement.Watch:
					val = "watch";
					break;
			}

			requirement.values["idiom"] = val; 
			return requirement;*/
		}


		public static IdiomDeviceRequirement GetIdiomRequirement(this iOSDeviceRequirement requirement)
		{
			throw new NotImplementedException();
			/*string value;
			requirement.values.TryGetValue("idiom", out value);

			switch (value)
			{
				case "iphone":
					return IdiomDeviceRequirement.iPhone;
				case "ipad":
					return IdiomDeviceRequirement.iPad;
				case "watch":
					return IdiomDeviceRequirement.Watch;
			}

			return IdiomDeviceRequirement.Any;*/
		}


		public static iOSDeviceRequirement SetScaleRequirement(this iOSDeviceRequirement requirement, ScaleDeviceRequirement value)
		{
			throw new NotImplementedException();
			/*string val = string.Empty;

			switch (value)
			{
				case ScaleDeviceRequirement.X1:
					val = "1x";
					break;
				case ScaleDeviceRequirement.X2:
					val = "2x";
					break;
				case ScaleDeviceRequirement.X3:
					val = "3x";
					break;
			}

			requirement.values["graphics-scale-set"] = val;
			return requirement;*/
		}


		public static ScaleDeviceRequirement GetScaleRequirement(this iOSDeviceRequirement requirement)
		{
			throw new NotImplementedException();
			/*string value;
			requirement.values.TryGetValue("graphics-scale-set", out value);

			switch (value)
			{
				case "1x":
					return ScaleDeviceRequirement.X1;
				case "2x":
					return ScaleDeviceRequirement.X2;
				case "3x":
					return ScaleDeviceRequirement.X3;
			}

			return ScaleDeviceRequirement.Any;*/
		}


		public static iOSDeviceRequirement SetGraphicsRequirement(this iOSDeviceRequirement requirement, GraphicsDeviceRequirement value)
		{
			throw new NotImplementedException();
			/*string value;
			requirement.values.TryGetValue("graphics-scale-set", out value);

			switch (value)
			{
				case "1x":
					return ScaleDeviceRequirement.X1;
				case "2x":
					return ScaleDeviceRequirement.X2;
				case "3x":
					return ScaleDeviceRequirement.X3;
			}

			return ScaleDeviceRequirement.Any;*/
		}


		public static GraphicsDeviceRequirement GetGraphicsRequirement(this iOSDeviceRequirement requirement)
		{
			throw new NotImplementedException();
			/*string value;
			requirement.values.TryGetValue("graphics-scale-set", out value);

			switch (value)
			{
				case "1x":
					return ScaleDeviceRequirement.X1;
				case "2x":
					return ScaleDeviceRequirement.X2;
				case "3x":
					return ScaleDeviceRequirement.X3;
			}

			return ScaleDeviceRequirement.Any;*/
		}
	}
}
