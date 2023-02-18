using Engine;
using Engine.Components;
using Engine.Prefabs;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Dungineer.Prefabs;
public class CharacterSelectScenePrefab : IPrefab<List<Entity>>
{
    public List<Entity> Instantiate(BaseGame game)
    {
        var entities = new List<Entity>();

        //cursor
        var cursorPrefab = new CursorPrefab();
        var cursor = cursorPrefab.Instantiate(game);
        entities.Add(cursor);


        //wizard description
        var descTxt = new Text
        {
            Content = string.Empty,
            FontName = "consolas_14",
            Tint = Color.White,
        };

        var descEnt = new Entity(game)
            .With(new Transform
            {
                Position = new Vector2(game.Width / 2f - (500 * game.WindowRatio), game.Height / 2f),
                Size = new Vector2(100, 100),
                Layer = 0.6f,
            })
            .With(descTxt);

        entities.Add(descEnt);

        //character select options
        var randWiz = new int[5];
        for (int i = 0; i < randWiz.Length; i++)
        {
            int rnd;
            do
            {
                rnd = game.Rand.Next(0, 8);
            } while (randWiz.Contains(rnd));
            randWiz[i] = rnd;
            var wizardNames = ContentLoader.LoadText("WizardNames.txt", game.Content);
            var wizardDescriptions = ContentLoader.LoadText("WizardDescriptions.txt", game.Content);
            
            var wizardName = wizardNames[randWiz[i]];
            var wizardDescription = wizardDescriptions.FirstOrDefault(t => t.StartsWith(wizardName))[(wizardName.Length + 2)..];
            var spaceSize = (game.Width / 5) * i + 10;

            var ent = AddCharcterChoice(
                game,
                "WizardPortraits_512",
                new Rectangle(rnd * 512, 0, 512, 512),
                new Rectangle((int)(spaceSize), (int)(10 * game.WindowRatio), 256, 256),
                wizardName,
                wizardDescription,
                descTxt,
                randWiz[i]);
            entities.Add(ent);
        }

        //instructions
        var instrEnt = new Entity(game)
            .With(new Transform
            {
                Position = new Vector2(game.Width / 2 - (140 * game.WindowRatio), game.Height / 2 + (300 * game.WindowRatio)),
                Size = new Vector2(game.Width, game.Height),
                Layer = 0.6f
            })
            .With(new Text
            {
                FontName = "consolas_14",
                Tint = Color.Orange,
                Content = "Select one of the wizards above."
            });
        entities.Add(instrEnt);


        //back button
        var backButtonPrefab = new ButtonPrefab(
            defaultColor: new Color(50, 50, 50),
            hoverColor: new Color(75, 75, 75),
            pressedColor: new Color(90, 90, 90),
            txtColor: Color.White,
            fontName: "consolas_22",
            text: "Back");

        var backButton = backButtonPrefab.Instantiate(game);
        var backTrn = backButton.GetComponent<Transform>();
        backTrn.Position = new Vector2(game.Width / 2f - backTrn.Size.X / 2f, game.Height  - backTrn.Size.Y - (10 * game.WindowRatio));
        backTrn.Layer = 0.7f;
        backButton.GetComponent<Text>().Offset = new Vector2(19, 12);
        backButton.GetComponent<MouseInput>().OnMouseReleased = (mb) =>
        {
            SceneManager.ChangeScene("Menu");
            game.BackgroundColor = new Color(10, 10, 10);
        };
        entities.Add(backButton);




        return entities;
    }


    private Entity AddCharcterChoice(BaseGame game, string textureName, Rectangle source, Rectangle bounds, string wizardName, string wizardDesc, Text descText, int potraitIndex)
    {
        var cardSpr = new Sprite
        {
            TextureName = textureName,
            Tint = Color.White,
            Source = source,
            Offset = Vector2.Zero,
        };


        //wizard card
        var card = new Entity(game)
            .With(new Transform
            {
                Position = bounds.Location.ToVector2(),
                Size = bounds.Size.ToVector2(),
                Layer = 0.7f,
            })
            .With(cardSpr)
            .With(new MouseInput
            {
                OnMouseEnter = () =>
                {
                    cardSpr.Tint = new Color(255, 255, 0, 150);
                    descText.Content = wizardDesc;
                },

                OnMouseLeave = () =>
                {
                    cardSpr.Tint = Color.White;
                },

                OnMouseReleased = (mb) =>
                {
                    SceneManager.ChangeScene("Play");
                    game.BackgroundColor = Color.Black;
                    SceneManager.AddEntity( //add player to play scene with chosen character data
                        "Play", 
                        new PlayerPrefab(
                            wizardName, 
                            wizardDesc, 
                            potraitIndex, 
                            potraitIndex)
                        .Instantiate(game));

                    SceneManager.AddEntity( //add player to play scene with chosen character data
                        "Play",
                        new MapPrefab().Instantiate(game));
                }
            })
            .With(new Text
            {
                Content = wizardName,
                FontName = "consolas_14",
                Tint = Color.OrangeRed,
                Offset = new Vector2(bounds.Width / 2 - (45 * game.WindowRatio), bounds.Height + (10 * game.WindowRatio))
            });

        return card;
    }
}