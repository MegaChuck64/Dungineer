using Engine;
using Engine.Components;
using Engine.Prefabs;
using Microsoft.Xna.Framework;
using System.Collections.Generic;


namespace Dungineer.Prefabs;

public class MenuScenePrefab : IPrefab<List<Entity>>
{
    public List<Entity> Instantiate(BaseGame game)
    {
        var ents = new List<Entity>();

        //cursor
        var cursorPrefab = new CursorPrefab();
        var cursor = cursorPrefab.Instantiate(game);
        ents.Add(cursor);

        //play button
        var playButtonPrefab = new ButtonPrefab(
            defaultColor: new Color(50, 50, 50),
            hoverColor: new Color(75, 75, 75),
            pressedColor: new Color(90, 90, 90),
            txtColor: Color.White,
            fontName: "consolas_22",
            text: "Play");

        var playButton = playButtonPrefab.Instantiate(game);
        var plytrns = playButton.GetComponent<Transform>();
        plytrns.Position = new Vector2(game.Width / 2f - plytrns.Size.X / 2f, game.Height / 2f + 100);
        plytrns.Layer = 0.7f;
        playButton.GetComponent<Text>().Offset = new Vector2(19, 12);
        playButton.GetComponent<MouseInput>().OnMouseReleased = (mb) =>
        {
            SceneManager.ChangeScene("CharacterSelect");
            game.BackgroundColor = new Color(25, 25, 25);
        };
        ents.Add(playButton);

        //wizard portraits
        var portrait = new Entity(game);
        var pTrns = new Transform(portrait)
        {
            Position = new Vector2(game.Width / 2f - 256, 10),
            Size = new Vector2(512, 512),
            Layer = 0.7f
        };
        var pspr = new Sprite(portrait)
        {
            TextureName = "WizardPortraits_512",
            Tint = Color.White,
            Source = new Rectangle(game.Rand.Next(7) * 512, 0, 512, 512),
        };
        ents.Add(portrait);




        return ents;
    }
}