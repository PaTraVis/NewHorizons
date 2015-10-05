using System;

namespace BEPUMono
{
	public class AssetAccessException : Exception
	{
		public AssetAccessException(string s) : base(s)
		{
		}
	}
}