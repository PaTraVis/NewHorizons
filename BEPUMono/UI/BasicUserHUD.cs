using System;
using System.Linq;
using System.Threading;
using Awesomium.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BEPUMono.UI
{
    public class BasicUserHUD : DrawableGameComponent
    {
        private byte[] _imageBytes;
        private Rectangle _area;
        private Rectangle? _newArea;
        private bool _resizing;
        private readonly SpriteBatch _spriteBatch;
        private Texture2D WebViewTexture { get; set; }
        private SynchronizationContext _awesomiumContext;
	    public WebView WebView { get; set; }
        private BitmapSurface Surface { get; set; }
        private MouseState _lastMouseState;
        private MouseState _currentMouseState;
        private Keys[] _lastPressedKeys;
        private Keys[] _currentPressedKeys = new Keys[0];
        private static readonly ManualResetEvent AwesomiumReady = new ManualResetEvent(false);

        public PlanetScreen Screen { get; set; }

        public string CurrentFPS { get; set; }
        public string CurrentActiveObjects { get; set; }

	    private JSObject _jsObject;

        public Rectangle Area
        {
            get { return _area; }
            set
            {
                _newArea = value;
            }
        }

        public BasicUserHUD(PlanetScreen game, Rectangle area)
            : base(game.Game)
        {
            _area = area;
	        Screen = game;
            Screen.GamePlayer.PlayerToolbarIndexChanged += GamePlayerOnPlayerToolbarIndexChanged;

            _spriteBatch = new SpriteBatch(game.Game.GraphicsDevice);

            AwesomiumThread = new Thread(() =>
            {
	            WebCore.Started += (s, e) => {
		                                         _awesomiumContext = SynchronizationContext.Current;
		                                         AwesomiumReady.Set();
	            };

	            WebCore.Run();
            });

            AwesomiumThread.Start();

            WebCore.Initialize(new WebConfig());

            AwesomiumReady.WaitOne();

            _awesomiumContext.Post(state =>
            {
                WebView = WebCore.CreateWebView(_area.Width, _area.Height, WebViewType.Offscreen);

                WebView.IsTransparent = true;
                WebView.CreateSurface += (s, e) =>
                {
                    Surface = new BitmapSurface(_area.Width, _area.Height);
                    e.Surface = Surface;
                };
            }, null);

            //_jsObject = WebView.CreateGlobalJavascriptObject("game");
            //_jsObject["test"] = "Hello World!";
        }

	    public Thread AwesomiumThread { get; set; }

	    public void SetResourceInterceptor(IResourceInterceptor interceptor)
        {
            _awesomiumContext.Post(state =>
            {
                WebCore.ResourceInterceptor = interceptor;
            }, null);
        }

        public void Execute(string method, params object[] args)
        {
            string script = $"viewModel.{method}({string.Join(",", args.Select(x => "\"" + x.ToString() + "\""))})";
            WebView.ExecuteJavascript(script);
        }

	    public void Load()
        {
            LoadContent();
        }

        protected override void LoadContent()
        {
            if (_area.IsEmpty)
            {
                _area = GraphicsDevice.Viewport.Bounds;
                _newArea = GraphicsDevice.Viewport.Bounds;
            }
            WebViewTexture = new Texture2D(Game.GraphicsDevice, _area.Width, _area.Height, false, SurfaceFormat.Color);

            _imageBytes = new byte[_area.Width * 4 * _area.Height];

	        _awesomiumContext.Post(state =>
	        {
                var path = @"file:///C:/Sourcecode/BEPUMono/BEPUMono/bin/Windows/Debug/UI/Screens/HUD.html";
		        //var path = "http://www.google.com";
                WebView.Source = new Uri(path);
		        WebView.DocumentReady += (sender, args) =>
		        {
			        _jsObject = WebView.CreateGlobalJavascriptObject("game");
                    _jsObject.BindAsync("getData", delegate(object o, JavascriptMethodEventArgs eventArgs)
                    {
                        JSObject window = WebView.ExecuteJavascriptWithResult("window");

                        if (window == null)
                            return;

                        using (window)
                        {
                            window.InvokeAsync("setValue", "debug_fps", CurrentFPS);
                            window.InvokeAsync("setValue", "debug_active_obj", CurrentActiveObjects);
                        }
                    });
		        };

	        }, null);
        }

	    protected override void UnloadContent()
	    {
            
            _awesomiumContext.Post(state =>
	    {
		    WebCore.Shutdown();
	    }, null);
            AwesomiumThread.Interrupt();
            AwesomiumThread.Abort();
	    }

	    public override void Update(GameTime gameTime)
	    {
		    var game = Game as NewHorizonGame;
		    CurrentFPS = game.FPS.CurrentFramesPerSecond.ToString();
		    CurrentActiveObjects = $"Active Objects: {Screen.Space.Entities.Count}";

            _awesomiumContext.Post(state =>
            {
                if (_newArea.HasValue && !_resizing && gameTime.TotalGameTime.TotalSeconds > 0.10f)
                {
                    _area = _newArea.Value;
                    if (_area.IsEmpty)
                        _area = GraphicsDevice.Viewport.Bounds;

                    WebView.Resize(_area.Width, _area.Height);
                    WebViewTexture = new Texture2D(Game.GraphicsDevice, _area.Width, _area.Height, false, SurfaceFormat.Color);
                    _imageBytes = new byte[_area.Width * 4 * _area.Height];
                    _resizing = true;

                    _newArea = null;
                }

                _lastMouseState = _currentMouseState;
                _currentMouseState = Mouse.GetState();

                WebView.InjectMouseMove(_currentMouseState.X - _area.X, _currentMouseState.Y - _area.Y);

                if (_currentMouseState.LeftButton == ButtonState.Pressed && _lastMouseState.LeftButton == ButtonState.Released)
                {
                    WebView.InjectMouseDown(MouseButton.Left);
                }
                if (_currentMouseState.LeftButton == ButtonState.Released && _lastMouseState.LeftButton == ButtonState.Pressed)
                {
                    WebView.InjectMouseUp(MouseButton.Left);
                }
                if (_currentMouseState.RightButton == ButtonState.Pressed && _lastMouseState.RightButton == ButtonState.Released)
                {
                    WebView.InjectMouseDown(MouseButton.Right);
                }
                if (_currentMouseState.RightButton == ButtonState.Released && _lastMouseState.RightButton == ButtonState.Pressed)
                {
                    WebView.InjectMouseUp(MouseButton.Right);
                }
                if (_currentMouseState.MiddleButton == ButtonState.Pressed && _lastMouseState.MiddleButton == ButtonState.Released)
                {
                    WebView.InjectMouseDown(MouseButton.Middle);
                }
                if (_currentMouseState.MiddleButton == ButtonState.Released && _lastMouseState.MiddleButton == ButtonState.Pressed)
                {
                    WebView.InjectMouseUp(MouseButton.Middle);
                }

                if (_currentMouseState.ScrollWheelValue != _lastMouseState.ScrollWheelValue)
                {
                    WebView.InjectMouseWheel((_currentMouseState.ScrollWheelValue - _lastMouseState.ScrollWheelValue), 0);
                }

                _lastPressedKeys = _currentPressedKeys;
                _currentPressedKeys = Keyboard.GetState().GetPressedKeys();

                // Key Down
                foreach (var key in _currentPressedKeys)
                {
                    if (!_lastPressedKeys.Contains(key))
                    {
                        WebView.InjectKeyboardEvent(new WebKeyboardEvent()
                        {
                            Type = WebKeyboardEventType.KeyDown,
                            VirtualKeyCode = (VirtualKey)(int)key,
                            NativeKeyCode = (int)key
                        });

                        if ((int)key >= 65 && (int)key <= 90)
                        {
                            WebView.InjectKeyboardEvent(new WebKeyboardEvent()
                            {
                                Type = WebKeyboardEventType.Char,
                                Text = key.ToString().ToLower()
                            });
                        }
                        else if (key == Keys.Space)
                        {
                            WebView.InjectKeyboardEvent(new WebKeyboardEvent()
                            {
                                Type = WebKeyboardEventType.Char,
                                Text = " "
                            });
                        }
                    }
                }

                // Key Up
                foreach (var key in _lastPressedKeys)
                {
                    if (!_currentPressedKeys.Contains(key))
                    {
                        WebView.InjectKeyboardEvent(new WebKeyboardEvent()
                        {
                            Type = WebKeyboardEventType.KeyUp,
                            VirtualKeyCode = (VirtualKey)(int)key,
                            NativeKeyCode = (int)key
                        });
                    }
                }

            }, null);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _awesomiumContext.Post(state =>
            {
                if (Surface != null && Surface.IsDirty && !_resizing)
                {
                    unsafe
                    {
                        // This part saves us from double copying everything.
                        fixed (byte* imagePtr = _imageBytes)
                        {
                            Surface.CopyTo((IntPtr)imagePtr, Surface.Width * 4, 4, true, false);
                        }
                    }
                    WebViewTexture.SetData(_imageBytes);
                }
            }, null);

            Vector2 pos = new Vector2(0, 0);
            _spriteBatch.Begin();
            _spriteBatch.Draw(WebViewTexture, pos, Color.White);
            _spriteBatch.End();
            GraphicsDevice.Textures[0] = null;
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            base.Draw(gameTime);
        }

        public Uri Source
        {
            get
            {
                return WebView.Source;
            }
            set
            {
                _awesomiumContext.Post(state =>
                {
                    WebView.Source = value;
                }, null);
            }
        }

        public void Resize(int width, int height)
        {
            _newArea = new Rectangle(0, 0, width, height);
        }

        private void GamePlayerOnPlayerToolbarIndexChanged(object sender, ToolbarSelectionEventArgs eventArgs)
        {
            UpdateSelectedIndex(eventArgs.SelectedIndex);
        }

        public void UpdateSelectedIndex(int index)
	    {
            _awesomiumContext.Post(state =>
            {
                JSObject window = WebView.ExecuteJavascriptWithResult("window");

                if (window == null)
                    return;

                using (window)
                {
                    window.InvokeAsync("changeSelection", index);
                }
            }, null);
        }
    }
}
