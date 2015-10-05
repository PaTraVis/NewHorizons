using BEPUMono.Core.Entities;
using Microsoft.Xna.Framework;

namespace BEPUMono.Core.Graphics.Renderer
{
	public interface IRenderer
	{
		void Render(GameTime gameTime, Block block);
	}
}