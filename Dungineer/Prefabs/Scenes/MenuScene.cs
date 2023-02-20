using Dungineer.Prefabs.Entities;
using Engine;
using Engine.Components;
using Engine.Prefabs;
using Microsoft.Xna.Framework;
using System.Collections.Generic;


namespace Dungineer.Prefabs.Scenes;

public class MenuScene : IPrefab<List<Entity>>
{
    public List<Entity> Instantiate(BaseGame game)
    {
        var ents = new List<Entity>
        {
            //wizard picture
            //play button
            new CursorPrefab().Instantiate(game)
        };
        /*
            Maybe a palette?

            #525252         rgb(82, 82, 82)  /grays
            #414141         rgb(65, 65, 65)
            #313131         rgb(49, 49, 49)                            
            #CA3E47         rgb(202, 62, 71) /red

        */
        var playButton = new ButtonPrefab(
             hoverColor: new Color(82, 82, 82),
             pressedColor: new Color(49, 49, 49),
             defaultColor: new Color(65, 65, 65),
             txtColor: new Color(202, 62, 71),
             fontName: "consolas_22",
             text: "Play")
            .Instantiate(game);

        playButton.GetComponent<MouseInput>()
            .OnMouseReleased = (mb) =>
            {
                SceneManager.ChangeScene("CharacterCreation");
            };

        var playTransform = playButton.GetComponent<Transform>();
        playTransform.Position = new Vector2((game.Width / 2) - 64, game.Height - (64 * 3));
        playTransform.Size = new Vector2(128, 64);

        var playTxt = playButton.GetComponent<Text>();
        playTxt.Offset = new Vector2(32, 12);
        ents.Add(playButton);

        return ents;
    }
}
