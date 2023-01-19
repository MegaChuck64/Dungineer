using Engine;
using GameCode.Entities;
using Microsoft.Xna.Framework;
namespace GameCode.Screens;

public class TestScreen : BaseScreen
{
    MapPrinter printer;
    Map map;
    MapCharacter player;
    public TestScreen(Game game) : base(game, "consolas_22")
    {
    }

    public override void LoadContent()
    {
        base.LoadContent();

        var cursor = new Cursor(BGame);
        EntityManager.AddEntity(cursor);

        map = new Map(25, 25, BGame.Rand);

        printer = new MapPrinter(BGame)
        {
            Map = map,
            Camera = Camera,
            TileSize = 32,
        };

        EntityManager.AddEntity(printer);

        var playerPos = map.GetRandomEmptyTile();
        player = new MapCharacter()
        {
            Name = "Human Fighter",
            Description = "A simple fighter.",
            Class = "Fighter",
            Race = "Human",
            Health = 10,
            MaxHealth = 10,
            HealthRegen = 3,
            Stamina = 4,
            MaxStamina = 4,
            StaminaRegen = 2,
            Strength = 7,
            Speed = 2,
            Armor = .2f,
            X = playerPos.x,
            Y = playerPos.y,
            Solid = true,
        };
        map.Objects.Add(player);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

    }
}