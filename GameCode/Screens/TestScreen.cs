using Engine;
using GameCode.Entities;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input;

namespace GameCode.Screens;

public class TestScreen : BaseScreen
{
    MapPrinter printer;
    Map map;
    Player player;
    const int tileSize = 32;
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
            TileSize = tileSize,
        };

        EntityManager.AddEntity(printer);

        var playerPos = map.GetRandomEmptyTile();

        var playerTile = new MapCharacter()
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
        map.Objects.Add(playerTile);

        player = new Player(BGame, map, playerTile);
        EntityManager.AddEntity(player);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (BGame.MouseState.WasButtonJustDown(MouseButton.Left))
        {
            var (targetX, targetY) = printer.WorldToMapPosition(BGame.MouseState.Position);
            player.Target(targetX, targetY);
        }
    }
}