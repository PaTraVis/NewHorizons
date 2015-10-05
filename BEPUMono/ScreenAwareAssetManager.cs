using System;
using Microsoft.Xna.Framework;

namespace BEPUMono
{
	public class ScreenAwareAssetManager : AssetManager, IDisposable
	{
		private IScreen _screen;

		public ScreenAwareAssetManager(Game game, IScreen screen) : base(game)
		{
			_screen = screen;
		}

		public void Dispose()
		{
			AssetCache.Clear();
			ContentManager.Unload();
			ContentManager.Dispose();
		}
	}
}