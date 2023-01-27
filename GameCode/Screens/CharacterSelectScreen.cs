using Engine;
using GameCode.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens.Transitions;
using System.Collections.Generic;
using System.Linq;

namespace GameCode.Screens;

public class CharacterSelectScreen : BaseScreen
{
    public List<Character> PlayableCharacters { get; set; }
    public int selected = 0;
    public Button selectButton;
    public List<CharacterInfoCard> CharacterCards { get; set; }

    public CharacterSelectScreen(Game game) : base(game, "consolas_14")
    {
        PlayableCharacters = new List<Character>();
    }

    public override void LoadContent()
    {
        base.LoadContent();

        var cursor = new Cursor(BGame);
        EntityManager.AddEntity(cursor);

        selectButton = new Button(BGame)
        {
            Color = Color.Orange,
            HighlightColor = Color.DarkGray,
            TextColor = Color.Red,
            HighlightTextColor = Color.White,
            Filled = true,
            Font = Font,
            Text = "Choose",
            TextScale = 1f,
            Rect = new Rectangle(Game.GraphicsDevice.Viewport.Width / 2 - 50, 150, 100, 40),
            TextOffset = new Point(18, 12),
        };
        EntityManager.AddEntity(selectButton);
        selectButton.OnClick += SelectButton_OnClick;


        PlayableCharacters =
            TileLoader.GetTileObjects(t =>
                t is Character chr &&
                chr.Flags.Contains("Playable"))
            .Cast<Character>()
            .ToList();

        CharacterCards = new List<CharacterInfoCard>();
        
        for (int i = 0; i < PlayableCharacters.Count; i++)
        {
            var chr = PlayableCharacters[i];
            CharacterCards.Add(
                new CharacterInfoCard(
                    BGame, 
                    new Rectangle(200 + (i * 32 * 5), 400, 32 * 4, 32 * 6), 
                    chr.Sprite, 
                    Font,
                    Font,
                    chr.Name, 
                    $"{chr.Race} {chr.Class}")); 
        }
    }

    private void SelectButton_OnClick(object sender, ClickEventArgs e)
    {
        ScreenManager.LoadScreen(new PlayScreen(Game, PlayableCharacters[selected]), new FadeTransition(GraphicsDevice, Color.Black, 2f));
    }

    public override void Draw(GameTime gameTime)
    {

        BGame.SpriteBatch.Begin(
            sortMode: SpriteSortMode.FrontToBack,
            samplerState: SamplerState.PointClamp);

        for (int i = 0; i < CharacterCards.Count; i++)
        {
            CharacterCards[i].Draw(BGame.SpriteBatch);
        }

        BGame.SpriteBatch.End();

        base.Draw(gameTime);

    }
}