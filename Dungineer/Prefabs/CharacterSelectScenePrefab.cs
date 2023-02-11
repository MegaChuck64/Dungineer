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
        var descEnt = new Entity(game);
        var descTrn = new Transform(descEnt)
        {
            Position = new Vector2(game.Width / 2f - 600, game.Height / 2f),
            Size = new Vector2(100, 100),
            Layer = 0.6f,
        };
        var descText = new Text(descEnt)
        {
            Content = string.Empty,
            FontName = "consolas_14",
            Tint = Color.White,
        };
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

            var ent = AddCharcterChoice(
                game,
                "WizardPortraits_512",
                new Rectangle(rnd * 512, 0, 512, 512),
                new Rectangle(10 + (256 * i), 10, 256, 256),
                wizardName,
                wizardDescription,
                descText);
            entities.Add(ent);
        }

        //instructions
        var instrEnt = new Entity(game);
        var instrTran = new Transform(instrEnt)
        {
            Position = new Vector2(game.Width / 2 - 140, game.Height / 2 + 300),
            Size = new Vector2(game.Width, game.Height),
            Layer = 0.6f
        };
        var instrTxt = new Text(instrEnt)
        {
            FontName = "consolas_14",
            Tint = Color.Orange,
            Content = "Select one of the wizards above."
        };
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
        backTrn.Position = new Vector2(game.Width / 2f - backTrn.Size.X / 2f, game.Height / 2f + 360);
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


    private Entity AddCharcterChoice(BaseGame game, string textureName, Rectangle source, Rectangle bounds, string wizardName, string wizardDesc, Text descText)
    {

        //wizard card
        var card = new Entity(game);
        //transform
        var pTrns = new Transform(card)
        {
            Position = bounds.Location.ToVector2(),
            Size = bounds.Size.ToVector2(),
            Layer = 0.7f,
        };
        //sprite
        var pspr = new Sprite(card)
        {
            TextureName = textureName,
            Tint = Color.White,
            Source = source,
        };
        //mouse input
        var pMI = new MouseInput(card)
        {
            OnMouseEnter = () =>
            {
                pspr.Tint = new Color(255, 255, 0, 150);
                descText.Content = wizardDesc;
            },

            OnMouseLeave = () =>
            {
                pspr.Tint = Color.White;
            },

            OnMouseReleased = (mb) =>
            {
                SceneManager.ChangeScene("Play");
                game.BackgroundColor = Color.Black;
            }
        };

        //name text
        var nameText = new Text(card)
        {
            Content = wizardName,
            FontName = "consolas_14",
            Tint = Color.OrangeRed,
            Offset = new Vector2(bounds.Width / 2 - 45, bounds.Height + 10)
        };

        return card;
    }
}