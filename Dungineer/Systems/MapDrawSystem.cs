using Dungineer.Components.GameWorld;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungineer.Systems;

public class MapDrawSystem : BaseSystem
{
    private const float GROUND_LAYER = 0.5f;
    private const float OBJECT_LAYER = 0.6f;
    private const float EFFECT_LAYER = 0.7f;
    private const float ITEM_LAYER = 0.8f;


    private readonly SpriteBatch _sb;
    private readonly Texture2D _tileSelectTexture;
    private readonly Texture2D _cursorTexture;

    private readonly Vector2 _offset;
    private readonly Rectangle _tileSource = new(0, 0, Settings.TileSize, Settings.TileSize);

    private MapSystem _mapSystem;
    private SightSystem _sightSystem;
    public MapDrawSystem(BaseGame game, ContentManager content, MapSystem mapSystem, SightSystem sightSystem) : base(game)
    {
        _offset = new Vector2(game.Width / 5, 0);

        _sb = new SpriteBatch(game.GraphicsDevice);

        _tileSelectTexture = ContentLoader.LoadTexture("ui_box_select_32", content);
        _cursorTexture = ContentLoader.LoadTexture("cursor_16", content);
        _mapSystem = mapSystem;
        _sightSystem = sightSystem;
    }

    public override void Update(GameTime gameTime, IEnumerable<Entity> entities)
    {

    }

    public override void Draw(GameTime gameTime, IEnumerable<Entity> entities)
    {
        //DRAWING
        Matrix? transformMatrix = SceneManager.CurrentScene == "Play" ? _mapSystem.Cam.Transform(Game.GraphicsDevice) : null;

        _sb.Begin(
            sortMode: SpriteSortMode.FrontToBack,
            blendState: BlendState.NonPremultiplied,
            samplerState: SamplerState.PointClamp,
            depthStencilState: DepthStencilState.DepthRead,
            rasterizerState: RasterizerState.CullCounterClockwise,
            effect: null,
            transformMatrix: transformMatrix);


        foreach (var ent in entities)
        {
            if (ent.HasTag("Map"))
            {
                DrawMap(ent, _sightSystem.ViewMap);
            }
            else if (ent.GetComponent<MapObject>() is MapObject mapObj)
            {
                float tintMod = 1f;
                if (_sightSystem.ViewMap.GetLength(0) > 0 && _sightSystem.ViewMap.GetLength(1) > 0)
                {
                    var distSqr = (float)Math.Sqrt(_sightSystem.ViewMap[mapObj.MapX, mapObj.MapY]) / _sightSystem.LastSightRange;
                    tintMod = MathHelper.Lerp(_sightSystem.LastSightRange, 0f, distSqr);
                }
                DrawMapObject(mapObj, _sightSystem.ViewMap, tintMod, ent.HasTag("Player"));

                if (ent.GetComponent<Wardrobe>() is Wardrobe wardrobe)
                {
                    DrawWardrobe(wardrobe, mapObj);
                }
            }
        }

        _sb.End();

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
            _sb.Draw(
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
        var invertedMatrix = Matrix.Invert(_mapSystem.Cam.Transform(Game.GraphicsDevice));
        var transformedMousePos = Vector2.Transform(mousePos, invertedMatrix);

        // Check if world-space mouse position is within the bounds of the map
        var worldBounds = new Rectangle(
            _offset.ToPoint(),
            new Point(
                map.GroundTiles.GetLength(0) * Settings.TileSize,
                map.GroundTiles.GetLength(1) * Settings.TileSize
            )
        );

        if (worldBounds.Contains(transformedMousePos.ToPoint()))
        {
            var mouseTile = MapSystem.MouseTilePosition(Game.GraphicsDevice, _mapSystem.Cam, _offset.ToPoint());
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
                    viewMap = _sightSystem.LastViewMap;

                //draw tile if visible
                if (viewMap.GetLength(0) > 0 && viewMap[x, y] != float.MaxValue)
                {
                    var distSqr = (float)Math.Sqrt(viewMap[x, y]) / _sightSystem.LastSightRange;
                    var tintMod = MathHelper.Lerp(_sightSystem.LastSightRange, 0f, distSqr);

                    var tileBounds = DrawTile(map.GroundTiles[x, y], GROUND_LAYER, tintMod / _sightSystem.LastSightRange);

                    //if aiming
                    if (_mapSystem.AimingPath.Contains(new Point(x, y)))
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
                                    if (hoverEnt.GetComponent<CreatureStats>() is CreatureStats monsterStats)
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
            }
        }

        //object tiles
        for (int i = 0; i < map.ObjectTiles.Count; i++)
        {
            if (viewMap[map.ObjectTiles[i].X, map.ObjectTiles[i].Y] != float.MaxValue)
            {
                var distSqr = (float)Math.Sqrt(viewMap[map.ObjectTiles[i].X, map.ObjectTiles[i].Y]) / _sightSystem.LastSightRange;
                var tintMod = MathHelper.Lerp(_sightSystem.LastSightRange, 0f, distSqr);
                DrawTile(map.ObjectTiles[i], OBJECT_LAYER, tintMod / _sightSystem.LastSightRange);
            }
        }

        var cursorBounds = new Rectangle(transformedMousePos.ToPoint(), new Point(16, 16));
        _sb.Draw(_cursorTexture, cursorBounds, Color.White);
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

        _sb.Draw(
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

        _sb.Draw(texture, bnds, tileInfo.Source, tile.Tint * tintMod, 0f, Vector2.Zero, SpriteEffects.None, layer);

        // Calculate mouse position in world space
        var mousePos = Input.MouseState.Position.ToVector2();
        var invertedMatrix = Matrix.Invert(_mapSystem.Cam.Transform(Game.GraphicsDevice));
        var transformedMousePos = Vector2.Transform(mousePos, invertedMatrix);

        // Check if the transformed mouse position is within bounds
        if (bnds.Contains(transformedMousePos))
        {
            _sb.Draw(
                _tileSelectTexture,
                bnds,
                _tileSource,
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
        _sb.Draw(Settings.TextureAtlas["_pixel"], bounds, null, tint, 0f, Vector2.Zero, SpriteEffects.None, layer);
    }


    /// <summary>
    /// Converts grid coordinates to world-space rectangle bounds.
    /// </summary>
    /// <param name="x">Grid X coordinate</param>
    /// <param name="y">Grid Y coordinate</param>
    /// <returns>Rectangle representing the tile bounds in world space</returns>
    private Rectangle GetTileBounds(int x, int y) =>
        new(location: _offset.ToPoint() + new Point(x * Settings.TileSize, y * Settings.TileSize),
            size: _tileSource.Size);
}