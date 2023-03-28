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

                if (entity.GetComponent<Terminal>() is Terminal terminal)
                {
                    DrawTerminal(terminal, ui);
                }
            }

            if (entity.GetComponent<MapObject>() is MapObject mapObj)
            {
                if (entity.HasTag("Player"))
                {
                    if (entity.GetComponent<CreatureStats>() is CreatureStats stats)
                        DrawCreatureStats(stats, new Point(16, 16));//DrawPlayerStats(stats);

                    if (entity.GetComponent<SpellBook>() is SpellBook spellBook)
                        DrawSpellBook(spellBook, new Point(16, Game.Height / 4));//DrawPlayerSpellBook(spellBook);

                    if (entity.GetComponent<EffectController>() is EffectController effectController)
                        DrawEffects(effectController, new Point(16, Game.Height / 2));//DrawPlayerEffects(effectController);
                }
                else if (mapObj.MapX == MouseTilePosition.X && mapObj.MapY == MouseTilePosition.Y)
                {
                    var info = Settings.MapObjectAtlas[mapObj.Type];
                    if (entity.GetComponent<CreatureStats>() is CreatureStats stats)
                        DrawCreatureStats(stats, new Point(Game.Width - (Game.Width / 5) + 16, 16), false, info.Name);//DrawItemStats(stats, mapObj);

                    if (entity.GetComponent<SpellBook>() is SpellBook spellBook)
                        DrawSpellBook(spellBook, new Point(Game.Width - (Game.Width / 5) + 16, Game.Height/4));

                    if (entity.GetComponent<EffectController>() is EffectController effectController)
                        DrawEffects(effectController, new Point(Game.Width - (Game.Width / 5) + 16, Game.Height / 2));
                }
            }
        }

        //fps
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

        //instructions
        if (SceneManager.CurrentScene == "Play")
            sb.DrawString(
                fonts["consolas_12"],
                $"Left click to move towards tile \r\n" +
                $"Select number of spell to aim and left click to attack",
                new Vector2(8, Game.Height - 200),
                Color.Tan,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.9f);

        //game over
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

    private void DrawTerminal(Terminal terminal, UIElement ui)
    {
        var bounds = ui.Bounds;


        sb.Draw(Settings.TextureAtlas["_pixel"], bounds, Color.DarkBlue);
    }


    private void DrawText(TextBox text, UIElement ui)
    {
        var font = fonts[text.FontName];
        var bounds = ui.Bounds;

        var fontSize = font.MeasureString(text.Text);
        var pos = new Vector2(bounds.X + ((bounds.Width / 2) - (fontSize.X / 2)), bounds.Y + ((bounds.Height / 2) - (fontSize.Y / 2)));
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

    
    private void DrawCreatureStats(CreatureStats stats, Point offset, bool showMoney = true, string name = null)
    {
        var font = fonts["consolas_14"];

        if (!string.IsNullOrWhiteSpace(name))
        {
            sb.DrawString(font, name, new Vector2(offset.X, offset.Y), Color.White);
            offset.Y += 20;
        }
        
        //health
        var healthTexture = textures["symbols_32"];
        var healthSource = new Rectangle(32, 0, 32, 32);
        var healthBounds = new Rectangle(offset.X, offset.Y, 32, 32);

        var healthStr = $"{stats.Health}/{stats.MaxHealth}";
        var healthStrSize = font.MeasureString(healthStr);

        sb.Draw(healthTexture, healthBounds, healthSource, Color.White);
        sb.DrawString(font, healthStr, new Vector2(healthBounds.X + 32 + 8, healthBounds.Y + (healthStrSize.Y / 2)), Color.White);

        //mana
        var manaTexture = textures["symbols_32"];
        var manaSource = new Rectangle(64, 0, 32, 32);
        var manaBounds = new Rectangle(offset.X, offset.Y + 32 + 8, 32, 32);

        var manaStr = $"{stats.Mana}/{stats.MaxMana}";
        var manaStrSize = font.MeasureString(manaStr);

        sb.Draw(manaTexture, manaBounds, manaSource, Color.White);
        sb.DrawString(font, manaStr, new Vector2(manaBounds.X + 32 + 8, manaBounds.Y + (manaStrSize.Y / 2)), Color.White);

        if (showMoney)
        {
            //money
            var moneyTexture = textures["treasure_32"];
            var moneySource = new Rectangle(0, 0, 32, 32);
            var moneyBounds = new Rectangle(offset.X, offset.Y + 32 * 2 + 8, 32, 32);

            var moneyStr = stats.Money.ToString();
            var moneyStrSize = font.MeasureString(healthStr);

            sb.Draw(moneyTexture, moneyBounds, moneySource, Color.White);
            sb.DrawString(font, moneyStr, new Vector2(moneyBounds.X + 32 + 8, moneyBounds.Y + (moneyStrSize.Y / 2)), Color.White);
        }
    }

    private void DrawSpellBook(SpellBook spellBook, Point offset)
    {
        var font = fonts["consolas_14"];

        var yOffset = 0;
        foreach (var spell in spellBook.Spells)
        {
            var info = Settings.SpellAtlas[spell.GetSpellType()];
            var txtr = Settings.TextureAtlas[info.TextureName];
            var src = info.Source;
            var bnds = new Rectangle(offset.X, offset.Y + 32 * (yOffset) + 8, 32, 32);

            var str = info.Name;
            var sze = font.MeasureString(str);

            sb.Draw(txtr, bnds, src, Color.White);
            sb.DrawString(
                font,
                str,
                new Vector2(bnds.X + 32 + 8, bnds.Y + (sze.Y / 2)),
                 spellBook.selectedSpell == yOffset ? Color.Green : Color.White);

            yOffset++;
        }
    }

    private void DrawEffects(EffectController effectController, Point offset)
    {
        var font = fonts["consolas_14"];

        var yOffset = 0;
        foreach (var effect in effectController.Effects)
        {
            if (effect.TurnsLeft <= 0) continue;
            
            var info = Settings.EffectAtlas[effect.GetEffectType()];
            var txtr = Settings.TextureAtlas[info.TextureName];
            var src = info.Source;
            var bnds = new Rectangle(offset.X, offset.Y + 32 * (yOffset++) + 8, 32, 32);

            var str = $"{info.Name}({effect.TurnsLeft})";
            var sze = font.MeasureString(str);

            sb.Draw(txtr, bnds, src, Color.White);
            sb.DrawString(font, str, new Vector2(bnds.X + 32 + 8, bnds.Y + (sze.Y / 2)), Color.White);
        }
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