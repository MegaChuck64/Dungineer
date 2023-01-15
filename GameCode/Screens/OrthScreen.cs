using Engine;
using GameCode.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCode.Screens;

public class OrthScreen : BaseScreen
{
    FPSCounter fpsCounter;

    TileMap map;
    float camSpeed = 64f;
    Dictionary<string, (int x, int y, Sprite obj)> TileObjects = new();
    bool showTileInfo = false;
    UIBox tileInfoBox;
    List<(string name, Sprite obj)> TileObjectsOnTile((int x, int y) tilePos) =>
        TileObjects.Values
        .Where(t => t.x == tilePos.x && t.y == tilePos.y)
        .Select(v => (TileObjects.First(h => h.Value.obj == v.obj).Key, v.obj))
        .ToList();

    public OrthScreen(MainGame game) : base(game, "consolas_22")
    {
        
    }
    public override void LoadContent()
    {
        base.LoadContent();
        
        fpsCounter = new FPSCounter(BGame as MainGame) { Font = Font };
        map = new TileMap(BGame);
        EntityManager.AddEntity(map);

        var player = 
            new Sprite(
                BGame, 
                Sprite.TextureFromSpriteAtlas("HumanFighter", new Rectangle(11, 11, 32, 32), BGame.Content), 
                map.MapToWorldPosition((10, 10)));
        EntityManager.AddEntity(player);
        TileObjects.Add("player", (10, 10, player));

        var tileSelect = new Sprite(
            BGame,
            Sprite.LoadTexture("ui_box_select", BGame.Content),
            new Vector2());

        EntityManager.AddEntity(tileSelect);
        TileObjects.Add("tileSelect", (10, 10, tileSelect));

        tileInfoBox = new UIBox(
            BGame, 
            Sprite.LoadTexture("hud-tileset", BGame.Content), 
            new Rectangle(new Point(BGame.Width/2 - 200, BGame.Height/2 - 250), new Point(400, 500)),
            Font);

    }
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        fpsCounter.Tick(gameTime);

        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;    
        
        HandleCameraMovement(dt);
        HandlePlayerMovement();
        HandleTileSelect();

        tileInfoBox.Update(dt);

        var (selectX, selectY, selectObj) = TileObjects["tileSelect"];
        var selectedTileObjects = TileObjectsOnTile((selectX, selectY));
        var tileInfo = map.Tiles[(selectX, selectY)];
        var tileInfoBuilder = new StringBuilder();
        tileInfoBuilder.AppendLine($"tile   {tileInfo.TileType}");
        tileInfoBuilder.AppendLine($"solid  {tileInfo.HasCollider}");
        tileInfoBuilder.AppendLine($"pos    {selectX}-{selectY}");
        tileInfoBuilder.AppendLine();

        if (selectedTileObjects.Count > 1)
            tileInfoBuilder.AppendLine("______objs______");
        else
            tileInfoBuilder.AppendLine("________________");

        tileInfoBuilder.AppendLine();

        foreach (var selected in selectedTileObjects)
        {
            if (selected.name != "tileSelect")
            {
                tileInfoBuilder.AppendLine($"name   {selected.name}");
            }
        }

        
        tileInfoBox.Data = tileInfoBuilder.ToString();

        showTileInfo = BGame.MouseState.IsButtonDown(MonoGame.Extended.Input.MouseButton.Left);
    }

    private void HandleTileSelect()
    {        
        AttempTileObjectMove(map.WorldToMapPosition(BGame.MouseState.Position.ToVector2() + Camera.Position), "tileSelect");
    }
    private void HandlePlayerMovement()
    {
        var (playerX, playerY, _) = TileObjects["player"];

        if (BGame.KeyState.WasKeyJustDown(Keys.Up))
        {
            AttempTileObjectMove((playerX, playerY - 1), "player");
        }
        if (BGame.KeyState.WasKeyJustDown(Keys.Down))
        {
            AttempTileObjectMove((playerX, playerY + 1), "player");
        }
        if (BGame.KeyState.WasKeyJustDown(Keys.Right))
        {
            AttempTileObjectMove((playerX + 1, playerY), "player");
        }
        if (BGame.KeyState.WasKeyJustDown(Keys.Left))
        {
            AttempTileObjectMove((playerX - 1, playerY), "player");
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
            Camera.Move(movement);
        }
    }

    private void AttempTileObjectMove((int x, int y) newTilePos, string name)
    {
        if (TileObjects.ContainsKey(name) && map.Tiles.ContainsKey(newTilePos))
        {            
            TileObjects[name].obj.Transform.Position = map.MapToWorldPosition(newTilePos);
            TileObjects[name] = (newTilePos.x, newTilePos.y, TileObjects[name].obj);
        }
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        
        BGame.SpriteBatch.Begin(samplerState: SamplerState.PointWrap);
        fpsCounter.Draw(BGame.SpriteBatch);

        //mouse pointer on top of everything
        BGame.SpriteBatch.DrawCircle(new CircleF(BGame.MouseState.Position, 4f), 10, Color.Yellow);

        if (showTileInfo)
            tileInfoBox.Draw(BGame.SpriteBatch);

        BGame.SpriteBatch.End();
    }
}