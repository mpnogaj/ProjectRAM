using System;
using System.ComponentModel;
using ProjectRAM.Editor.Properties;

namespace ProjectRAM.Editor
{
	public class InfoAttribute : Attribute { }

	public class LocalizedDisplayNameAttribute : DisplayNameAttribute
	{
		private readonly string _resourceKey;

		public override string DisplayName
			=> Strings.ResourceManager.GetString(_resourceKey, Settings.Language) ?? throw new Exception();

		public LocalizedDisplayNameAttribute(string resourceKey) : base()
		{
			_resourceKey = resourceKey;
		}
	}
}
