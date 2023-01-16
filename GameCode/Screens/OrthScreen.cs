using Engine;
using GameCode.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System.Collections.Generic;

namespace GameCode.Screens;

public class OrthScreen : BaseScreen
{
    FPSCounter fpsCounter;
    TileMap map;
    Sprite tileSelector;
    SidebarMenu tileInfoBox;
    bool showTileInfo = false;  
    readonly float camSpeed = 128f;

    public OrthScreen(MainGame game) : base(game, "consolas_22") { }

    public static List<string> PrintTileInfo(Tile tile)
    {
        var messages = new List<string>
        {
            $"tile   {tile.TileType}",
            $"solid  {tile.HasCollider}"
        };

        return messages;
    }
    public override void LoadContent()
    {
        base.LoadContent();
        
        fpsCounter = new FPSCounter(BGame as MainGame) { Font = Font };
        map = new TileMap(BGame, Camera);
        EntityManager.AddEntity(map);

        tileSelector = new Sprite(
            BGame,
            Sprite.LoadTexture("ui_box_select", BGame.Content),
            new Vector2());

        EntityManager.AddEntity(tileSelector);

        tileInfoBox = new SidebarMenu(BGame, Font);

    }
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        fpsCounter.Tick(gameTime);

        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;    
        tileInfoBox.Update(dt);

        HandleCameraMovement(dt);
        HandleSelectorMovement();
        HandleTileSelecting();
    }



    private void HandleSelectorMovement()
    {
        var (x, y) = map.WorldToMapPosition(BGame.MouseState.Position.ToVector2() + Camera.Position);
        if (map.TryGetTile(x, y) != null)
        {
            tileSelector.Transform.Position = map.MapToWorldPosition((x, y));
        }
    }

    private void HandleTileSelecting()
    {
        if (BGame.MouseState.WasButtonJustDown(MouseButton.Left))
        {
            showTileInfo = true;
        }
        else if (BGame.MouseState.WasButtonJustDown(MouseButton.Right))
        {
            showTileInfo = false;
        }
        else return;

        tileInfoBox.Items.Clear();

        if (showTileInfo)
        {
            tileInfoBox.AddItem($"Tile Info", Color.White, true);
            
            var (selectX, selectY) = map.WorldToMapPosition(tileSelector.Transform.Position);
            
            var selectedObj = map.TileObjects[selectX, selectY];            
            var selectedTile = map.Tiles[selectX, selectY];

            tileInfoBox.AddItem($"tile:   {selectedTile.TileType}", Color.Yellow);
            tileInfoBox.AddItem($"solid:  {selectedTile.HasCollider}", Color.Yellow);
            tileInfoBox.AddItem($"pos:    {selectX}-{selectY}", Color.Yellow);



            if (selectedObj != null)
            {
                tileInfoBox.AddItem("", Color.White);
                tileInfoBox.AddItem($"Object", Color.White);
                tileInfoBox.AddItem($"{(selectedObj as TileObject).Name}", Color.Yellow, true);
            }
        }
    }

    private void HandleCameraMovement(float dt)
    {
        if (BGame.KeyState.IsKeyDown(Keys.W))
        {
            AttemptCameraMove(-Vector2.UnitY, dt);
        }
        if (BGame.KeyState.IsKeyDown(Keys.S))
        {
            AttemptCameraMove(Vector2.UnitY, dt);
        }
        if (BGame.KeyState.IsKeyDown(Keys.D))
        {
            AttemptCameraMove(Vector2.UnitX, dt);
        }
        if (BGame.KeyState.IsKeyDown(Keys.A))
        {
            AttemptCameraMove(-Vector2.UnitX, dt);
        }
    }

    private void AttemptCameraMove(Vector2 movement, float dt)
    {
        var rect = Camera.BoundingRectangle;
        var pos = rect.Position;
        var size = rect.Size;
        var nextPos = pos + movement * camSpeed * dt;
        if (nextPos.X <= map.TileSize * map.Width - size.Width &&
            nextPos.X >= 0f &&
            nextPos.Y <= map.TileSize * map.Height - size.Height &&
            nextPos.Y >= 0f)
        {
            Camera.Move(movement * camSpeed * dt);
        }
    }


    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        
        BGame.SpriteBatch.Begin(samplerState: SamplerState.PointWrap);


        if (showTileInfo)
            tileInfoBox.Draw(BGame.SpriteBatch);

        fpsCounter.Draw(BGame.SpriteBatch);

        //mouse pointer on top of everything
        BGame.SpriteBatch.DrawCircle(new CircleF(BGame.MouseState.Position, 4f), 10, Color.Yellow);

        BGame.SpriteBatch.End();
    }
}