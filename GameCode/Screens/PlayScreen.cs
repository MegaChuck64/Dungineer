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
    public TileInfoCard ItemInfo { get; set; }
    public PlayScreen(Game game, Character player) : base(game, "consolas_14")
    {
        Player = player; 
    }
    public override void LoadContent()
    {
        base.LoadContent();

        var cursor = new Entity(BGame);
        var cursorComp = new Cursor(cursor);
        cursor.Components.Add(cursorComp);
        EntityManager.Entities.Add(cursor);

        var mapEntity = new Entity(BGame);
        Map = new TileMap(mapEntity, 25, 25, BGame.Rand, Camera);
        Player.X = 4;
        Player.Y = 4;
        Map.TileObjects.Add(Player);
        mapEntity.Components.Add(Map);
        EntityManager.Entities.Add(mapEntity);

        var selectEntity = new Entity(BGame);
        Select = new TileSelector(selectEntity, ContentLoader.LoadTexture("ui_box_select_32", Content), Map.Width, Map.Height, 32);
        selectEntity.Components.Add(Select);
        EntityManager.Entities.Add(selectEntity);

        var pfEntity = new Entity(BGame);
        PathFinder = new PathFinder(pfEntity, Map.ToShortCollisionMap(), 32);
        pfEntity.Components.Add(PathFinder);
        EntityManager.Entities.Add(pfEntity);

        var termHeight = 200;
        var termPos = new Point(Map.Width * Map.TileSize + 2, Map.Height * Map.TileSize - termHeight);
        var termSize = new Point(BGame.Width - termPos.X - 2, termHeight);
        var termEntity = new Entity(BGame);
        Terminal = new Terminal(termEntity, Font)
        {
            Bounds = new Rectangle(termPos, termSize)
        };
        termEntity.Components.Add(Terminal);
        EntityManager.Entities.Add(termEntity);

        //arbitrarily loading short boy to initialize out of laziness.
        //Probably remove some params from tile info constructor 
        var item = TileLoader.GetTileObject(t => t.Name == "Short Bow") as Weapon;

        var infoCardEntity = new Entity(BGame);
        ItemInfo = new TileInfoCard(
            infoCardEntity, 
            new Rectangle(termPos.X, 2, termSize.X, Map.TileSize * 4), 
            null, 
            Font, 
            Font, 
            item.Name, 
            item.Description);
        infoCardEntity.Components.Add(ItemInfo);
        EntityManager.Entities.Add(infoCardEntity);
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
                else if (infoTile is Character ch) ItemInfo.Info = $"{ch.Race} {ch.Class} {ch.Weapon}";
                //else if (infoTile is ItemTile it) ItemInfo.Info = it.Description;
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