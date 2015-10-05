using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BEPUMono
{
    public class Profiler : DrawableGameComponent
    {
        private const int XStart = 125;
        private const int YStart = 30;
        private const float Adjustment = .001f;

        private readonly Dictionary<String, Stopwatch> _timers;
        private readonly SpriteBatch _spriteBatch;
        private readonly Texture2D _blank;
        private readonly SpriteFont _font;

        #region Frame Counting
        public long TotalFrames { get; private set; }
        public float TotalSeconds { get; private set; }
        public float AverageFramesPerSecond { get; private set; }
        public float CurrentFramesPerSecond { get; private set; }

        public const int MaximumSamples = 100;

        private readonly Queue<float> _sampleBuffer = new Queue<float>();
        #endregion

        public Profiler(Game game) : base(game)
        {
            _spriteBatch = new SpriteBatch(game.GraphicsDevice);
            _blank = game.Content.Load<Texture2D>("Textures\\Blank");
            _font = game.Content.Load<SpriteFont>("Fonts\\Console");
            _timers = new Dictionary<string, Stopwatch>();
        }

        /// <summary>
        /// Starts a timer.  If it doesn't exist, then the timer is created.
        /// The timers continue from their last point.  Timers are only reset
        /// during the draw phase or if explicitly reset with a call to
        /// Reset().  Every call to Start() must be accompanied by an 
        /// appropriate End() for accurate results.
        /// </summary>
        /// <param name="timerId">The name of the timer.  The name of the timer
        /// is what will be displayed next to the bar, when it is drawn to the
        /// screen.</param>
        public void Start(string timerId)
        {
            if (!_timers.ContainsKey(timerId))
            {
                _timers.Add(timerId, new Stopwatch());
            }
            _timers[timerId].Start();
        }

        /// <summary>
        /// Resets a timer.  Can be used if a timer needs to be reset before
        /// the end the frame.
        /// </summary>
        /// <param name="timerId">The name of the timer.</param>
        public void Reset(string timerId)
        {
            _timers[timerId].Reset();
        }

        /// <summary>
        /// Stops a timer.  You have to ensure the timer ID is identical to a previously
        /// called Start().
        /// </summary>
        /// <param name="timerId">The name of the timer.</param>
        public void Stop(string timerId)
        {
            _timers[timerId].Stop();
        }

        public override void Update(GameTime gameTime)
        {
            var deltaTicks = (float)gameTime.ElapsedGameTime.TotalSeconds;
            CurrentFramesPerSecond = 1.0f / deltaTicks;
            _sampleBuffer.Enqueue(CurrentFramesPerSecond);

            if (_sampleBuffer.Count > MaximumSamples)
            {
                _sampleBuffer.Dequeue();
                AverageFramesPerSecond = _sampleBuffer.Average(i => i);
            }
            else
            {
                AverageFramesPerSecond = CurrentFramesPerSecond;
            }

            TotalFrames++;
            TotalSeconds += deltaTicks;
        }

        public override void Draw(GameTime gameTime)
        {
            var game = Game as NewHorizonGame;
            //var fpsText = $"{Math.Round(AverageFramesPerSecond, 0)}";
            //var objText = $"Active Objects: {game.Space.Entities.Count}";

            //game.HUD.CurrentFPS = fpsText;
	        //game.HUD.CurrentActiveObjects = objText;

            /*
            if (game.ShowDebugInfo)
            {
                //Show FPS

                int currentY = YStart;

                _spriteBatch.Begin();
                _spriteBatch.DrawString(_font, fpsText, new Vector2(5, 2), Color.GreenYellow);

                foreach (var timer in _timers.Keys)
                {
                    _spriteBatch.DrawString(_font, timer, new Vector2(XStart - 5 - _font.MeasureString(timer).X, currentY - 5), Color.Yellow);
                    _spriteBatch.Draw(_blank, new Rectangle(XStart, currentY, (int)(_timers[timer].ElapsedTicks * Adjustment), 18), Color.Yellow);
                    _timers[timer].Reset();
                    currentY += 20;
                }
                //Show Active Objects
	            var objText = $"Active Objects: {game.Space.Entities.Count}";
                _spriteBatch.DrawString(_font, objText, new Vector2(XStart + 50 - 5 - _font.MeasureString(objText).X, currentY - 5 + 25), Color.Yellow);

                _spriteBatch.End();
                GraphicsDevice.BlendState = BlendState.Opaque;
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            }
            */
            base.Draw(gameTime);
        }
    }
}
