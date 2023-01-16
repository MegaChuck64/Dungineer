using Engine;
using GameCode.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System.Collections.Generic;

namespace GameCode.Screens;

public class OrthScreen : BaseScreen
{
    FPSCounter fpsCounter;
    TileMap map;
    TileSelector tileSelector;
    SidebarMenu tileInfoBox;
    bool showTileInfo = false;  
    
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

        tileSelector = new TileSelector(
            BGame,
            map,
            Camera,
            Sprite.LoadTexture("ui_box_select", BGame.Content),
            new Vector2());

        EntityManager.AddEntity(tileSelector);

        var camController = new CameraController(BGame, Camera, map);
        EntityManager.AddEntity(camController);

        tileInfoBox = new SidebarMenu(BGame, Font);

    }
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        fpsCounter.Tick(gameTime);

        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;    

        tileInfoBox.Update(dt);

        HandleTileSelecting();
    }




    private void HandleTileSelecting()
    {
        var (selectX, selectY) = map.WorldToMapPosition(tileSelector.Transform.Position);

        if (BGame.MouseState.WasButtonJustDown(MouseButton.Left))
        {
            showTileInfo = true;
            var fighter = map.TryGetTileObject(10, 10);
            if (fighter is Character chr)
            {
                chr.SetTarget((selectX, selectY));
            }
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
            
            
            var selectedObj = map.TileObjects[selectX, selectY];            
            var selectedTile = map.Tiles[selectX, selectY];

            tileInfoBox.AddItem($"tile:   {selectedTile.TileType}", Color.Yellow);
            tileInfoBox.AddItem($"solid:  {selectedTile.HasCollider}", Color.Yellow);
            tileInfoBox.AddItem($"pos:    {selectX}-{selectY}", Color.Yellow);



            if (selectedObj != null)
            {
                tileInfoBox.AddItem("", Color.White);
                tileInfoBox.AddItem($"Object", Color.White, true);
                tileInfoBox.AddItem($"{selectedObj.Name}", Color.Yellow);

                if (selectedObj is Character chr)
                {
                    tileInfoBox.AddItem($"{chr.Race} {chr.Class}", Color.Yellow);                    
                    tileInfoBox.AddItem($" HP: {chr.Health}/{chr.MaxHealth}", Color.Yellow);
                    tileInfoBox.AddItem($"STM: {chr.Stamina}/{chr.MaxStamina}", Color.Yellow);
                    tileInfoBox.AddItem($"STR: {chr.Strength}", Color.Yellow);
                    tileInfoBox.AddItem($"SPD: {chr.Speed}", Color.Yellow);
                }
            }
        }
    }






    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        
        BGame.SpriteBatch.Begin(
            samplerState: SamplerState.PointClamp,
            sortMode: SpriteSortMode.FrontToBack);


        if (showTileInfo)
            tileInfoBox.Draw(BGame.SpriteBatch);

        fpsCounter.Draw(BGame.SpriteBatch);

        //mouse pointer on top of everything
        BGame.SpriteBatch.DrawCircle(new CircleF(BGame.MouseState.Position, 4f), 10, Color.Yellow);

        BGame.SpriteBatch.End();
    }
}