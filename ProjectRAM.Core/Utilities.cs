using System.Numerics;

namespace ProjectRAM.Core;

internal static class Utilities
{
	/// <summary>
	/// Compares two big integers.
	/// </summary>
	/// <param name="v1">First value</param>
	/// <param name="v2">Second value</param>
	/// <returns>Bigger integer value</returns>
	internal static string Max(string v1, string v2)
	{
		if (string.Empty == v1)
		{
			return v2;
		}
		if (string.Empty == v2)
		{
			return v1;
		}
		return BigInteger.Parse(v1) >= BigInteger.Parse(v2) ? v1 : v2;
	}
}