using Dungineer.Components.GameWorld;
using Dungineer.Components.UI;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungineer.Systems;

public class UISystem : BaseSystem
{

    private List<UIElement> entered = new();
    private Dictionary<string, SelectItem> selected = new();

    private MouseState mouseState;
    private MouseState lastMouseState;
    private KeyboardState keyState;
    private KeyboardState lastKeyState;
    
    private SpriteBatch sb;

    private Dictionary<string, Texture2D> textures = new ();
    private Dictionary<string, SpriteFont> fonts = new();

    private int frameRate = 0;
    private int frameCounter = 0;
    private TimeSpan elapsedTime = TimeSpan.Zero;
    private Point MouseTilePosition =>
    new((mouseState.X - (Game.Width / 5)) / Settings.TileSize,
        mouseState.Y / Settings.TileSize);

    public UISystem(BaseGame game) : base(game)
    {
        sb = new SpriteBatch(game.GraphicsDevice);
        LoadTextures(game);
        LoadFonts(game);

    }
    private void LoadFonts(BaseGame game)
    {
        fonts.Add("consolas_12",
            ContentLoader.LoadFont("consolas_12", game.Content));

        fonts.Add("consolas_14",
            ContentLoader.LoadFont("consolas_14", game.Content));

        fonts.Add("consolas_22",
            ContentLoader.LoadFont("consolas_22", game.Content));
    }

