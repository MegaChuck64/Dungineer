using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Engine
{
    //spell system
    //encoded string of colored characters, we create a system of arbritrarily decoding it to different properties of a spell
    public abstract class BaseGame : Game
    {
        private GraphicsDeviceManager _graphics;
        public Color BackgroundColor { get; set; } = Color.Black;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Random Rand { get; set; }
        public bool Debug { get; set; } = false;
        public List<BaseSystem> Systems { get; private set; }
        public BaseGame(int? seed = null, int width = 1300, int height = 900)
        {
            Width = width;
            Height = height;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Systems = new List<BaseSystem>();
            
            Rand = seed == null ? new Random(1) : new Random(seed.Value);            
        }

        protected override void Initialize()
        {

            _graphics.PreferredBackBufferWidth = Width;
            _graphics.PreferredBackBufferHeight = Height;
            
            IsFixedTimeStep = true;
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
            Load(Content);
        }

        protected override void Update(GameTime gameTime)
        {

            base.Update(gameTime);

            foreach (var sys in Systems)
            {
                sys.Update(gameTime, SceneManager.Entities);
            }

            OnUpdate(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(BackgroundColor);

            base.Draw(gameTime);

            foreach (var sys in Systems)
            {
                sys.Draw(gameTime, SceneManager.Entities);
            }

            OnDraw(gameTime);
        }

        public abstract void Load(ContentManager content);
        public abstract void Init();
        public abstract void OnUpdate(GameTime gameTime);
        public abstract void OnDraw(GameTime gameTime);
    }
}
