using System.Collections.Concurrent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace BEPUMono
{
	public class AssetManager
	{
		public readonly ConcurrentDictionary<string, object> AssetCache;
		public ContentManager ContentManager { get; }
		private readonly Microsoft.Xna.Framework.Game _game;

		public AssetManager(Game game)
		{
			_game = game;
			AssetCache = new ConcurrentDictionary<string, object>();
            ContentManager = new ContentManager(game.Services, game.Content.RootDirectory);
		}

		public T GetAsset<T>(string name)
		{
			T asset;

			if (AssetCache.ContainsKey(name))
			{
				if (AssetCache[name].GetType() == typeof (T))
				{
					asset = (T) AssetCache[name];
				}
				else
				{
					throw new AssetAccessException($"Asset Type is not {typeof (T)}!");
				}
			}
			else
			{
				throw new ContentLoadException("Asset not found!");
			}

			return asset;
		}

		public T CreateAsset<T>(string name, string path)
		{
			T obj = default(T);
			if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(path))
			{
				obj = ContentManager.Load<T>(path);
				if (obj != null)
				{
					LoadAsset(name, obj);
				}
			}

			return obj;
		}

		public void LoadAsset(string name, object asset)
		{
			if (AssetCache.ContainsKey(name))
			{
				throw new AssetAccessException("There already exists an asset with this name!");
			}
			AssetCache.TryAdd(name, asset);
		}

		public void UnloadAsset(string name)
		{
			if (AssetCache.ContainsKey(name))
			{
				object obj;
				AssetCache.TryRemove(name, out obj);
			}
		}
	}
}