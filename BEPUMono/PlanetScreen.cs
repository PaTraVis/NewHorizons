using System;
using System.Collections.ObjectModel;
using BEPUMono.InputListeners;
using BEPUMono.Physics;
using BEPUMono.UI;
using BEPUphysics;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.UpdateableSystems.ForceFields;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.InputListeners;
using Vector3 = BEPUutilities.Vector3;

namespace BEPUMono
{
    public class PlanetScreen : IScreen
    {
	    public Game Game { get; private set; }
	    private ScreenAwareAssetManager _assetManager;

        public Collection<IDrawableComponent> Components { get; }
	    private SpriteBatch _spriteBatch;

        public Player GamePlayer { get; private set; }

	    public Space Space { get; private set; }

	    private Model _planet;
	    private Model _cube;

	    private Texture2D _background;
	    private Texture2D _planetTex;

        public BasicUserHUD HUD { get; set; }

        public PlanetScreen()
	    {
            Components = new Collection<IDrawableComponent>();
        }

	    public void Initialize(Game game)
	    {
            Game = game;
            _assetManager = new ScreenAwareAssetManager(Game, this);
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            var myGame = game as NewHorizonGame;

            GamePlayer = new Player(myGame);

            HUD = new BasicUserHUD(this, new Rectangle(0, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height))
            {
                DrawOrder = 9999
            };
            Game.Components.Add(HUD);

            myGame.KeyboardListener.KeyReleased += KeyboardListenerOnKeyReleased;
            myGame.KeyboardListener.KeyTyped += KeyboardListenerOnKeyPressed;
            myGame.MouseListener.MouseWheelMoved += MouseListenerOnMouseWheelMoved;
            myGame.MouseListener.MouseUp += MouseListenerOnMouseUp;
        }

	    private void KeyboardListenerOnKeyPressed(object sender, KeyboardEventArgs keyboardEventArgs)
	    {
		    var keyStep = 1000/60f/100;

		    if (keyboardEventArgs.Key == Keys.W)
		    {
			    GamePlayer.Camera.MoveForward(keyStep);
		    }

            if (keyboardEventArgs.Key == Keys.S)
            {
                GamePlayer.Camera.MoveBackward(keyStep);
            }

            if (keyboardEventArgs.Key == Keys.A)
            {
                GamePlayer.Camera.MoveLeft(keyStep);
            }

            if (keyboardEventArgs.Key == Keys.D)
            {
                GamePlayer.Camera.MoveRight(keyStep);
            }
        }

	    private void MouseListenerOnMouseUp(object sender, MouseEventArgs mouseEventArgs)
	    {
		    //if (mouseEventArgs.Button == MouseButton.Left)
		    //{
                var size = 0.1f;
                var shootBox = new Box(GamePlayer.Camera.Position, size, size, size, 1) { LinearVelocity = GamePlayer.Camera.WorldMatrix.Forward * 25 };

                //Add the new box to the simulation.
                Space.Add(shootBox);

                //Add a graphical representation of the box to the drawable game components.
                Game.Components.Add(new EntityModel(shootBox, _cube, Matrix.Identity, Game, GamePlayer));
            //}
	    }

	    private void MouseListenerOnMouseWheelMoved(object sender, MouseEventArgs mouseEventArgs)
	    {
		    if (mouseEventArgs.CurrentState.ScrollWheelValue < mouseEventArgs.PreviousState.ScrollWheelValue)
		    {
			    GamePlayer.DecreasePlayerToolbarSelection();
		    }

            if (mouseEventArgs.CurrentState.ScrollWheelValue > mouseEventArgs.PreviousState.ScrollWheelValue)
            {
                GamePlayer.IncreasePlayerToolbarSelection();
            }
        }

	    private void KeyboardListenerOnKeyReleased(object sender, KeyboardEventArgs keyboardEventArgs)
	    {
		    if (keyboardEventArgs.Key == Keys.D1)
		    {
			    GamePlayer.SetSelectedToolbarIndex(1);
		    }

            if (keyboardEventArgs.Key == Keys.D2)
            {
                GamePlayer.SetSelectedToolbarIndex(2);
            }

            if (keyboardEventArgs.Key == Keys.D3)
            {
                GamePlayer.SetSelectedToolbarIndex(3);
            }

            if (keyboardEventArgs.Key == Keys.D4)
            {
                GamePlayer.SetSelectedToolbarIndex(4);
            }

            if (keyboardEventArgs.Key == Keys.D5)
            {
                GamePlayer.SetSelectedToolbarIndex(5);
            }

            if (keyboardEventArgs.Key == Keys.D6)
            {
                GamePlayer.SetSelectedToolbarIndex(6);
            }

            if (keyboardEventArgs.Key == Keys.D7)
            {
                GamePlayer.SetSelectedToolbarIndex(7);
            }

            if (keyboardEventArgs.Key == Keys.D8)
            {
                GamePlayer.SetSelectedToolbarIndex(8);
            }

		    if (keyboardEventArgs.Key == Keys.D9)
		    {
			    GamePlayer.SetSelectedToolbarIndex(9);
		    }
	    }

        public void LoadContent()
	    {
            //Load Textures
            _planet = _assetManager.CreateAsset<Model>("planet,", "sphere");
            _cube = _assetManager.CreateAsset<Model>("cube", "cube");

            _background = _assetManager.CreateAsset<Texture2D>("background", "Textures\\space");
            _planetTex = _assetManager.CreateAsset<Texture2D>("planetTexture", "Textures\\planet");

            Space = new Space { ForceUpdater = { Gravity = new Vector3(0, -9.81f, 0) } };

            var spaceObject = new Box(Vector3.Zero, 30, 1, 30);
            Space.Add(spaceObject);

            var sphere = new Sphere(new Vector3(0, 30, 0), 5);
            //sphere.BecomeKinematic();
            Space.Add(sphere);

            var field = new GravitationField(new InfiniteForceFieldShape(), sphere.Position, 66730 / 2f, 100);
            Space.Add(field);

            //Go through the list of entities in the Space and create a graphical representation for them.
            foreach (Entity e in Space.Entities)
            {
                if (e.GetType() == typeof(Box))
                {
                    var box = e as Box;
                    if (box != null) //This won't create any graphics for an entity that isn't a box since the model being used is a box.
                    {
                        Matrix scaling = Matrix.CreateScale(box.Width, box.Height, box.Length); //Since the cube model is 1x1x1, it needs to be scaled to match the size of each individual box.
                                                                                                //Add the drawable game component for this entity to the game.
                        Game.Components.Add(new EntityModel(e, _cube, scaling, Game, GamePlayer));
                    }
                }

                if (e.GetType() == typeof(Sphere))
                {
                    var eSphere = e as Sphere;
                    if (eSphere != null)
                    {
                        var size = 5f;
                        var scaling = Matrix.CreateScale(MathConverter.Convert(new Vector3(size, size, size)));
                        Game.Components.Add(new EntityModel(e, _planet, scaling, Game, GamePlayer));
                    }
                }

            }
        }

	    public void UnloadContent()
	    {
            _assetManager.Dispose();
            HUD.AwesomiumThread.Abort();
        }

	    public void Draw(GameTime gameTime)
	    {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_background, Vector2.Zero, new Rectangle(0, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height), Color.White);
            _spriteBatch.End();

            Game.GraphicsDevice.BlendState = BlendState.Opaque;
            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        public void Update(GameTime gameTime)
	    {
            GamePlayer.Camera.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            Space.Update();

            //foreach (var component in Components)
            //{
            //    component.Update(gameTime);
            // }
        }
    }
}
