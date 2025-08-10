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

    private readonly List<Entity> _entitiesToRemove = [];
    private readonly List<Point> _aimingPath = [];
    private readonly Vector2 _offset;

    private readonly Rectangle _tileSource = new(0, 0, Settings.TileSize, Settings.TileSize);

    public IReadOnlyList<Point> AimingPath => _aimingPath.AsReadOnly();
    public Camera Cam { get; private set; }


    private readonly MapObjectType[] dropLottery = new MapObjectType[]
    {
        MapObjectType.Arcanium,
        MapObjectType.HealthPotion,
        MapObjectType.ManaPotion
    };


    /// <summary>
    /// Creates a new map system that manages the game world, handling tiles, objects and camera movement
    /// </summary>
    /// <param name="game">Main game instance providing access to graphics and dimensions</param>
    /// <param name="content">Content manager for loading textures and resources</param>
    public MapSystem(BaseGame game, ContentManager content) : base(game)
    {
        _offset = new Vector2(game.Width / 5, 0);

        Cam = new Camera();
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

        var map = entities.FirstOrDefault(t => t.HasTag("Map"))?.GetComponent<Map>();

        var playerEnt = entities.FirstOrDefault(e => e.HasTag("Player"));
        var playerMapObj = playerEnt?.GetComponent<MapObject>();
        var playerMoved = false;

        foreach (var ent in entities.Reverse()) //todo, reversing for now because we add the player last to the scene, but we want to process it first
        {
            var mapObj = ent.GetComponent<MapObject>();

            if (TryKillCreature(ent, mapObj))
                continue;
            if (ent == playerEnt)
            {
                playerMoved = UpdatePlayer(ent, mapObj);
            }
            else if (mapObj is not null)
            {
                if (playerMapObj == null)
                    continue;

                if (!playerMoved)
                    continue;

                if (MapPixelBounds.Contains(Input.MouseState.Position) == false) //todo: is this right? 
                    continue;

                UpdateMapObject(ent, mapObj, playerMapObj, map);
            }
            else if (ent.HasTag("Cursor"))
            {
                ent.GetComponent<UIElement>().IsActive = !MapPixelBounds.Contains(Input.MouseState.Position);
            }
        }

        foreach (var ent in _entitiesToRemove)
        {
            SceneManager.RemoveEntity(SceneManager.CurrentScene, ent);
        }
        _entitiesToRemove.Clear();
    }

  
    /// <summary>
    /// Checks if a creature's health is zero or negative and handles its death.
    /// Dead creatures are marked for removal and may drop items based on their drop chance.
    /// </summary>
    /// <param name="ent">Entity to check for death condition</param>
    /// <returns>True if the creature died and was processed, false otherwise</returns>
    private bool TryKillCreature(Entity ent, MapObject mapObj)
    {
        if (ent.GetComponent<CreatureStats>() is CreatureStats creatureStats)
        {
            if (creatureStats.Health <= 0)
            {
                creatureStats.Health = 0;
                _entitiesToRemove.Add(ent);

                if (mapObj is not null)
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
    /// Processes keyboard (WASD) input for movement, left clicks for actions and number keys for spell selection.
    /// </summary>
    /// <param name="ent">The player entity to update</param>
    /// <param name="mapObj">The map object component of the player entity</param>
    private bool UpdatePlayer(Entity ent, MapObject mapObj)
    {
        // Cache controllers at the beginning for reuse
        var behaviorController = ent.GetComponent<BehaviorController>();
        var effectController = ent.GetComponent<EffectController>();
        var spellBook = ent.GetComponent<SpellBook>();

        if (behaviorController is null || effectController is null || spellBook is null)
            return false;

        bool result = false;
        bool movementHandled = false;
        Point targetPosition = new Point(mapObj.MapX, mapObj.MapY);

        // Handle WASD keyboard movement
        if (Input.WasPressed(Keys.W))
        {
            targetPosition.Y--;
            movementHandled = true;
        }
        else if (Input.WasPressed(Keys.S))
        {
            targetPosition.Y++;
            movementHandled = true;
        }
        else if (Input.WasPressed(Keys.A))
        {
            targetPosition.X--;
            movementHandled = true;
        }
        else if (Input.WasPressed(Keys.D))
        {
            targetPosition.X++;
            movementHandled = true;
        }

        // Process player actions based on input
        if (movementHandled)
        {
            // Apply movement and update effects
            UpdateBehaviorController(behaviorController, ent, targetPosition);
            UpdateEffectController(effectController, ent);
            UpdateCollectables(ent, mapObj);
            HandleSpellSelection(ent, spellBook, true);
            result = true;
        }
        // Handle mouse-based interactions when in map bounds
        else if (MapPixelBounds.Contains(Input.MouseState.Position))
        {
            // Handle spell targeting and casting
            if (Input.WasPressed(MouseButton.Left))
            {
                var mouseTilePos = MouseTilePosition(Game.GraphicsDevice, Cam, _offset.ToPoint());

                if (_aimingPath.Count > 0)
                {
                    // We're targeting a spell
                    HandleTarget(ent, spellBook, mouseTilePos);
                }
                else
                {
                    // We're moving or performing a basic action
                    UpdateBehaviorController(behaviorController, ent, mouseTilePos);
                    UpdateEffectController(effectController, ent);
                    UpdateCollectables(ent, mapObj);
                }

                _aimingPath.Clear();
                spellBook.selectedSpell = -1;
                result = true;
            }
        }

        HandleSpellSelection(ent, spellBook);

        // Update camera to follow player
        Cam.Position = GetTileBounds(mapObj.MapX, mapObj.MapY).Location.ToVector2();

        return result;
    }

    /// <summary>
    /// Handles spell selection via number keys (1-9)
    /// </summary>
    /// <param name="ent">The player entity</param>
    /// <param name="spellBook">The player's spell book</param>
    private void HandleSpellSelection(Entity ent, SpellBook spellBook, bool move = false)
    {
        if (move && spellBook.selectedSpell >= 0)
            updatePath(spellBook.Spells[spellBook.selectedSpell]);
        else
        {
            // 49 = Keys.D1, check keys 1-9
            for (int i = 49; i < 49 + 9; i++)
            {
                if (Input.WasPressed((Keys)i))
                {
                    int spellIndex = i - 49;
                    if (spellBook.Spells.Count > spellIndex)
                    {
                        if (spellBook.Spells[spellIndex] is ISpell spell)
                        {
                            spellBook.selectedSpell = spellIndex;

                            updatePath(spell);
                        }
                    }
                }
            }
        }

        void updatePath(ISpell spell)
        {
            _aimingPath.Clear();
            _aimingPath.AddRange(spell.Aim(ent));
        }
    }

    /// <summary>
    /// Handles targeting for spells and actions when the player has selected a target.
    /// Validates if the target is within range and performs the selected spell if valid.
    /// </summary>
    /// <param name="ent">The player entity casting the spell</param>
    private void HandleTarget(Entity ent, SpellBook spellBook, Point mouseTilePos)
    {
        var mapObj = SceneManager.ComponentsOfType<MapObject>().FirstOrDefault(t => t.MapX == mouseTilePos.X && t.MapY == mouseTilePos.Y);
        var targetEnt = SceneManager.GetEntityWithComponent(mapObj);
        //var stats = ent.GetComponent<CreatureStats>();
        if (targetEnt != null && _aimingPath.Contains(mouseTilePos))
        //Vector2.Distance(new Vector2(playerObject.MapX, playerObject.MapY), mouseTile.ToVector2()) <= stats.AttackRange)
        {
            if (spellBook is not null && spellBook.selectedSpell >= 0)
            {
                var spell = spellBook.Spells[spellBook.selectedSpell];
                spell.SetTarget(mouseTilePos);
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
                    _entitiesToRemove.Add(collectEnt);
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
       
    }


   


  

  

   
    #endregion


    #region Helpers 
    /// <summary>
    /// Gets the pixel bounds of the map area on screen.
    /// </summary>
    private Rectangle MapPixelBounds => new(Game.Width / 5, 0, (Game.Width / 5) * 3, Game.Height);

    /// <summary>
    /// Converts grid coordinates to world-space rectangle bounds.
    /// </summary>
    /// <param name="x">Grid X coordinate</param>
    /// <param name="y">Grid Y coordinate</param>
    /// <returns>Rectangle representing the tile bounds in world space</returns>
    private Rectangle GetTileBounds(int x, int y) =>
        new(location:  _offset.ToPoint() + new Point(x * Settings.TileSize, y * Settings.TileSize), 
            size: _tileSource.Size);


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