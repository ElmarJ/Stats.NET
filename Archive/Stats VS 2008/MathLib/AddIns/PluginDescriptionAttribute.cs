using System;

namespace Stats.Core.AddIns
{
	[AttributeUsage(AttributeTargets.Class)]
	public class PluginDescriptionAttribute : Attribute
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