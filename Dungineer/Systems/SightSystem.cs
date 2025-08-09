using Dungineer.Components.GameWorld;
using Engine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Dungineer.Systems;

public class SightSystem : BaseSystem
{

    private float sightRangeBreathSpeed = 2f;
    private float sightRangeBreathTimer = 0f;
    private float sightRangeBreath;
    private float sightBreathOffset = 3f;
    private bool sightRangeBreathOut = false;

    public float LastSightRange { get; private set; }
    public float[,] ViewMap { get; private set; }
    public float[,] LastViewMap { get; private set; }

    public SightSystem(BaseGame game) : base(game)
    {
    }


    public override void Update(GameTime gameTime, IEnumerable<Entity> entities)
    {
        var playerEnt = entities.FirstOrDefault(e => e.HasTag("Player"));
        var playerMapObj = playerEnt?.GetComponent<MapObject>();

        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        sightRangeBreathTimer += dt;
        const float BREATH_STEP = 1f;

        if (sightRangeBreathTimer >= 1f / sightRangeBreathSpeed)
        {
            sightRangeBreathTimer = 0f;

            if (sightRangeBreath == 0)
            {
                if (sightRangeBreathOut)
                    sightRangeBreath += BREATH_STEP;
                else
                    sightRangeBreath -= BREATH_STEP;
            }
            else if (sightRangeBreath > 0)
            {
                if (sightRangeBreath < sightBreathOffset && !sightRangeBreathOut)
                    sightRangeBreath += BREATH_STEP;
                else
                {
                    sightRangeBreathOut = false;
                    sightRangeBreath -= BREATH_STEP;
                }
            }
            else if (sightRangeBreath < 0)
            {
                if (sightRangeBreath > -sightBreathOffset && !sightRangeBreathOut)
                    sightRangeBreath -= BREATH_STEP;
                else
                {
                    sightRangeBreathOut = true;
                    sightRangeBreath += BREATH_STEP;
                }
            }
        }
    }

    public override void Draw(GameTime gameTime, IEnumerable<Entity> entities)
    {
        var mapEnt = SceneManager.Entities.FirstOrDefault(t => t.HasTag("Map"));
        if (mapEnt == null)
        {
            ViewMap = new float[0, 0];
            return;
        }

        var map = mapEnt.GetComponent<Map>();

        var playerEnt = SceneManager.Entities.FirstOrDefault(t => t.HasTag("Player"));
        if (playerEnt != null)
        {
            var player = playerEnt.GetComponent<MapObject>();
            var playerStats = playerEnt.GetComponent<CreatureStats>();
            LastSightRange = playerStats.SightRange;
            ViewMap = GetViewMap(new Point(player.MapX, player.MapY), map, playerStats.SightRange + sightRangeBreath);
            LastViewMap = ViewMap;            
        }
        else
        {
            ViewMap = new float[0, 0];
        }
    }

    /// <summary>
    /// Calculates which tiles are visible from a given position with line-of-sight checks.
    /// Creates a visibility map that accounts for obstacles and sight radius.
    /// </summary>
    /// <param name="start">Starting position for the line-of-sight checks</param>
    /// <param name="map">Map to check visibility on</param>
    /// <param name="viewRadius">Maximum visibility radius</param>
    /// <param name="mapObjects">Map objects that might block visibility</param>
    /// <returns>2D array of float values representing visibility, with float.MaxValue for tiles not visible</returns>
    /// <remarks>
    /// TODO: Implement proper handling for decals vs. solid objects. Currently floor holes are treated as object tiles
    /// which might not be correct from a visibility standpoint.
    /// </remarks>
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

}