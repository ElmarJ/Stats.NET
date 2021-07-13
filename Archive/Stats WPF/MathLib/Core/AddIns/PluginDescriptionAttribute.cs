using System;

namespace MathLib.Core.AddIns
{
	[AttributeUsage(AttributeTargets.Class)]
	public class PluginDescriptionAttribute : System.Attribute
	{
		private string description;

		public PluginDescriptionAttribute(string Description) : base()
		{
			description = Description;
			return;
		}

		public override string ToString()
		{
			return description;
		}
	}
}