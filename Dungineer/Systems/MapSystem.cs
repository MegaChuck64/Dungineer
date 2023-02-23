using Dungineer.Components.GameWorld;
using Dungineer.Components.UI;
using Dungineer.Models;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Dungineer.Systems;

public class MapSystem : BaseSystem
{
    private Vector2 offset;

    private MouseState mouseState;
    private MouseState lastMouseState;

    private const float groundLayer = 0.5f;
    private const float objectLayer = 0.6f;
    private const float effectLayer = 0.7f;
    private const float itemLayer = 0.8f;

    private readonly Texture2D tileSelectTexture;
    private readonly SpriteBatch sb;
    private readonly List<Entity> entitiesToRemove = new ();

    private float sightRangeBreathSpeed = 2f;
    private float sightRangeBreathTimer = 0f;
    private float sightRangeBreath;
    private float sightBreathOffset = 3f;
    private bool sightRangeBreathOut = false;
    private float lastSightRange = 0f;
    public MapSystem(BaseGame game, ContentManager content) : base(game)
    {
        offset = new Vector2(game.Width / 5, 0);

        sb = new SpriteBatch(game.GraphicsDevice);

        tileSelectTexture = ContentLoader.LoadTexture("ui_box_select_32", content);
    }


    public override void Update(GameTime gameTime, IEnumerable<Entity> entities)
    {
        if (SceneManager.CurrentScene != "Play") return;

        sightRangeBreathTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (sightRangeBreathTimer >= 1f/sightRangeBreathSpeed)
        {
            sightRangeBreathTimer = 0f;

            if (sightRangeBreath == 0)
            {
                if (sightRangeBreathOut)
                    sightRangeBreath++;
                else
                    sightRangeBreath--;
            }
            else if (sightRangeBreath > 0)
            {
                if (sightRangeBreath < sightBreathOffset && !sightRangeBreathOut)
                    sightRangeBreath++;
                else
                {
                    sightRangeBreathOut = false;
                    sightRangeBreath--;
                }
            }
            else if (sightRangeBreath < 0)
            {
                if (sightRangeBreath > -sightBreathOffset && !sightRangeBreathOut)
                    sightRangeBreath--;
                else
                { 
                    sightRangeBreathOut = true;
                    sightRangeBreath++;
                }
            }
        }

        var map = entities.FirstOrDefault(t => t.Components.Any(v => v is Map))?.GetComponent<Map>();

        lastMouseState = mouseState;
        mouseState = Mouse.GetState();

        foreach (var ent in entities)
        {
            if (ent.HasTag("Player"))
            {
                var playerObject = ent.GetComponent<MapObject>();
                if (MouseWasClicked && MapPixelBounds.Contains(mouseState.Position))
                {
                    MovePlayer(playerObject, map, SceneManager.ComponentsOfType<MapObject>().ToArray());
                }
            }
            else if (ent.HasTag("Ghost"))
            {
                var player = entities.First(t => t.HasTag("Player")).GetComponent<MapObject>();
                var ghostObj = ent.GetComponent<MapObject>();
                if (MouseWasClicked && MapPixelBounds.Contains(mouseState.Position))
                {
                    MoveGhost(ent, ghostObj, player, map, SceneManager.ComponentsOfType<MapObject>().ToArray());
                }
            }
            else if (ent.HasTag("Cursor"))
            {
                ent.GetComponent<UIElement>().IsActive = !MapPixelBounds.Contains(mouseState.Position);
            }
        }

        foreach (var ent in entitiesToRemove)
        {
            SceneManager.RemoveEntity(SceneManager.CurrentScene, ent);
        }
        entitiesToRemove.Clear();
    }



    
    #region Drawing

