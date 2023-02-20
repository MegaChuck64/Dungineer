using Dungineer.Components;
using Engine;
using Engine.Components;
using Engine.Prefabs;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

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

        var selectBtn = 
            new ButtonPrefab(Color.Gray, Color.DarkSlateGray, Color.SlateGray, Color.Yellow, "consolas_22", "Play")
            .Instantiate(game);

        selectBtn.GetComponent<MouseInput>().OnMouseReleased = (mb) =>
        {
            SceneManager.ChangeScene("Play");
            SceneManager.AddEntity("Play", player);
        };

        var btnTran = selectBtn.GetComponent<Transform>();
        btnTran.Position = new Vector2(game.Width / 2f - btnTran.Size.X / 2f, game.Height - (btnTran.Size.Y) - (10 * game.WindowRatio));
        btnTran.Layer = 0.7f;
        selectBtn.GetComponent<Text>().Offset = new Vector2(19, 12);

        ents.Add(selectBtn);

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
                Size = new Vector2(64, 64)
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
                Size = new Vector2(64, 64)
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