    private void LoadTextures(BaseGame game)
    {

        textures.Add(
            "WizardPortraits_512",
            ContentLoader.LoadTexture("WizardPortraits_512", game.Content));


        var pxl = new Texture2D(game.GraphicsDevice, 1, 1);
        pxl.SetData(new Color[] { Color.White });
        textures.Add("_pixel", pxl);

        textures.Add(
            "cursor_16",
            ContentLoader.LoadTexture("cursor_16", game.Content));

        textures.Add(
            "symbols_32",
            ContentLoader.LoadTexture("symbols_32", game.Content));

        textures.Add(
            "robes_32",
            ContentLoader.LoadTexture("robes_32", game.Content));

        textures.Add(
            "treasure_32",
            ContentLoader.LoadTexture("treasure_32", game.Content));
    }

    
    public override void Update(GameTime gameTime, IEnumerable<Entity> entities)
    {

        elapsedTime += gameTime.ElapsedGameTime;

        if (elapsedTime > TimeSpan.FromSeconds(1))
        {
            elapsedTime -= TimeSpan.FromSeconds(1);
            frameRate = frameCounter;
            frameCounter = 0;
        }

        lastMouseState = mouseState;
        mouseState = Mouse.GetState();

        lastKeyState = keyState;
        keyState = Keyboard.GetState();


        foreach (var entity in entities)
        {
            var ui = entity.GetComponent<UIElement>();

            if (ui == null || !ui.IsActive)
                continue;

            var bounds = ui.Bounds;

            if (entity.HasTag("Cursor"))
            {
                ui.Position = mouseState.Position;
            }


            if (bounds.Contains(mouseState.Position))
            {
                if (!entered.Contains(ui))
                {
                    entered.Add(ui);
                    ui.OnMouseEnter?.Invoke();
                }

                if (WasPressed(MouseButton.Left))
                {
                    ui.OnMousePressed?.Invoke(MouseButton.Left);
                }
                if (WasReleased(MouseButton.Left))
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
    }

    public override void Draw(GameTime gameTime, IEnumerable<Entity> entities)
    {
        sb.Begin(
          sortMode: SpriteSortMode.FrontToBack,
          blendState: BlendState.NonPremultiplied,
          samplerState: SamplerState.PointClamp,
          depthStencilState: DepthStencilState.DepthRead,
          rasterizerState: RasterizerState.CullCounterClockwise,
          effect: null,
          transformMatrix: null); //camera here todo

        foreach (var entity in entities)
        {
            var ui = entity.GetComponent<UIElement>();

            if (ui != null && ui.IsActive)
            { 
                var bounds = ui.Bounds;

                foreach (var img in entity.GetComponents<Image>())
                {
                    //by default tint is the image tint
                    var tint = img.Tint;

                    if (entity.GetComponent<SelectItem>() is SelectItem selectItem)
                    {
                        //if is a select item, image tint gets overriden
                        HandleSelection(selectItem, bounds, out tint);
                    }

                    DrawImage(img, ui, tint);
                }

                foreach (var txt in entity.GetComponents<TextBox>())
                {
                    DrawText(txt, ui);
                }
            }

            if (entity.GetComponent<MapObject>() is MapObject mapObj)
            {
                if (entity.HasTag("Player"))
                {
                    var stats = entity.GetComponent<CreatureStats>();
                    DrawPlayerStats(stats);
                }
                else if (mapObj.MapX == MouseTilePosition.X && mapObj.MapY == MouseTilePosition.Y)
                {
                    
                    if (entity.GetComponent<CreatureStats>() is CreatureStats stats)
                        DrawItemStats(stats, mapObj);
                }
            }
        }

        frameCounter++;
        sb.DrawString(
            fonts["consolas_12"], 
            $"FPS: {frameRate}", 
            new Vector2(4, Game.Height - 20), 
            Color.Yellow, 
            0f, 
            Vector2.Zero, 
            1f, 
            SpriteEffects.None, 
            0.9f);

        if (SceneManager.CurrentScene == "Play")
            sb.DrawString(
                fonts["consolas_12"],
                $"Left click to move towards tile \r\n" +
                $"1 to aim and click to attack",
                new Vector2(8, Game.Height - 200),
                Color.Tan,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.9f);


        if (SceneManager.CurrentScene == "Play" && !SceneManager.Entities.Any(t=>t.HasTag("Player")))
        {
            sb.DrawString(
                fonts["consolas_22"],
                $"Game Over",
                new Vector2(Game.Width/2 - 62, Game.Height/2),
                Color.Red,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.9f);
        }
        sb.End();
    }


    private void HandleSelection(SelectItem selectItem, Rectangle bounds, out Color tint)
    {
        //if we have a select item component, override img tint with default color
        tint = selectItem.DefaultColor;

        //handle selected item
        if (selected.ContainsKey(selectItem.SelectionGroup) && selected[selectItem.SelectionGroup] == selectItem)
        {
            tint = selectItem.SelectedColor;
            selectItem.Selected = true;
        }
        else
        {
            selectItem.Selected = false;
        }

        //hovering
        if (bounds.Contains(mouseState.Position))
        {
            tint = selectItem.HoverColor;

            //select item on pressed
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                tint = selectItem.PressedColor;
                selected[selectItem.SelectionGroup] = selectItem;
                selectItem.Selected = true;
            }
        }
    }


    private void DrawText(TextBox text, UIElement ui)
    {
        var font = fonts[text.FontName];
        var bounds = ui.Bounds;

        var fontSize = font.MeasureString(text.Text);
        var pos = new Vector2(bounds.X + (bounds.Width / 2) - (fontSize.X / 2), bounds.Y + (bounds.Height / 2) - (fontSize.Y / 2));
        pos += text.Offset;
        sb.DrawString(font, text.Text, pos, text.TextColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, text.Layer);
    }
    private void DrawImage(Image image, UIElement ui, Color tint)
    {
        var txtr = textures[image.TextureName];
        var bounds = new Rectangle(ui.Position + image.Position, ui.Size * image.Size);
        sb.Draw(
            txtr, 
            bounds, 
            image.Source, 
            tint, 
            0f, 
            Vector2.Zero, 
            SpriteEffects.None, 
            image.Layer);
    }

    private void DrawPlayerStats(CreatureStats stats)
    {
        var font = fonts["consolas_14"];

        //health
        var healthTexture = textures["symbols_32"];
        var healthSource = new Rectangle(32, 0, 32, 32);
        var healthBounds = new Rectangle(16, 32 * 3, 32, 32);

        var healthStr = $"{stats.Health}/{stats.MaxHealth}";
        var healthStrSize = font.MeasureString(healthStr);

        sb.Draw(healthTexture, healthBounds, healthSource, Color.White);
        sb.DrawString(font, healthStr, new Vector2(healthBounds.X + 32 + 8, healthBounds.Y + (healthStrSize.Y/2)), Color.White);

        //money
        var moneyTexture = textures["treasure_32"];
        var moneySource = new Rectangle(0, 0, 32, 32);
        var moneyBounds = new Rectangle(16, 32 * 4 + 8, 32, 32);

        var moneyStr = stats.Money.ToString();
        var moneyStrSize = font.MeasureString(healthStr);

        sb.Draw(moneyTexture, moneyBounds, moneySource, Color.White);
        sb.DrawString(font, moneyStr, new Vector2(moneyBounds.X + 32 + 8, moneyBounds.Y + (moneyStrSize.Y / 2)), Color.White);
    }

    private void DrawItemStats(CreatureStats stats, MapObject mapObj)
    {
        var font = fonts["consolas_14"];

        //health
        var healthStr = $"{stats.Health}/{stats.MaxHealth}";
        var healthStrSize = font.MeasureString(healthStr);

        var healthTexture = textures["symbols_32"];
        var healthSource = new Rectangle(32, 0, 32, 32);
        var healthBounds = new Rectangle(Game.Width - 16 - (int)healthStrSize.X - 32 - 16, 32 * 3, 32, 32);

        var itemInfo = Settings.MapObjectAtlas[mapObj.Type];

        sb.Draw(healthTexture, healthBounds, healthSource, Color.White);
        sb.DrawString(font, healthStr, new Vector2(healthBounds.X + 32 + 8, healthBounds.Y + (healthStrSize.Y / 2)), Color.White);

        sb.DrawString(font, itemInfo.Name, new Vector2(healthBounds.X + 32, healthBounds.Y - healthStrSize.Y), Color.White);

        ////money
        //var moneyTexture = textures["treasure_32"];
        //var moneySource = new Rectangle(0, 0, 32, 32);
        //var moneyBounds = new Rectangle(16, 32 * 4 + 8, 32, 32);

        //var moneyStr = stats.Money.ToString();
        //var moneyStrSize = font.MeasureString(healthStr);

        //sb.Draw(moneyTexture, moneyBounds, moneySource, Color.White);
        //sb.DrawString(font, moneyStr, new Vector2(moneyBounds.X + 32 + 8, moneyBounds.Y + (moneyStrSize.Y / 2)), Color.White);
    }


    private bool WasPressed(MouseButton mb) => mb switch
    {
        MouseButton.Left => mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released,
        MouseButton.Middle => mouseState.MiddleButton == ButtonState.Pressed && lastMouseState.MiddleButton == ButtonState.Released,
        MouseButton.Right => mouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton == ButtonState.Released,
        _ => false,
    };

    private bool WasReleased(MouseButton mb) => mb switch
    {
        MouseButton.Left => mouseState.LeftButton == ButtonState.Released && lastMouseState.LeftButton == ButtonState.Pressed,
        MouseButton.Middle => mouseState.MiddleButton == ButtonState.Released && lastMouseState.MiddleButton == ButtonState.Pressed,
        MouseButton.Right => mouseState.RightButton == ButtonState.Released && lastMouseState.RightButton == ButtonState.Pressed,
        _ => false,
    };


}



public enum MouseButton
{
    Left,
    Middle,
    Right
}