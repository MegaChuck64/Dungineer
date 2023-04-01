using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonogameUI;
using Myra.Graphics2D.UI;
using System.Collections.Generic;

namespace TestGameCode
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private List<UIElement> entered = new();
        //private Dictionary<string, SelectItem> selected = new();

        private MouseState mouseState;
        private MouseState lastMouseState;
        private KeyboardState keyState;
        private KeyboardState lastKeyState;

        private Desktop _desktop;
        private List<Entity> entities = new List<Entity>();

        private Texture2D square;
        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            square = CreateRectangleTexture(32, 32, Color.White);
            var testEnt = new Entity()
                .With(new UIElement
                {
                    Color = new Color(60, 60, 100),
                    Size = new Point(200, 200),
                    Position = new Point(100, 100),
                });
            entities.Add(testEnt);

            Myra.MyraEnvironment.Platform = new MGPlatform(GraphicsDevice);
            
            var grid = new Grid
            {
                RowSpacing = 8,
                ColumnSpacing = 8
            };
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            var helloWorld = new Label
            {
                Id = "label",
                Text = "Hello World!"
            };

            grid.Widgets.Add(helloWorld);

            var combo = new ComboBox
            {
                GridColumn = 1,
                GridRow = 0
            };

            combo.Items.Add(new ListItem("Red", FontStashSharp.FSColor.Red));
            combo.Items.Add(new ListItem("Green", FontStashSharp.FSColor.Green));
            combo.Items.Add(new ListItem("Blue", FontStashSharp.FSColor.Blue));
            grid.Widgets.Add(combo);

            // Button
            var button = new TextButton
            {
                GridColumn = 0,
                GridRow = 1,
                Text = "Show"
            };

            button.Click += (s, a) =>
            {
                var messageBox = Dialog.CreateMessageBox("Message", "Some message!");
                messageBox.ShowModal(_desktop);
            };

            grid.Widgets.Add(button);

            // Spin button
            var spinButton = new SpinButton
            {
                GridColumn = 1,
                GridRow = 1,
                Width = 100,
                Nullable = true
            };
            grid.Widgets.Add(spinButton);

            // Add it to the desktop
            _desktop = new Desktop
            {
                Root = grid
            };

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            foreach (var entity in entities)
            {
                var ui = entity.GetComponent<UIElement>();

                if (ui == null || !ui.IsActive)
                    continue;

                var bounds = ui.Bounds;

                if (bounds.Contains(mouseState.Position))
                {
                    if (!entered.Contains(ui))
                    {
                        entered.Add(ui);
                        ui.OnMouseEnter?.Invoke();
                    }

                    if (Input.WasPressed(MouseButton.Left, mouseState, lastMouseState))
                    {
                        ui.OnMousePressed?.Invoke(MouseButton.Left);
                    }
                    if (Input.WasReleased(MouseButton.Left, mouseState, lastMouseState))
                    {
                        ui.OnMouseReleased?.Invoke(MouseButton.Left);
                    }

                }
                else
                {
                    if (entered.Contains(ui))
                    {
                        entered.Remove(ui);
                        ui.OnMouseLeave?.Invoke();
                    }
                }

            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            foreach (var entity in entities)
            {
                var ui = entity.GetComponent<UIElement>();

                if (ui != null && ui.IsActive)
                {
                    _spriteBatch.Begin();
                    DrawUIElement(ui);
                    _spriteBatch.End();
                }


            }

            _desktop.Render();

            base.Draw(gameTime);
        }


        private void DrawUIElement(UIElement ui) =>
            _spriteBatch.Draw(square, ui.Bounds, ui.Color);
        

        private Texture2D CreateRectangleTexture(int width, int height, Color color)
        {
            var texture = new Texture2D(GraphicsDevice, width, height);
            var data = new Color[width * height];

            for (var i = 0; i < data.Length; ++i)
                data[i] = color;

            texture.SetData(data);

            return texture;
        }
    }
    
}