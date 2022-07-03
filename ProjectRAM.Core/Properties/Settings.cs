using ProjectRAM.Core.Properties;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;

namespace ProjectRAM.Core.Properties
{
	/// <summary>
	/// Storage class for all Validator and Interpreter settings
	/// </summary>
	[ExcludeFromCodeCoverage]
	public static class Settings
	{
		private static CultureInfo _currentCulture = Thread.CurrentThread.CurrentCulture;

		/// <summary>
		/// Get or set current language
		/// </summary>
		public static CultureInfo CurrentCulture
		{
			get => _currentCulture;
			set
			{
				_currentCulture = value;
				Strings.Culture = value;
			}
		}
	}
}