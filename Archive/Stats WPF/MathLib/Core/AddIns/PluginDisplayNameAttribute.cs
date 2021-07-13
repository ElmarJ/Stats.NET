using System;

namespace MathLib.Core.AddIns
{
	[AttributeUsage(AttributeTargets.Class)]
	public class PluginDisplayNameAttribute : System.Attribute
	{
		private string displayName;

		public PluginDisplayNameAttribute(string DisplayName) : base()
		{
			displayName = DisplayName;
            return;
		}

		public override string ToString()
		{
			return displayName;
		}
	}
}