    public override void Draw(GameTime gameTime, IEnumerable<Entity> entities)
    {
        //DRAWING
        sb.Begin(
            sortMode: SpriteSortMode.FrontToBack,
            blendState: BlendState.NonPremultiplied,
            samplerState: SamplerState.PointClamp,
            depthStencilState: DepthStencilState.DepthRead,
            rasterizerState: RasterizerState.CullCounterClockwise,
            effect: null,
            transformMatrix: null); //camera here todo


        var viewMap = UpdatePlayerViewMap();


        foreach (var ent in entities)
        {
            if (ent.HasTag("Map"))
            {
                DrawMap(ent, viewMap);
            }
            else if (ent.GetComponent<MapObject>() is MapObject mapObj)
            {

                float tintMod = 1f;
                if (viewMap.GetLength(0) > 0 && viewMap.GetLength(1) > 0)
                {
                    var distSqr = (float)Math.Sqrt(viewMap[mapObj.MapX, mapObj.MapY]) / lastSightRange;
                    tintMod = MathHelper.Lerp(lastSightRange, 0f, distSqr);
                }
                DrawMapObject(mapObj, viewMap, tintMod);

                if (ent.GetComponent<Wardrobe>() is Wardrobe wardrobe)
                {
                    DrawWardrobe(wardrobe, mapObj);
                }
            }
        }

        sb.End();

    }

    private float[,] UpdatePlayerViewMap()
    {
        var mapEnt = SceneManager.Entities.FirstOrDefault(t => t.HasTag("Map"));
        if (mapEnt == null)
            return new float[0,0];

        var map = mapEnt.GetComponent<Map>();

        var playerEnt = SceneManager.Entities.FirstOrDefault(t => t.HasTag("Player"));
        var player = playerEnt.GetComponent<MapObject>();
        var playerStats = playerEnt.GetComponent<CreatureStats>();
        lastSightRange = playerStats.SightRange;
        var viewMap = GetViewMap(new Point(player.MapX, player.MapY), map, playerStats.SightRange + sightRangeBreath);

        return viewMap;
    }
    private void DrawWardrobe(Wardrobe wardrobe, MapObject mapObj)
    {
        if (wardrobe.BodySlot.HasValue)
        {
            var winfo = Settings.WardrobeAtlas[wardrobe.BodySlot.Value];
            var wtxt = Settings.TextureAtlas[winfo.TextureName];

            var bnds = GetTileBounds(mapObj.MapX, mapObj.MapY);
            bnds = new Rectangle(bnds.X, bnds.Y, (int)(bnds.Width * mapObj.Scale), (int)(bnds.Height * mapObj.Scale));
            sb.Draw(
                wtxt,
                bnds,
                winfo.Source,
                mapObj.Tint,
                0f,
                mapObj.Scale == 1f ? Vector2.Zero : new Vector2(bnds.Width / 2, bnds.Height / 2),
                SpriteEffects.None,
                itemLayer + 0.1f);
        }
    }

    private void DrawMap(Entity ent, float[,] viewMap)
    {

        var map = ent.GetComponent<Map>();
        if (map == null) throw new System.Exception("Entity tagged with 'Map' must have map component");



        //ground tiles
        for (int x = 0; x < map.GroundTiles.GetLength(0); x++)
        {
            for (int y = 0; y < map.GroundTiles.GetLength(1); y++)
            {
                if (viewMap[x, y] != float.MaxValue)
                {
                    var distSqr = (float)Math.Sqrt(viewMap[x, y])/lastSightRange;
                    var tintMod = MathHelper.Lerp(lastSightRange, 0f, distSqr);
                    DrawTile(map.GroundTiles[x, y], groundLayer, tintMod/lastSightRange);

                }
            }
        }

        //object tiles
        for (int i = 0; i < map.ObjectTiles.Count; i++)
        {
            if (viewMap[map.ObjectTiles[i].X, map.ObjectTiles[i].Y] != float.MaxValue)
            {
                var distSqr = (float)Math.Sqrt(viewMap[map.ObjectTiles[i].X, map.ObjectTiles[i].Y]) / lastSightRange;
                var tintMod = MathHelper.Lerp(lastSightRange, 0f, distSqr);
                DrawTile(map.ObjectTiles[i], objectLayer, tintMod/lastSightRange);
            }
        }
    }

