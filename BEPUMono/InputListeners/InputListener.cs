using Microsoft.Xna.Framework;

namespace BEPUMono.InputListeners
{
    public abstract class InputListener
    {
        protected InputListener()
        {
        }

        internal abstract void Update(GameTime gameTime);
    }
}