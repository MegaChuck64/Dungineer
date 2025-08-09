using Dungineer.Behaviors;
using Dungineer.Behaviors.Effects;
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
    private const float GROUND_LAYER = 0.5f;
    private const float OBJECT_LAYER = 0.6f;
    private const float EFFECT_LAYER = 0.7f;
    private const float ITEM_LAYER = 0.8f;

    private readonly Texture2D tileSelectTexture;
    private readonly Texture2D cursorTexture;
    private readonly SpriteBatch sb;
    private readonly List<Entity> entitiesToRemove = new();
    private readonly List<Point> aimingPath = new();
    private readonly Vector2 offset;


    public Camera Cam { get; private set; }
    public SightSystem PlayerSight { get; private set; }


    private readonly MapObjectType[] dropLottery = new MapObjectType[]
    {
        MapObjectType.Arcanium,
        MapObjectType.HealthPotion
    };


    /// <summary>
    /// Creates a new map system that manages the game world, handling tiles, objects and camera movement
    /// </summary>
    /// <param name="game">Main game instance providing access to graphics and dimensions</param>
    /// <param name="content">Content manager for loading textures and resources</param>
    public MapSystem(BaseGame game, ContentManager content) : base(game)
    {
        offset = new Vector2(game.Width / 5, 0);

        sb = new SpriteBatch(game.GraphicsDevice);

        tileSelectTexture = ContentLoader.LoadTexture("ui_box_select_32", content);
        cursorTexture = ContentLoader.LoadTexture("cursor_16", content);

        Cam = new Camera();
        PlayerSight = new SightSystem(game);
    }

    /// <summary>
    /// Main update loop for the map system, handling entity processing and state management
    /// </summary>
    /// <param name="gameTime">Provides timing information for the game</param>
    /// <param name="entities">All entities currently in the scene</param>
    public override void Update(GameTime gameTime, IEnumerable<Entity> entities)
    {
        if (SceneManager.CurrentScene != "Play") return;

        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        PlayerSight.Update(gameTime, entities);

        var map = entities.FirstOrDefault(t => t.HasTag("Map"))?.GetComponent<Map>();

        foreach (var ent in entities.Reverse()) //todo, reversing for now because we add the player last to the scene, but we want to process it first
        {
            if (TryKillCreature(ent))
                continue;

            if (ent.HasTag("Player"))
            {
                UpdatePlayer(ent);
            }
            else if (ent.GetComponent<MapObject>() is MapObject mapObj)
            {
                var player = entities.FirstOrDefault(t => t.HasTag("Player"))?.GetComponent<MapObject>();
                if (player == null)
                    continue;

                if (Input.WasPressed(MouseButton.Left) == false && Input.WasPressed(MouseButton.Right) == false)
                    continue;

                if (MapPixelBounds.Contains(Input.MouseState.Position) == false) //todo: is this right? 
                    continue;

                UpdateMapObject(ent, mapObj, player, map);
            }
            else if (ent.HasTag("Cursor"))
            {
                ent.GetComponent<UIElement>().IsActive = !MapPixelBounds.Contains(Input.MouseState.Position);
            }
        }

        foreach (var ent in entitiesToRemove)
        {
            SceneManager.RemoveEntity(SceneManager.CurrentScene, ent);
        }
        entitiesToRemove.Clear();
    }

  
    /// <summary>
    /// Checks if a creature's health is zero or negative and handles its death.
    /// Dead creatures are marked for removal and may drop items based on their drop chance.
    /// </summary>
    /// <param name="ent">Entity to check for death condition</param>
    /// <returns>True if the creature died and was processed, false otherwise</returns>
    private bool TryKillCreature(Entity ent)
    {
        if (ent.GetComponent<CreatureStats>() is CreatureStats creatureStats)
        {
            if (creatureStats.Health <= 0)
            {
                creatureStats.Health = 0;
                entitiesToRemove.Add(ent);

                if (ent.GetComponent<MapObject>() is MapObject mapObj)
                {
                    var info = Settings.MapObjectAtlas[mapObj.Type];
                    var rand = MainGame.Rand.NextDouble();
                    if (rand <= info.DropChance)
                    {
                        var drop = new DropOnDeath(dropLottery);
                        drop.TryPerform(ent, null);
                    }
                }

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Updates the player entity based on input, handling movement, spells, and item collection.
    /// Processes left clicks for actions and number keys for spell selection.
    /// </summary>
    /// <param name="ent">The player entity to update</param>
    private void UpdatePlayer(Entity ent)
    {
        var playerObj = ent.GetComponent<MapObject>();

        if (MapPixelBounds.Contains(Input.MouseState.Position))
        {
            if (Input.WasPressed(MouseButton.Left))
            {
                if (aimingPath.Count > 0)
                {
                    HandleTarget(ent);
                }
                else
                {
                    if (ent.GetComponent<BehaviorController>() is BehaviorController behaviorController)
                    {
                        UpdateBehaviorController(behaviorController, ent, MouseTilePosition(Game.GraphicsDevice, Cam, offset.ToPoint()));
                    }

                    if (ent.GetComponent<EffectController>() is EffectController effectController)
                    {
                        UpdateEffectController(effectController, ent);
                    }

                    UpdateCollectables(ent, playerObj);
                }

                aimingPath.Clear();

                if (ent.GetComponent<SpellBook>() is SpellBook sb)
                    sb.selectedSpell = -1;
            }
            else if (ent.GetComponent<SpellBook>() is SpellBook spellBook)
            {
                //49 = Keys.D1
                for (int i = 49; i < 49 + 10; i++)
                {
                    if (Input.WasPressed((Keys)i))
                    {
                        if (spellBook.Spells.Count > i - 49)
                        {
                            if (spellBook.Spells[i - 49] is ISpell spell)
                            {
                                aimingPath.Clear();
                                spellBook.selectedSpell = i - 49;
                                aimingPath.AddRange(spell.Aim(ent));
                            }
                        }
                    }
                }
            }
        }
        Cam.Position = GetTileBounds(playerObj.MapX, playerObj.MapY).Location.ToVector2();
    }

    /// <summary>
    /// Handles targeting for spells and actions when the player has selected a target.
    /// Validates if the target is within range and performs the selected spell if valid.
    /// </summary>
    /// <param name="ent">The player entity casting the spell</param>
    private void HandleTarget(Entity ent)
    {
        var mouseTile = MouseTilePosition(Game.GraphicsDevice, Cam, offset.ToPoint());
        var mapObj = SceneManager.ComponentsOfType<MapObject>().FirstOrDefault(t => t.MapX == mouseTile.X && t.MapY == mouseTile.Y);
        var targetEnt = SceneManager.GetEntityWithComponent(mapObj);
        var stats = ent.GetComponent<CreatureStats>();
        if (targetEnt != null && aimingPath.Contains(mouseTile))
        //Vector2.Distance(new Vector2(playerObject.MapX, playerObject.MapY), mouseTile.ToVector2()) <= stats.AttackRange)
        {
            if (ent.GetComponent<SpellBook>() is SpellBook spellBook && spellBook.selectedSpell >= 0)
            {
                var spell = spellBook.Spells[spellBook.selectedSpell];
                spell.SetTarget(mouseTile);
                spell.TryPerform(ent, targetEnt);
                spellBook.selectedSpell = -1;
            }
        }
    }

    /// <summary>
    /// Updates non-player map objects such as enemies or NPCs.
    /// Handles AI behavior, effects, and spell casting based on distance to player.
    /// </summary>
    /// <param name="ent">Entity to update</param>
    /// <param name="mapObj">Map object component of the entity</param>
    /// <param name="player">Player's map object for reference</param>
    /// <param name="map">Current map for navigation and pathfinding</param>
    private void UpdateMapObject(Entity ent, MapObject mapObj, MapObject player, Map map)
    {
        if (ent.GetComponent<CreatureStats>() is not CreatureStats stats)
            return;

        var dist = Vector2.Distance(new Vector2(player.MapX, player.MapY), new Vector2(mapObj.MapX, mapObj.MapY));

        if (dist > stats.SightRange)
            return;

        if (ent.GetComponent<BehaviorController>() is BehaviorController behaviorController)
        {
            UpdateBehaviorController(behaviorController, ent, new Point(player.MapX, player.MapY), SceneManager.GetEntityWithComponent(player));
        }

        if (ent.GetComponent<EffectController>() is EffectController effectController)
        {
            UpdateEffectController(effectController, ent);
        }

        if (ent.GetComponent<SpellBook>() is SpellBook spellBook)
        {
            UpdateSpellBook(spellBook, ent, map, player, dist);
        }

    }

    /// <summary>
    /// Updates the behaviors of an entity using its behavior controller.
    /// Sets targeting information and attempts to perform each behavior.
    /// </summary>
    /// <param name="behaviorController">The behavior controller to update</param>
    /// <param name="ent">Entity that owns the behaviors</param>
    /// <param name="target">Target position for behaviors that require targeting</param>
    /// <param name="inflicted">Optional entity that will be affected by the behaviors</param>
    private void UpdateBehaviorController(BehaviorController behaviorController, Entity ent, Point target, Entity inflicted = null)//MapObject player)
    {
        foreach (var behavior in behaviorController.Behaviors)
        {
            if (behavior is ITarget targeter)
                targeter.SetTarget(target);

            behavior.TryPerform(ent, inflicted);//SceneManager.GetEntityWithComponent(player));
        }
    }

    /// <summary>
    /// Processes active effects on an entity, removing expired effects and applying behavior effects.
    /// </summary>
    /// <param name="effectController">The effect controller to update</param>
    /// <param name="ent">Entity that has the effects</param>
    private void UpdateEffectController(EffectController effectController, Entity ent)
    {
        var effectsToRemove = new List<IEffect>();
        foreach (var effect in effectController.Effects)
        {
            if (effect.TurnsLeft <= 0)
            {
                effectsToRemove.Add(effect);
                continue;
            }

            if (effect is IBehavior behavior)
                behavior.TryPerform(null, ent);
        }
        foreach (var effect in effectsToRemove)
        {
            effectController.Effects.Remove(effect);
        }
    }

    /// <summary>
    /// Updates an entity's spell book, checking if conditions are right to cast a spell at the player.
    /// Selects a random spell and attempts to cast it if the player is within range.
    /// </summary>
    /// <param name="spellBook">The spell book to update</param>
    /// <param name="ent">Entity that owns the spell book</param>
    /// <param name="map">Current map for navigation checks</param>
    /// <param name="player">Player's map object as potential target</param>
    /// <param name="distToPlayer">Distance from this entity to the player</param>
    private void UpdateSpellBook(SpellBook spellBook, Entity ent, Map map, MapObject player, float distToPlayer)
    {
        if (spellBook.Spells.Count > 0)
        {
            var targAdj =
                map.GetAdjacentEmptyTiles(
                    player.MapX,
                    player.MapY,
                    true,
                    SceneManager.ComponentsOfType<MapObject>().ToArray());

            if (targAdj.Any())
            {
                var spellIndex = MainGame.Rand.Next(0, spellBook.Spells.Count);
                var spell = spellBook.Spells[spellIndex];
                var spellInfo = Settings.SpellAtlas[spell.GetSpellType()];
                if (distToPlayer <= spellInfo.Range)
                {
                    spell.SetTarget(new Point(player.MapX, player.MapY));
                    spellBook.Spells[spellIndex].TryPerform(ent, SceneManager.GetEntityWithComponent(player));
                }
            }

        }
    }

    /// <summary>
    /// Handles collection of items that are on the same tile as the entity.
    /// </summary>
    /// <param name="ent">Entity attempting to collect items</param>
    /// <param name="mapObj">Map object of the collecting entity</param>
    private void UpdateCollectables(Entity ent, MapObject mapObj)
    {
        //collect
        var collectables = SceneManager
            .ComponentsOfType<MapObject>()
            .Where(t => t.MapX == mapObj.MapX && t.MapY == mapObj.MapY && Settings.MapObjectAtlas[t.Type].Collectable)
            .ToArray();

        if (collectables.Any())
        {
            var collect = new Collect();
            for (int i = 0; i < collectables.Length; i++)
            {
                var collectEnt = SceneManager.GetEntityWithComponent(collectables[i]);

                if (collect.TryPerform(ent, collectEnt))
                    entitiesToRemove.Add(collectEnt);
            }
        }
    }


    #region Drawing
    /// <summary>
    /// Renders the map, entities, and visual effects in the world
    /// </summary>
    /// <param name="gameTime">Provides timing information for the game</param>
    /// <param name="entities">All entities to render</param>
    public override void Draw(GameTime gameTime, IEnumerable<Entity> entities)
    {
        //DRAWING
        Matrix? transformMatrix = SceneManager.CurrentScene == "Play" ? Cam.Transform(Game.GraphicsDevice) : null;

        sb.Begin(
            sortMode: SpriteSortMode.FrontToBack,
            blendState: BlendState.NonPremultiplied,
            samplerState: SamplerState.PointClamp,
            depthStencilState: DepthStencilState.DepthRead,
            rasterizerState: RasterizerState.CullCounterClockwise,
            effect: null,
            transformMatrix: transformMatrix);


        PlayerSight.Draw(gameTime, entities);

        foreach (var ent in entities)
        {
            if (ent.HasTag("Map"))
            {
                DrawMap(ent, PlayerSight.ViewMap);
            }
            else if (ent.GetComponent<MapObject>() is MapObject mapObj)
            {
                float tintMod = 1f;
                if (PlayerSight.ViewMap.GetLength(0) > 0 && PlayerSight.ViewMap.GetLength(1) > 0)
                {
                    var distSqr = (float)Math.Sqrt(PlayerSight.ViewMap[mapObj.MapX, mapObj.MapY]) / PlayerSight. LastSightRange;
                    tintMod = MathHelper.Lerp(PlayerSight.LastSightRange, 0f, distSqr);
                }
                DrawMapObject(mapObj, PlayerSight.ViewMap, tintMod, ent.HasTag("Player"));

                if (ent.GetComponent<Wardrobe>() is Wardrobe wardrobe)
                {
                    DrawWardrobe(wardrobe, mapObj);
                }
            }
        }

        sb.End();

    }

    /// <summary>
    /// Draws a character's wardrobe items on top of their base sprite.
    /// </summary>
    /// <param name="wardrobe">The wardrobe component containing equipped items</param>
    /// <param name="mapObj">The map object to position the wardrobe items on</param>
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
                ITEM_LAYER + 0.1f);
        }
    }

    /// <summary>
    /// Renders the map tiles and handles mouse hover effects and spell targeting highlights.
    /// </summary>
    /// <param name="ent">The map entity</param>
    /// <param name="viewMap">Current visibility map from player's perspective</param>
    private void DrawMap(Entity ent, float[,] viewMap)
    {

        var map = ent.GetComponent<Map>();
        if (map == null) throw new System.Exception("Entity tagged with 'Map' must have map component");
        MapObject hoverObj = null;
        // Get mouse position in world space
        var mousePos = Input.MouseState.Position.ToVector2();
        var invertedMatrix = Matrix.Invert(Cam.Transform(Game.GraphicsDevice));
        var transformedMousePos = Vector2.Transform(mousePos, invertedMatrix);

        // Check if world-space mouse position is within the bounds of the map
        var worldBounds = new Rectangle(
            offset.ToPoint(),
            new Point(
                map.GroundTiles.GetLength(0) * Settings.TileSize,
                map.GroundTiles.GetLength(1) * Settings.TileSize
            )
        );

        if (worldBounds.Contains(transformedMousePos.ToPoint()))
        {
            var mouseTile = MouseTilePosition(Game.GraphicsDevice, Cam, offset.ToPoint());
            hoverObj = SceneManager.ComponentsOfType<MapObject>()
                .Where(t => t.MapX == mouseTile.X && t.MapY == mouseTile.Y)
                .FirstOrDefault();
        }

        //ground tiles
        for (int x = 0; x < map.GroundTiles.GetLength(0); x++)
        {
            for (int y = 0; y < map.GroundTiles.GetLength(1); y++)
            {
                if (viewMap.GetLength(0) == 0)
                    viewMap = PlayerSight.LastViewMap;

                //draw tile if visible
                if (viewMap.GetLength(0) > 0 && viewMap[x, y] != float.MaxValue)
                {
                    var distSqr = (float)Math.Sqrt(viewMap[x, y]) / PlayerSight.LastSightRange;
                    var tintMod = MathHelper.Lerp(PlayerSight.LastSightRange, 0f, distSqr);

                    var tileBounds = DrawTile(map.GroundTiles[x, y], GROUND_LAYER, tintMod / PlayerSight.LastSightRange);

                    //if aiming
                    if (aimingPath.Contains(new Point(x, y)))
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
                                    if (hoverEnt.GetComponent<CreatureStats>() is CreatureStats monsterStats)//&&
                                                                                                             //Vector2.Distance(
                                                                                                             //    new Vector2(playerObj.MapX, playerObj.MapY),
                                                                                                             //    new Vector2(hoverObj.MapX, hoverObj.MapY)) <= playerStats.AttackRange)
                                    {
                                        tint = new Color(1f, 0f, 0f, 0.5f);
                                    }
                                }

                            }

                            if (tint.HasValue)
                                DrawTileHighlight(tint.Value, tileBounds, GROUND_LAYER + 0.1f);
                        }
                        else
                        {
                            DrawTileHighlight(new Color(0f, 1f, 0f, 0.25f), tileBounds, GROUND_LAYER + 0.1f);
                        }
                    }

                }
                else
                {

                    //var mousePos = Input.MouseState.Position.ToVector2();
                    //var invertedMatrix = Matrix.Invert(Cam.Transform(Game.GraphicsDevice));
                    //var transformedMousePos = Vector2.Transform(mousePos, invertedMatrix);
                    var tileBounds = new Rectangle(transformedMousePos.ToPoint(), new Point(16, 16));
                    sb.Draw(cursorTexture, tileBounds, Color.White);

                }
            }
        }

        //object tiles
        for (int i = 0; i < map.ObjectTiles.Count; i++)
        {
            if (viewMap[map.ObjectTiles[i].X, map.ObjectTiles[i].Y] != float.MaxValue)
            {
                var distSqr = (float)Math.Sqrt(viewMap[map.ObjectTiles[i].X, map.ObjectTiles[i].Y]) / PlayerSight.LastSightRange;
                var tintMod = MathHelper.Lerp(PlayerSight.LastSightRange, 0f, distSqr);
                DrawTile(map.ObjectTiles[i], OBJECT_LAYER, tintMod / PlayerSight.LastSightRange);
            }
        }
    }


    /// <summary>
    /// Draws a map object like a character or item with proper scaling and visibility.
    /// </summary>
    /// <param name="mapObject">The map object to draw</param>
    /// <param name="viewMap">Current visibility map from player's perspective</param>
    /// <param name="tint">Visibility tint factor based on distance from player</param>
    /// <param name="isPlayer">Whether this object is the player</param>
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
            ITEM_LAYER);

    }

    /// <summary>
    /// Draws a tile with proper tinting based on visibility and handles mouse hover highlighting.
    /// </summary>
    /// <param name="tile">The tile to draw</param>
    /// <param name="layer">Render depth layer</param>
    /// <param name="tintMod">Tint modifier for visibility effects</param>
    /// <returns>The rendered bounds of the tile</returns>
    private Rectangle DrawTile(Tile tile, float layer, float tintMod)
    {
        var tileInfo = Settings.TileAtlas[tile.Type];
        var texture = Settings.TextureAtlas[tileInfo.TextureName];

        var bnds = GetTileBounds(tile.X, tile.Y);

        sb.Draw(texture, bnds, tileInfo.Source, tile.Tint * tintMod, 0f, Vector2.Zero, SpriteEffects.None, layer);

        // Calculate mouse position in world space
        var mousePos = Input.MouseState.Position.ToVector2();
        var invertedMatrix = Matrix.Invert(Cam.Transform(Game.GraphicsDevice));
        var transformedMousePos = Vector2.Transform(mousePos, invertedMatrix);

        // Check if the transformed mouse position is within bounds
        if (bnds.Contains(transformedMousePos))
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


    /// <summary>
    /// Draws a highlight overlay on a tile, used for targeting and hover effects.
    /// </summary>
    /// <param name="tint">Color of the highlight</param>
    /// <param name="bounds">Area to highlight</param>
    /// <param name="layer">Render depth layer</param>
    private void DrawTileHighlight(Color tint, Rectangle bounds, float layer)
    {
        sb.Draw(Settings.TextureAtlas["_pixel"], bounds, null, tint, 0f, Vector2.Zero, SpriteEffects.None, layer);
    }

    #endregion


    #region Helpers 
    /// <summary>
    /// Gets the pixel bounds of the map area on screen.
    /// </summary>
    private Rectangle MapPixelBounds => new(Game.Width / 5, 0, (Game.Width / 5) * 3, Game.Height);

    /// <summary>
    /// Converts a grid position to world-space rectangle bounds.
    /// </summary>
    /// <param name="pos">Grid position</param>
    /// <returns>Rectangle representing the tile bounds in world space</returns>
    private Rectangle GetTileBounds(Point pos) => GetTileBounds(pos.X, pos.Y);

    /// <summary>
    /// Converts grid coordinates to world-space rectangle bounds.
    /// </summary>
    /// <param name="x">Grid X coordinate</param>
    /// <param name="y">Grid Y coordinate</param>
    /// <returns>Rectangle representing the tile bounds in world space</returns>
    private Rectangle GetTileBounds(int x, int y) =>
        new(offset.ToPoint() + new Point(x * Settings.TileSize, y * Settings.TileSize),
            new Point(Settings.TileSize, Settings.TileSize));


    /// <summary>
    /// Converts a mouse position to grid coordinates, handling camera transformation.
    /// </summary>
    /// <param name="graphicsDevice">Graphics device for screen transformations</param>
    /// <param name="camera">Current camera for world-to-screen transformations</param>
    /// <param name="offset">Optional offset to apply to coordinates</param>
    /// <returns>Grid coordinates where the mouse is pointing</returns>
    public static Point MouseTilePosition(GraphicsDevice graphicsDevice, Camera camera, Point? offset = null)
    {
        var mousePos = Input.MouseState.Position.ToVector2();
        var invertedMatrix = Matrix.Invert(camera.Transform(graphicsDevice));
        var worldPos = Vector2.Transform(mousePos, invertedMatrix);

        offset ??= Point.Zero;

        return new Point(
            (int)((worldPos.X - offset.Value.X) / Settings.TileSize),
            (int)((worldPos.Y - offset.Value.Y) / Settings.TileSize)
        );

    }

    #endregion
}