    //items that will move around our map
    private void DrawMapObject(MapObject mapObject, float[,] viewMap, float tint)
    {
        if (viewMap.GetLength(0) != 0 && viewMap[mapObject.MapX, mapObject.MapY] == float.MaxValue) return;


        var mapObjectInfo = Settings.MapObjectAtlas[mapObject.Type];
        var texture = Settings.TextureAtlas[mapObjectInfo.TextureName];

        var bnds = GetTileBounds(mapObject.MapX, mapObject.MapY);
        bnds = new Rectangle(bnds.X, bnds.Y, (int)(bnds.Width * mapObject.Scale), (int)(bnds.Height * mapObject.Scale));

        sb.Draw(
            texture,
            bnds,
            mapObjectInfo.Source,
            mapObject.Tint * tint,
            0f,
            mapObject.Scale == 1f ? Vector2.Zero : new Vector2(bnds.Width / 2, bnds.Height / 2),
            SpriteEffects.None,
            itemLayer);


    }
    private void DrawTile(Tile tile, float layer, float tintMod)
    {
        var tileInfo = Settings.TileAtlas[tile.Type];
        var texture = Settings.TextureAtlas[tileInfo.TextureName];

        var bnds = GetTileBounds(tile.X, tile.Y);

        sb.Draw(texture, bnds, tileInfo.Source, tile.Tint * tintMod, 0f, Vector2.Zero, SpriteEffects.None, layer);

        if (bnds.Contains(mouseState.Position))
        {
            sb.Draw(
                tileSelectTexture,
                bnds,
                new Rectangle(0, 0, 32, 32),
                Color.White,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                layer + 0.1f);
        }

    }

    #endregion

    
    #region Creature Movement

    private void MoveGhost(Entity ent, MapObject ghost, MapObject target, Map map, params MapObject[] mapObjects)
    {
        //is the target visible
        var dist = Vector2.Distance(new Vector2(ghost.MapX, ghost.MapY), new Vector2(target.MapX, target.MapY));

        var stats = ent.GetComponent<CreatureStats>();

        if (dist > stats.SightRange)
            return;
        

        //are there any tiles adjacent to target to stand on
        var targAdj = map.GetAdjacentEmptyTiles(target.MapX, target.MapY, true, mapObjects);

        if (!targAdj.Any())
            return;
   

        //is there a valid path to the target
        var (x, y) = targAdj.First();

        var path = GetPath(
            new Point(ghost.MapX, ghost.MapY),
            new Point(x, y),
                map,
                mapObjects);

        if (path == null || path.Count == 0)
            return;
        

        //Attack or move 
        if (Game.Rand.NextSingle() < 0.4 && dist <= stats.AttackRange)
        {
            //attack
            var otherEnt = SceneManager.GetEntityWithComponent(target);
            var otherStats = otherEnt.GetComponent<CreatureStats>();

            otherStats.Health -= stats.Strength;
        }
        else 
        {
            //step
            var nextStep = path.First();

            if (nextStep != new Point(target.MapX, target.MapY))
            {
                ghost.MapX = nextStep.X;
                ghost.MapY = nextStep.Y;
            }
        }
        
    }

    private void MovePlayer(MapObject player, Map map, params MapObject[] mapObjects)
    {
        var path = GetPath(
            new Point(player.MapX, player.MapY),
                MouseTilePosition,
                map,
                mapObjects);

        if (path != null && path.Count > 0)
        {
            var nextStep = path.First();
            player.MapX = nextStep.X;
            player.MapY = nextStep.Y;

            if (mapObjects.Any(t=>t.MapX == player.MapX && t.MapY == player.MapY && Settings.MapObjectAtlas[t.Type].Collectable))
            {
                var collectables = mapObjects
                    .Where(t => t.MapX == player.MapX && t.MapY == player.MapY && Settings.MapObjectAtlas[t.Type].Collectable)
                    .ToList();

                for (int i = 0; i < collectables.Count; i++)
                {
                    switch (collectables[i].Type)
                    {
                        case MapObjectType.Arcanium:
                            var playerEntity = SceneManager.GetEntityWithComponent(player);
                            if (playerEntity != null)
                            {
                                var stats = playerEntity.GetComponent<CreatureStats>();
                                stats.Money += Game.Rand.Next(1, 10);

                                entitiesToRemove.Add(SceneManager.GetEntityWithComponent(collectables[i]));
                            }
                            break;
                    }
                }
            }
        }
    }

