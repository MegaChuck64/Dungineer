using Engine;
using GameCode.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Collections.Generic;
using System.Linq;

namespace GameCode.Screens;

public class CharacterSelectScreen : BaseScreen
{
    public List<Character> PlayableCharacters { get; set; }
    int selected = 0;
    public CharacterSelectScreen(Game game) : base(game, "consolas_14")
    {
        PlayableCharacters = new List<Character>();
    }

    public override void LoadContent()
    {
        base.LoadContent();
        PlayableCharacters = 
            TileLoader.TileObjects
            .Where(t => 
                t is Character chr && 
                chr.Flags.Contains("Playable"))
            .Cast<Character>()
            .ToList();


        var cursor = new Cursor(BGame);
        EntityManager.AddEntity(cursor);
    }

    public override void Draw(GameTime gameTime)
    {

        BGame.SpriteBatch.Begin(
            sortMode: SpriteSortMode.FrontToBack,
            samplerState: SamplerState.PointClamp);

        for (int i = 0; i < PlayableCharacters.Count; i++)
        {
            var chr = PlayableCharacters[i];
            
            //sprite
            var pos = new Point((10 * 32) + (i * 64 + 2), 10 * 32);
            if (i == selected)
            {
                BGame.SpriteBatch.FillRectangle(
                    new RectangleF(pos.X + 64 * i - 7, pos.Y, 78, 142), new Color(33, 33, 33, 100));
            }

            BGame.SpriteBatch.Draw(chr.Sprite, new Rectangle(pos, new Point(64, 64)), Color.White);

            //name
            var namePos = new Point(pos.X, pos.Y + 64 + 2);
            var nameSize = Font.MeasureString(chr.Name);

            var rectSize = nameSize + new Vector2(4, 4);
            var rectPos = new Point(namePos.X - 2, namePos.Y - 2);

           //BGame.SpriteBatch.FillRectangle(new RectangleF(rectPos, rectSize), Color.Gray);
            BGame.SpriteBatch.DrawString(Font, chr.Name, namePos.ToVector2(), Color.Yellow);

            //race 
            var racePos = new Point(pos.X, pos.Y + 64 + 2 + (int)rectSize.Y + 2);
            var raceSize = Font.MeasureString(chr.Race);

            var raceRectSize = raceSize + new Vector2(4, 4);
            var raceRectPos = new Point(racePos.X - 2, racePos.Y - 2);

            //BGame.SpriteBatch.FillRectangle(new RectangleF(raceRectPos, raceRectSize), Color.Gray);
            BGame.SpriteBatch.DrawString(Font, chr.Race, racePos.ToVector2(), Color.Yellow);

            //class
            var classPos = 
                new Point(pos.X, pos.Y + 64 + 2 + (int)rectSize.Y + 2 + (int)raceRectSize.Y + 2);
            var classSize = Font.MeasureString(chr.Class);

            var classRectSize = classSize + new Vector2(4, 4);
            var classRectPos = new Point(classPos.X - 2, classPos.Y - 2);

            //BGame.SpriteBatch.FillRectangle(new RectangleF(classRectPos, classRectSize), Color.Gray);
            BGame.SpriteBatch.DrawString(Font, chr.Class, classPos.ToVector2(), Color.Yellow);


        }

        BGame.SpriteBatch.End();

        base.Draw(gameTime);

    }
}