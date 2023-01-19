using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System;

namespace Engine
{
    public abstract class BaseGame : Game
    {
        private GraphicsDeviceManager _graphics;
        public SpriteBatch SpriteBatch { get; set; }
        public KeyboardStateExtended KeyState { get; private set; }
        public MouseStateExtended MouseState { get; private set; }
        public Color BackgroundColor { get; set; } = Color.Black;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public FastRandom Rand { get; set; }
        public bool Debug { get; set; } = false;
        public BaseGame(int? seed = null, int width = 1300, int height = 900)
        {
            Width = width;
            Height = height;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Rand = seed == null ? new FastRandom() : new FastRandom(seed.Value);            
        }

        protected override void Initialize()
        {

            _graphics.PreferredBackBufferWidth = Width;
            _graphics.PreferredBackBufferHeight = Height;
            
            IsFixedTimeStep = true;
            IsMouseVisible = false;
            _graphics.ApplyChanges();

            Init();


            base.Initialize();
        }

        public void ToggleFullscreen()
        {
            if (_graphics.PreferredBackBufferHeight == GraphicsDevice.DisplayMode.Height && 
                _graphics.PreferredBackBufferWidth == GraphicsDevice.DisplayMode.Width)
            {
                _graphics.PreferredBackBufferWidth = Width;
                _graphics.PreferredBackBufferHeight = Height;
            }
            else
            {
                _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width - 100;
                _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height - 100;
            }
            
            _graphics.ApplyChanges();
        }


        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Load(Content);

        }

        protected override void Update(GameTime gameTime)
        {
            KeyState = KeyboardExtended.GetState();
            MouseState = MouseExtended.GetState();
  
            base.Update(gameTime);

            OnUpdate(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(BackgroundColor);

            base.Draw(gameTime);

            OnDraw(gameTime);
        }

        public abstract void Load(ContentManager content);
        public abstract void Init();
        public abstract void OnUpdate(GameTime gameTime);
        public abstract void OnDraw(GameTime gameTime);
    }
}
