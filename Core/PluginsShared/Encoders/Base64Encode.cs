using System;
using System.Text;

namespace Mnk.TBox.Core.PluginsShared.Encoders
{
	public static class Base64Encode
	{
		public static string EncodeTo64(string toEncode)
		{
			return Convert.ToBase64String(Encoding.ASCII.GetBytes(toEncode));
		}

		public static string DecodeFrom64(string encodedData)
		{
			return Encoding.ASCII.GetString(Convert.FromBase64String(encodedData));
		}
	}
}
