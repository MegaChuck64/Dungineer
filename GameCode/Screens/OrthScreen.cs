using Engine;
using GameCode.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System.Collections.Generic;
using System.Linq;

namespace GameCode.Screens;

public class OrthScreen : BaseScreen
{
    FPSCounter fpsCounter;
    TileMap map;
    SidebarMenu tileInfoBox;
    bool showTileInfo = false;
    
    readonly float camSpeed = 64f;
    readonly Dictionary<string, (int x, int y, Sprite obj)> TileObjects = new();

    public OrthScreen(MainGame game) : base(game, "consolas_22") { }

    List<(string name, Sprite obj)> TileObjectsOnTile((int x, int y) tilePos) =>
        TileObjects.Values
        .Where(t => t.x == tilePos.x && t.y == tilePos.y)
        .Select(v => (TileObjects.First(h => h.Value.obj == v.obj).Key, v.obj))
        .ToList();

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

        //player
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
        TileObjects.Add("tileSelector", (10, 10, tileSelect));

        tileInfoBox = new SidebarMenu(BGame, Font);

    }
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        fpsCounter.Tick(gameTime);

        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;    
        tileInfoBox.Update(dt);

        HandleCameraMovement(dt);
        HandlePlayerMovement();
        HandleSelectorMovement();
        HandleTileSelecting();


    }



    private void HandleSelectorMovement()
    {        
        AttempTileObjectMove(
            map.WorldToMapPosition(BGame.MouseState.Position.ToVector2() + Camera.Position), "tileSelector");
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
            tileInfoBox.AddItem($"Tile Info", Color.White);

            var (selectX, selectY, selectObj) = TileObjects["tileSelector"];
            var selectedTileObjects = TileObjectsOnTile((selectX, selectY));
            /*new List<(string name, Sprite obj)>();*/
            var tileInfo = map.Tiles[selectX, selectY];
            tileInfoBox.AddItem($"tile:   {tileInfo.TileType}", Color.Yellow);
            tileInfoBox.AddItem($"solid:  {tileInfo.HasCollider}", Color.Yellow);
            tileInfoBox.AddItem($"pos:    {selectX}-{selectY}", Color.Yellow);

            
            tileInfoBox.AddItem($"Objects", Color.White);

            foreach (var (name, obj) in selectedTileObjects
                .Where(selected => selected.name != "tileSelector"))
            {
                tileInfoBox.AddItem($"name:   {name}", Color.Yellow);
            }
        }
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
        if (TileObjects.ContainsKey(name) && 
            newTilePos.x < map.Tiles.GetLength(0) && newTilePos.x >= 0 &&
            newTilePos.y < map.Tiles.GetLength(1) && newTilePos.y >= 0)
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