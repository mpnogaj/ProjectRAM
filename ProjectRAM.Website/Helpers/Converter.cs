using Microsoft.AspNetCore.Http;
using System.Collections.Specialized;
using System.IO;

namespace ProjectRAM.Website.Helpers
{
	public static class Converter
	{
		public static StringCollection StringToStringCollection(string text, char separator)
		{
			StringCollection sc = new StringCollection();
			sc.AddRange(text.Split(separator));
			return sc;
		}

		public static StringCollection IFormFileToStringCollection(IFormFile file)
		{
			StringCollection sc = new StringCollection();
			using (var reader = new StreamReader(file.OpenReadStream()))
			{
				while (reader.Peek() >= 0)
				{
					sc.Add(reader.ReadLine());
				}
			}
			return sc;
		}
	}
}
