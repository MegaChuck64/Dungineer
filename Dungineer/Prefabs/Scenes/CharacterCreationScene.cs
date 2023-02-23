using Dungineer.Components.GameWorld;
using Dungineer.Components.UI;
using Dungineer.Models;
using Dungineer.Prefabs.Entities;
using Engine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Dungineer.Prefabs.Scenes;

public class CharacterCreationScene : IPrefab<List<Entity>>
{
    public List<Entity> Instantiate(BaseGame game)
    {
        var ents = new List<Entity>();

        var cursor = new CursorPrefab().Instantiate(game);
        ents.Add(cursor);

        var player = CreatePlayer(game);
        ents.Add(player);


        ents.Add(CreateRobe(game, player, "symbols_32", new Rectangle(0, 0, 32, 32), -100, null));
        ents.Add(CreateRobe(game, player, "robes_32", new Rectangle(0, 0, 32, 32), 0, WardrobeType.BasicRobe));


        ents.Add(CreateStartButton(game, player));

        return ents;
    }

    private static Entity CreateStartButton(BaseGame game, Entity player)
    {

        var btn = new Entity()
            .With(new UIElement
            {
                Position = new Point((game.Width / 2) - 64, game.Height - (64 * 3)),
                Size = new Point(128, 64),
                OnMouseReleased = (mb) =>
                {
                    SceneManager.ChangeScene("Play");
                    var playerObj = player.GetComponent<MapObject>();
                    playerObj.Scale = 1f;
                    var map = SceneManager.Entities.First(t => t.HasTag("Map")).GetComponent<Map>();
                    var mapObjs = SceneManager.ComponentsOfType<MapObject>();
                    var playerPos = map.GetRandomEmptyTile(game, mapObjs.ToArray());
                    playerObj.MapX = playerPos.x;
                    playerObj.MapY = playerPos.y;

                    SceneManager.AddEntity("Play", player);
                }
            })
            .With(new SelectItem
            {
                PressedColor = new Color(82, 82, 82),
                HoverColor = new Color(65, 65, 65),
                DefaultColor = new Color(49, 49, 49),
                SelectedColor = new Color(49, 49, 49),
            })
            .With(new Image
            {
                Layer = 0.8f,
                Position = Point.Zero,
                Size = new Point(1, 1),
                Source = new Rectangle(0, 0, 1, 1),
                TextureName = "_pixel",
                Tint = Color.White,
            })
            .With(new TextBox
            {
                FontName = "consolas_22",
                Text = "Start",
                TextColor = new Color(202, 62, 71),
                Layer = 0.9f,
            });

        return btn;

    }
    private static Entity CreateRobe(BaseGame game, Entity player, string textureName, Rectangle source, int yOffset, WardrobeType? wardrobeType)
    {
        var robe = new Entity()
            .With(new UIElement
            {
                Position = new Point(game.Width/2 + 100, game.Height/ 2 + yOffset),
                Size = new Point(64, 64),     
                OnMousePressed = (mb) =>
                {
                    var stats = player.GetComponent<CreatureStats>();
                    var wardrobe = player.GetComponent<Wardrobe>();

                    if (wardrobe.BodySlot != null)
                    {
                        var wardrobeInfo = Settings.WardrobeAtlas[wardrobe.BodySlot.Value];
                        stats.Money += wardrobeInfo.Cost;
                        stats.MaxHealth -= wardrobeInfo.HealthMod;
                    }
                    wardrobe.BodySlot = wardrobeType;

                    if (wardrobe.BodySlot != null)
                    {
                        var wardrobeInfo = Settings.WardrobeAtlas[wardrobe.BodySlot.Value];
                        stats.Money -= wardrobeInfo.Cost;
                        stats.MaxHealth += wardrobeInfo.HealthMod;
                    }

                },
            })
            .With(new Image
            {
                Layer = 0.8f,
                Position = Point.Zero,
                Size = new Point(1,1),
                TextureName = textureName,
                Source = source,
                Tint = Color.White
            }) 
            .With(new SelectItem
            {

                DefaultColor = Color.White,
                SelectedColor = new Color(50, 200, 100),
                SelectionGroup = "Robes",
                HoverColor = Color.White,
                PressedColor = Color.White,
            });



        return robe;
    }
   
    
    private static Entity CreatePlayer(BaseGame game)
    {
        var ent = new Entity()
            .With(new MapObject
            {
                MapX = 5,
                MapY = 16,
                Tint = Color.White,
                Type = MapObjectType.Human,
                Scale = 2,
            })
            .With(new CreatureStats
            {
                Health = 12,
                MaxHealth = 12,

                Stamina = 20,
                MaxStamina = 20,

                MoveSpeed = 1,
                
                Money = 5,
                
                SightRange = 8f,
                AttackRange = 2f,

                Strength = 3,
            })
            .With(new Wardrobe
            {
                BodySlot = null,
                HatSlot = null,
            })
            .WithTag("Player");



        return ent;
    }
}