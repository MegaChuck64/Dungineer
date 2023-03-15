using Dungineer.Behaviors;
using Dungineer.Components.GameWorld;
using Dungineer.Components.UI;
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

    private KeyboardState keyState;
    private KeyboardState lastKeyState;

    private const float groundLayer = 0.5f;
    private const float objectLayer = 0.6f;
    private const float effectLayer = 0.7f;
    private const float itemLayer = 0.8f;

    private readonly Texture2D tileSelectTexture;
    private readonly SpriteBatch sb;
    private readonly List<Entity> entitiesToRemove = new();
    private readonly List<Point> aimingPath = new();

    private float sightRangeBreathSpeed = 2f;
    private float sightRangeBreathTimer = 0f;
    private float sightRangeBreath;
    private float sightBreathOffset = 3f;
    private bool sightRangeBreathOut = false;
    private float lastSightRange = 0f;

    private Camera camera;

    private float[,] lastViewMap;
    public MapSystem(BaseGame game, ContentManager content) : base(game)
    {
        offset = new Vector2(game.Width / 5, 0);

        sb = new SpriteBatch(game.GraphicsDevice);

        tileSelectTexture = ContentLoader.LoadTexture("ui_box_select_32", content);

        camera = new Camera();
    }


    public override void Update(GameTime gameTime, IEnumerable<Entity> entities)
    {
        if (SceneManager.CurrentScene != "Play") return;

        sightRangeBreathTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (sightRangeBreathTimer >= 1f / sightRangeBreathSpeed)
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

        var map = entities.FirstOrDefault(t => t.HasTag("Map"))?.GetComponent<Map>();


        lastMouseState = mouseState;
        mouseState = Mouse.GetState();

        lastKeyState = keyState;
        keyState = Keyboard.GetState();

        foreach (var ent in entities.Reverse()) //todo, reversing for now because we add the player last to the scene, but we want to process it first
        {
            if (ent.GetComponent<CreatureStats>() is CreatureStats creatureStats)
            {
                if (creatureStats.Health <= 0)
                {
                    creatureStats.Health = 0;
                    entitiesToRemove.Add(ent);
                    continue;
                }
            }

            if (ent.HasTag("Player"))
            {
                var playerObject = ent.GetComponent<MapObject>();
                if (MapPixelBounds.Contains(mouseState.Position))
                {
                    if (MouseWasClicked(MouseButton.Left))
                    {
                        if (aimingPath.Count > 0)
                        {
                            var mouseTile = MouseTilePosition;
                            var mapObj = SceneManager.ComponentsOfType<MapObject>().FirstOrDefault(t => t.MapX == mouseTile.X && t.MapY == mouseTile.Y);
                            var targetEnt = SceneManager.GetEntityWithComponent(mapObj);
                            var stats = ent.GetComponent<CreatureStats>();
                            if (targetEnt != null &&
                                Vector2.Distance(new Vector2(playerObject.MapX, playerObject.MapY), mouseTile.ToVector2()) <= stats.AttackRange)
                            {
                                var attack = new BasicAttack(targetEnt);
                                attack.Perform(ent);
                            }
                        }
                        else
                        {
                            //move
                            var movement = new TargetMovement(MouseTilePosition);
                            movement.Perform(ent);

                            //collect
                            var collectables = SceneManager
                                .ComponentsOfType<MapObject>()
                                .Where(t => t.MapX == playerObject.MapX && t.MapY == playerObject.MapY && Settings.MapObjectAtlas[t.Type].Collectable)
                                .ToArray();

                            if (collectables.Any())
                            {
                                var collect = new Collect(collectables.First(), Game);
                                collect.Perform(ent);
                                entitiesToRemove.Add(SceneManager.GetEntityWithComponent(collectables[0]));
                                for (int i = 1; i < collectables.Length; i++)
                                {
                                    collect.MapObject = collectables[i];
                                    collect.Perform(ent);
                                    entitiesToRemove.Add(SceneManager.GetEntityWithComponent(collectables[i]));
                                }
                            }
                        }

                        aimingPath.Clear();
                    }
                    else if (KeyWasClicked(Keys.D1))
                    {
                        //aiming
                        var attack = new BasicAttack(null);
                        aimingPath.AddRange(attack.Aim(ent));
                    }
                }
                
            }
            else if (ent.GetComponent<MapObject>() is MapObject mapObj)
            {
                var mapObjInfo = Settings.MapObjectAtlas[mapObj.Type];

                var player = entities.FirstOrDefault(t => t.HasTag("Player"))?.GetComponent<MapObject>();             
                if (player == null)
                    continue;
                
                if (MouseWasClicked(MouseButton.Left) == false && MouseWasClicked(MouseButton.Right) == false)
                    continue;

                if (MapPixelBounds.Contains(mouseState.Position) == false)
                    continue;

                if (mapObjInfo.Behaviors == null || mapObjInfo.Behaviors.Count == 0)
                    continue;

                if (ent.GetComponent<CreatureStats>() is CreatureStats stats)
                {
                    var dist = Vector2.Distance(new Vector2(player.MapX, player.MapY), new Vector2(mapObj.MapX, mapObj.MapY));

                    var rand = Game.Rand.Next(0, mapObjInfo.Behaviors.Count);
                    var behavior = mapObjInfo.Behaviors[rand];

                    if (behavior == "TargetPlayer")
                    {
                        if (dist <= stats.SightRange)
                        {
                            var targAdj =
                                map.GetAdjacentEmptyTiles(
                                    player.MapX,
                                    player.MapY,
                                    true,
                                    SceneManager.ComponentsOfType<MapObject>().ToArray());

                            if (targAdj.Any())
                            {
                                var (x, y) = targAdj.First();
                                var movement = new TargetMovement(new Point(x, y));
                                movement.Perform(ent);
                            }
                        }
                    }
                    else if (behavior == "BasicAttack")
                    {
                        if (dist <= stats.AttackRange)
                        {
                            var attack = new BasicAttack(SceneManager.GetEntityWithComponent(player));
                            attack.Perform(ent);
                        }
                    }
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
            transformMatrix: camera.Transform(Game.GraphicsDevice)); //camera here todo


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
                DrawMapObject(mapObj, viewMap, tintMod, ent.HasTag("Player"));

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
            return new float[0, 0];

        var map = mapEnt.GetComponent<Map>();

        var playerEnt = SceneManager.Entities.FirstOrDefault(t => t.HasTag("Player"));
        if (playerEnt != null)
        {
            var player = playerEnt.GetComponent<MapObject>();
            var playerStats = playerEnt.GetComponent<CreatureStats>();
            lastSightRange = playerStats.SightRange;
            var viewMap = GetViewMap(new Point(player.MapX, player.MapY), map, playerStats.SightRange + sightRangeBreath);
            lastViewMap = viewMap;
            return viewMap;
        }

        return new float[0,0];
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

        MapObject hoverObj = null;
        if (MapPixelBounds.Contains(mouseState.Position))
        {
            hoverObj = SceneManager.ComponentsOfType<MapObject>()
                .Where(t => t.MapX == MouseTilePosition.X && t.MapY == MouseTilePosition.Y)
                .FirstOrDefault();
        }

        //ground tiles
        for (int x = 0; x < map.GroundTiles.GetLength(0); x++)
        {
            for (int y = 0; y < map.GroundTiles.GetLength(1); y++)
            {
                if (viewMap.GetLength(0) == 0)
                    viewMap = lastViewMap;

                //draw tile if visible
                if (viewMap.GetLength(0) > 0 && viewMap[x, y] != float.MaxValue)
                {
                    var distSqr = (float)Math.Sqrt(viewMap[x, y]) / lastSightRange;
                    var tintMod = MathHelper.Lerp(lastSightRange, 0f, distSqr);

                    var tileBounds = DrawTile(map.GroundTiles[x, y], groundLayer, tintMod / lastSightRange);

                    //if aiming
                    if (aimingPath.Contains(new Point(x,y)))
                    {
                        //draw highlight on hover
                        if (hoverObj != null && hoverObj.MapX == x && hoverObj.MapY == y)
                        {
                            Color? tint = null;
                            //new Color(255, 200, 0, 100)
                            var hoverEnt = SceneManager.GetEntityWithComponent(hoverObj);
                            if (hoverEnt.HasTag("Player") == false)
                            {
                                if (SceneManager.Entities.FirstOrDefault(t => t.HasTag("Player")) is Entity playerEnt)
                                {
                                    var playerObj = playerEnt.GetComponent<MapObject>();
                                    var playerStats = playerEnt.GetComponent<CreatureStats>();
                                    if (hoverEnt.GetComponent<CreatureStats>() is CreatureStats monsterStats &&
                                        Vector2.Distance(
                                            new Vector2(playerObj.MapX, playerObj.MapY),
                                            new Vector2(hoverObj.MapX, hoverObj.MapY)) <= playerStats.AttackRange)
                                    {
                                        tint = new Color(1f, 0f, 0f, 0.5f);
                                    }
                                }

                            }

                            if (tint.HasValue)
                                DrawTileHighlight(tint.Value, tileBounds, groundLayer + 0.1f);
                        }
                        else
                        {
                            DrawTileHighlight(new Color(0f, 1f, 0f, 0.25f), tileBounds, groundLayer + 0.1f);
                        }
                    }
                
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
                DrawTile(map.ObjectTiles[i], objectLayer, tintMod / lastSightRange);
            }
        }
    }

    //items that will move around our map
    private void DrawMapObject(MapObject mapObject, float[,] viewMap, float tint, bool isPlayer)
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


        if (isPlayer)
        {
            camera.Position = bnds.Location.ToVector2();
        }

    }
    private Rectangle DrawTile(Tile tile, float layer, float tintMod)
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

        return bnds;
    }

    private void DrawTileHighlight(Color tint, Rectangle bounds, float layer)
    {
        sb.Draw(Settings.TextureAtlas["_pixel"], bounds, null, tint, 0f, Vector2.Zero, SpriteEffects.None, layer);
    }

    #endregion


    #region Helpers 

    private bool MouseWasClicked(MouseButton mb) => mb switch
    {
        MouseButton.Left => lastMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed,
        MouseButton.Right => lastMouseState.RightButton == ButtonState.Released && mouseState.RightButton == ButtonState.Pressed,
        MouseButton.Middle => lastMouseState.MiddleButton == ButtonState.Released && mouseState.MiddleButton == ButtonState.Pressed,
        _ => false
    };

    private bool KeyWasClicked(Keys key) => lastKeyState.IsKeyUp(key) && keyState.IsKeyDown(key);

    private Rectangle MapPixelBounds => new(Game.Width / 5, 0, (Game.Width / 5) * 3, Game.Height);

    private Rectangle GetTileBounds(Point pos) => GetTileBounds(pos.X, pos.Y);

    private Rectangle GetTileBounds(int x, int y) =>
        new (offset.ToPoint() + new Point(x * Settings.TileSize, y * Settings.TileSize),
            new Point(Settings.TileSize, Settings.TileSize));


    private Point MouseTilePosition => 
        new ((int)(mouseState.X - offset.ToPoint().X) / Settings.TileSize,
            (int)(mouseState.Y - offset.ToPoint().Y) / Settings.TileSize);
    




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