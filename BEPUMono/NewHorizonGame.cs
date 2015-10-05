using System.Collections.ObjectModel;
using BEPUMono.InputListeners;
using BEPUMono.UI;
using BEPUMono.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BEPUMono
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class NewHorizonGame : Game
    {
        public bool ShowDebugInfo { get; set; }
	    public InputListenerManager InputManager { get; private set; }
        public KeyboardListener KeyboardListener { get; private set; }
        public MouseListener MouseListener { get; private set; }

        public FramesPerSecondCounter FPS { get; private set; }

        GraphicsDeviceManager graphics;

        public IScreen ActiveScreen { get; set; }

	    public BasicUserHUD HUD { get; private set; }

        /// <summary>
        /// Controls the viewpoint and how the user can see the world.
        /// </summary>
        public Camera Camera;

        /// <summary>
        /// Contains the latest snapshot of the mouse's input state.
        /// </summary>
        public MouseState MouseState;


        public NewHorizonGame()
        {
	        graphics = new GraphicsDeviceManager(this)
	        {
		        PreferredBackBufferWidth = 1280,
		        PreferredBackBufferHeight = 720,
                SynchronizeWithVerticalRetrace = true
	        };
	        IsFixedTimeStep = true;
	        Content.RootDirectory = "Content";
	        Window.Title = "Mein kleiner Spielplatz!";

            ActiveScreen = new PlanetScreen();
        }

	    protected override void Initialize()
	    {
		    InputManager = new InputListenerManager();

		    KeyboardListener = InputManager.AddListener<KeyboardListener>();
		    MouseListener = InputManager.AddListener<MouseListener>();
            FPS = new FramesPerSecondCounter();

            KeyboardListener.KeyReleased += KeyboardListenerOnKeyReleased;
            MouseListener.MouseMoved += MouseListenerOnMouseMoved;

		    ShowDebugInfo = false;

            ActiveScreen?.Initialize(this);
		    base.Initialize();
	    }

	    private void MouseListenerOnMouseMoved(object sender, MouseEventArgs mouseEventArgs)
	    {
		    MouseState = mouseEventArgs.CurrentState;
	    }

	    private void KeyboardListenerOnKeyReleased(object sender, KeyboardEventArgs keyboardEventArgs)
	    {
		    if (keyboardEventArgs.Key == Keys.F3)
		    {
			    ShowDebugInfo = !ShowDebugInfo;
		    }
		    if (keyboardEventArgs.Key == Keys.Escape)
		    {
			    Exit();
		    }
	    }

	    /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            ActiveScreen?.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            ActiveScreen?.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardListener.Update(gameTime);
            MouseListener.Update(gameTime);

            ActiveScreen?.Update(gameTime);
            FPS.Update(gameTime);
            base.Update(gameTime);
        }


	    /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            ActiveScreen?.Draw(gameTime);

            base.Draw(gameTime);
        }
    }

	public interface IScreen
	{
        Collection<IDrawableComponent> Components { get; }

		void Initialize(Game game);
		void LoadContent();
		void UnloadContent();

		void Draw(GameTime gameTime);
		void Update(GameTime gameTime);
	}

	public interface IDrawableComponent
	{
		void Update(GameTime gameTime);
		void Draw(GameTime gameTime);
	}
}