    #endregion


    #region Helpers 

    private bool MouseWasClicked => lastMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed;

    private Rectangle MapPixelBounds => new(Game.Width / 5, 0, (Game.Width / 5) * 3, Game.Height);

    private Rectangle GetTileBounds(Point pos) => GetTileBounds(pos.X, pos.Y);

    private Rectangle GetTileBounds(int x, int y) =>
        new (offset.ToPoint() + new Point(x * Settings.TileSize, y * Settings.TileSize),
            new Point(Settings.TileSize, Settings.TileSize));


    private Point MouseTilePosition => 
        new ((int)(mouseState.X - offset.ToPoint().X) / Settings.TileSize,
            (int)(mouseState.Y - offset.ToPoint().Y) / Settings.TileSize);
    


    public static List<Point> GetPath(Point start, Point end, Map map, params MapObject[] mapObjects)
    {
        var grid = new bool[map.GroundTiles.GetLength(0), map.GroundTiles.GetLength(1)];

        for (int x = 0; x < map.GroundTiles.GetLength(0); x++)
        {
            for (int y = 0; y < map.GroundTiles.GetLength(1); y++)
            {
                var groundTile = Settings.TileAtlas[map.GroundTiles[x, y].Type];
                var objectTiles = map.ObjectTiles.Where(t => t.X == x && t.Y == y).Select(v => Settings.TileAtlas[v.Type]);

                var hasGroundCollision = groundTile.Solid;
                var hasObjectCollision = objectTiles.Any(y => y.Solid);
                var hasItemCollision = mapObjects.Any(g => g.MapX == x && g.MapY == y && !Settings.MapObjectAtlas[g.Type].Collectable);

                grid[x, y] = !hasGroundCollision && !hasObjectCollision && !hasItemCollision;
            }
        }

        var pathFinder = new PathFinder(new(start, end, grid));

        return pathFinder.FindPath();

    }

    public static float[,] GetViewMap(Point start, Map map, float viewRadius, params MapObject[] mapObjects)
    {
        var grid = new bool[map.GroundTiles.GetLength(0), map.GroundTiles.GetLength(1)];

        for (int x = 0; x < map.GroundTiles.GetLength(0); x++)
        {
            for (int y = 0; y < map.GroundTiles.GetLength(1); y++)
            {
                //var groundTile = Settings.TileAtlas[map.GroundTiles[x, y].Type];
                var objectTiles = map.ObjectTiles.Where(t => t.X == x && t.Y == y).Select(v => Settings.TileAtlas[v.Type]);


                //not checking ground collisions. Because solid ground tiles should be like impassible obstacles at ground level
                //like water, or a hole in the floor. Walls and things on top of the ground that would block light belong
                //as a mapobject or an object tile

                //todo: the above comment should be the standard. But right now we are using holes in the floor as an object tile,
                //because it relys on the background of the tile underneath it. Might need to consider a new list of decals, or a height property

                //var hasGroundCollision = groundTile.Solid;
                var hasObjectCollision = objectTiles.Any(y => y.Solid);
                var hasItemCollision = mapObjects.Any(g => g.MapX == x && g.MapY == y && !Settings.MapObjectAtlas[g.Type].Collectable);

                grid[x, y] = hasObjectCollision || hasItemCollision;
            }
        }

        var viewMap = ShadowCaster.GetViewMap(grid, start, viewRadius);
        return viewMap;
    }
    #endregion
}