﻿using Engine;
using GameCode.Entities;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input;

namespace GameCode.Screens;

public class PlayScreen : BaseScreen
{
    public Character Player { get; set; }
    public PathFinder PathFinder { get; set; }
    public TileSelector Select { get; set; }
    public TileMap Map { get; set; }
    public Terminal Terminal { get; set; }
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
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (BGame.MouseState.WasButtonJustDown(MouseButton.Left))
        {
            var (targetX, targetY) = Map.WorldToMapPosition(BGame.MouseState.Position);
            PathFinder.CreatePath((Player.X, Player.Y), (targetX, targetY), true, false);

            Terminal.Active = Terminal.Bounds.Contains(BGame.MouseState.Position);            
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