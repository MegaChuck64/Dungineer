using Engine;
using GameCode.Entities;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input;
using System.Linq;

namespace GameCode.Screens;

public class PlayScreen : BaseScreen
{
    public Character Player { get; set; }
    public PathFinder PathFinder { get; set; }
    public TileSelector Select { get; set; }
    public TileMap Map { get; set; }
    public Terminal Terminal { get; set; }
    public ItemInfoCard ItemInfo { get; set; }
    public PlayScreen(Game game, Character player) : base(game, "consolas_14")
    {
        Player = player;
    }
    public override void LoadContent()
    {
        base.LoadContent();

        var cursor = new Cursor(BGame);
        EntityManager.AddEntity(cursor);

        Map = new TileMap(BGame, 25, 25, Camera);
        Player.X = 4;
        Player.Y = 4;
        Map.TileObjects ??= new System.Collections.Generic.List<TileObject>();
        Map.TileObjects.Add(Player);
        EntityManager.AddEntity(Map);

        Select = new TileSelector(BGame, Sprite.LoadTexture("ui_box_select_32", Content), Map.Width, Map.Height, 32);
        EntityManager.AddEntity(Select);

        PathFinder = new PathFinder(BGame, Map.ToShortCollisionMap(), 32);
        EntityManager.AddEntity(PathFinder);

        var termHeight = 200;
        var termPos = new Point(Map.Width * Map.TileSize + 2, Map.Height * Map.TileSize - termHeight);
        var termSize = new Point(BGame.Width - termPos.X - 2, termHeight);
        Terminal = new Terminal(BGame, Font)
        {
            Bounds = new Rectangle(termPos, termSize)
        };
        EntityManager.AddEntity(Terminal);

        var item = TileLoader.TileObjects.First(t => t.Name == "Short Bow") as Weapon;
        ItemInfo = new ItemInfoCard(BGame, new Vector2(termPos.X, 2), null, Font, item.Name, item.Description);
        EntityManager.AddEntity(ItemInfo);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        var (targetX, targetY) = Map.WorldToMapPosition(BGame.MouseState.Position);
        PathFinder.Map = Map.ToShortCollisionMap((Player.X, Player.Y));
        PathFinder.CreatePath((Player.X, Player.Y), (targetX, targetY), true, false);

        if (BGame.MouseState.WasButtonJustDown(MouseButton.Left))
        {

            Terminal.Active = Terminal.Bounds.Contains(BGame.MouseState.Position);

            TileObject infoTile;
            var mapObjs = Map.GetMapObjects(targetX, targetY);
            if (mapObjs?.FirstOrDefault() is TileObject tileObj)
            {
                infoTile = tileObj;
            }
            else
            {
                infoTile = Map.GetGroundTile(targetX, targetY);
            }

            if (infoTile != null)
            {
                if (PathFinder.TryStepForward(out (int x, int y)? newPosition))
                {
                    if (newPosition != null)
                    {
                        Player.X = newPosition.Value.x;
                        Player.Y = newPosition.Value.y;
                    }
                }
                ItemInfo.Texture = infoTile.Sprite;
                ItemInfo.Name = infoTile.Name;
                if (infoTile is GroundTile gt) ItemInfo.Info = "Speed Mod: " + gt.SpeedMod;
                else if (infoTile is Weapon wp) ItemInfo.Info = wp.Description;
                else if (infoTile is Character ch) ItemInfo.Info = ch.Description;
                else ItemInfo.Info = $"{infoTile.X}-{infoTile.Y}";
            }
        }
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        
        //BGame.SpriteBatch.Begin(
        //    sortMode: SpriteSortMode.FrontToBack,
        //    samplerState: SamplerState.PointClamp);

        //BGame.SpriteBatch.Draw(Player.Sprite, new Vector2(32 * Player.X, 32 * Player.Y), Color.White);

        //BGame.SpriteBatch.End();

    }
}