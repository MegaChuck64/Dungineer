using Dungineer.Components.GameWorld;
using Dungineer.Prefabs.Entities;
using Engine;
using Engine.Components;
using Engine.Prefabs;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Dungineer.Prefabs.Scenes;

public class CharacterCreationScene : IPrefab<List<Entity>>
{
    public List<Entity> Instantiate(BaseGame game)
    {
        var ents = new List<Entity>
        {
            new CursorPrefab().Instantiate(game),
        };
        var player = CreatePlayer(game);
        ents.Add(player);

        ents.AddRange(CreateRobes(game, player));

        var startButton = new ButtonPrefab(
             hoverColor: new Color(82, 82, 82),
             pressedColor: new Color(49, 49, 49),
             defaultColor: new Color(65, 65, 65),
             txtColor: new Color(202, 62, 71),
             fontName: "consolas_22",
             text: "Start")
            .Instantiate(game);

        startButton.GetComponent<MouseInput>()
            .OnMouseReleased = (mb) =>
            {
                SceneManager.ChangeScene("Play");
                SceneManager.AddEntity("Play", player);
            };

        var startButtonTransform = startButton.GetComponent<Transform>();
        startButtonTransform.Position = new Vector2((game.Width / 2) - 64, game.Height - (64 * 3));
        startButtonTransform.Size = new Vector2(128, 64);

        var startButtonText = startButton.GetComponent<Text>();
        startButtonText.Offset = new Vector2(24, 12);
        ents.Add(startButton);

        return ents;
    }

    private static List<Entity> CreateRobes(BaseGame game, Entity player)
    {
        var ents = new List<Entity>();

        var basicRobe = new Entity(game);
        var clearRobe = new Entity(game);
        
        clearRobe = new Entity(game)
            .With(new Transform
            {
                Position = new Vector2(game.Width / 2 + 100, game.Height / 2 - 100),
                Size = new Vector2(64, 64),
                Layer = 0.6f
            })
            .With(new Sprite
            {
                Source = new Rectangle(0, 0, 32, 32),
                TextureName = "symbols_32",
                Tint = Color.White,
            })
            .With(new MouseInput
            {
                OnMousePressed = (mb) =>
                {
                    var wardrobe = player.GetComponent<Wardrobe>();
                    wardrobe.BodySlot = null;
                    basicRobe.GetComponent<Sprite>().Tint = Color.White;
                    clearRobe.GetComponent<Sprite>().Tint = new Color(100, 250, 200);
                }
            });
        ents.Add(clearRobe);


        basicRobe = new Entity(game)
            .With(new Transform
            {
                Position = new Vector2(game.Width / 2 + 100, game.Height / 2),
                Size = new Vector2(64, 64),
                Layer = 0.6f
            })
            .With(new Sprite
            {
                Source = new Rectangle(0,0, 32, 32),
                TextureName = "robes_32",
                Tint = Color.White
            })
            .With(new MouseInput
            {
                OnMousePressed = (mb) =>
                {
                    var wardrbobe = player.GetComponent<Wardrobe>();
                    wardrbobe.BodySlot = WardrobeType.BasicRobe;
                    clearRobe.GetComponent<Sprite>().Tint = Color.White;
                    basicRobe.GetComponent<Sprite>().Tint = new Color(100, 250, 200);
                }
            });

        ents.Add(basicRobe);

        return ents;
    }
    
    private static Entity CreatePlayer(BaseGame game)
    {
        var ent = new Entity(game)
            .With(new MapObject
            {
                MapX = 5,
                MapY = 5,
                Tint = Color.White,
                Type = MapObjectType.Human,
            })
            .With(new CreatureStats
            {
                Health = 20,
                MaxHealth = 20,
                Stamina = 20,
                MaxStamina = 20,
                MoveSpeed = 1
            })
            .With(new Wardrobe
            {
                BodySlot = null,
                HatSlot = null,
            })
            .With(new Tag 
            { 
                Value = "Player" 
            });



        return ent;
    }
}