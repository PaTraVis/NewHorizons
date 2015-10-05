using System;
using BEPUMono.Core.Graphics.Renderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector3 = BEPUutilities.Vector3;

namespace BEPUMono.Core.Entities
{
    [Serializable]
    public abstract class Block
    {
        public int Id { get; set; }
        public int Mass { get; set; }
        public bool IsDynamic { get; set; }

        public Vector3 Position { get; set; }
        public IRenderer CurrentRenderer { get; set; }
        public Texture2D Texture { get; set; }
        public Model Mesh { get; set; }

	    protected Block()
	    {
		    
	    }

	    protected Block(Vector3 position)
	    {
		    Position = position;
	    }

	    public virtual void Render(GameTime gameTime)
	    {
		    CurrentRenderer.Render(gameTime, this);
	    }
    }
}
