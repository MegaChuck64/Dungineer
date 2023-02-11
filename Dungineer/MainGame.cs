using Engine;
using Engine.Components;
using Engine.Prefabs;
using Engine.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Dungineer;

public class MainGame : BaseGame
{
    public const string MenuScene = "Menu";
    public const string CharacterSelectScene = "CharacterSelect";
    public const string PlayScene = "Play";
    public MainGame() : base(new System.Random().Next())
    {
        BackgroundColor = Color.MonoGameOrange;
        IsMouseVisible = true;
    }
    public override void Init()
    {
        //ToggleFullscreen();

    }
    public override void Load(ContentManager content)
    {
        AddSystems();


        BuildMenuScene();
        BuildCharacterSelectScene();

        BackgroundColor = new Color(10, 10, 10);
        SceneManager.ChangeScene(MenuScene);
    }

    private void AddSystems()
    {
        //mouse input
        Systems.Add(new MouseInputSystem(this));


        //sprites
        var textureNames = new string[]
        {
            "ghost_32",
            "GnomeMage_32",
            "grounds_32",
            "hand_32",
            "HumanFighter_32",
            "trees_32",
            "ui_box_select_32",
            "weapons_32",
            "WizardPortraits_512"
        };
        Systems.Add(new SpriteRenderSystem(this, Content, textureNames));


        //fonts
        var fontNames = new string[]
        {
            "consolas_12",
            "consolas_14",
            "consolas_22"
        };
        Systems.Add(new FontRenderSystem(this, Content, fontNames));

    }
    private void BuildMenuScene()
    {
        SceneManager.AddScene(MenuScene);

        //play button
        var playButtonPrefab = new ButtonPrefab(
            defaultColor:   new Color(50, 50, 50),
            hoverColor:     new Color(75, 75, 75), 
            pressedColor:   new Color(90, 90, 90),
            txtColor:       Color.White, 
            fontName:       "consolas_22", 
            text:           "Play");

        var playButton = playButtonPrefab.Instantiate(this);
        var plytrns = playButton.GetComponent<Transform>();
        plytrns.Position = new Vector2(Width / 2f - plytrns.Size.X / 2f, Height / 2f + 100);
        playButton.GetComponent<Text>().Offset = new Vector2(19, 12);
        playButton.GetComponent<MouseInput>().OnMouseReleased = (mb) => 
        {
            SceneManager.ChangeScene(CharacterSelectScene);
            BackgroundColor = new Color(25, 25, 25);
        };
        SceneManager.AddEntity(MenuScene, playButton);

        //wizard portraits
        var portrait = new Entity(this);
        var pTrns = new Transform(portrait)
        {
            Position = new Vector2(Width / 2f - 256, 10),
            Size = new Vector2(512, 512)
        };
        var pspr = new Sprite(portrait)
        {
            TextureName = "WizardPortraits_512",
            Tint = Color.White,
            Source = new Rectangle(Rand.Next(7) * 512, 0, 512, 512),            
        };
        SceneManager.AddEntity(MenuScene, portrait);
    }

    private void BuildCharacterSelectScene()
    {
        SceneManager.AddScene(CharacterSelectScene);

        //play button
        var backButtonPrefab = new ButtonPrefab(
            defaultColor: new Color(50, 50, 50),
            hoverColor: new Color(75, 75, 75),
            pressedColor: new Color(90, 90, 90),
            txtColor: Color.White,
            fontName: "consolas_22",
            text: "Back");

        var backButton = backButtonPrefab.Instantiate(this);
        var backTrn = backButton.GetComponent<Transform>();
        backTrn.Position = new Vector2(Width / 2f - backTrn.Size.X / 2f, Height / 2f + 100);
        backButton.GetComponent<Text>().Offset = new Vector2(19, 12);
        backButton.GetComponent<MouseInput>().OnMouseReleased = (mb) =>
        {
            SceneManager.ChangeScene(MenuScene);
            BackgroundColor = new Color(10, 10, 10);
        };
        SceneManager.AddEntity(CharacterSelectScene, backButton);

        //wizard portraits
        var portrait = new Entity(this);
        var pTrns = new Transform(portrait)
        {
            Position = new Vector2(Width / 2f - 256, 10),
            Size = new Vector2(512, 512)
        };
        var pspr = new Sprite(portrait)
        {
            TextureName = "WizardPortraits_512",
            Tint = Color.White,
            Source = new Rectangle(Rand.Next(7) * 512, 0, 512, 512),
        };
        SceneManager.AddEntity(CharacterSelectScene, portrait);
    }

    public override void OnUpdate(GameTime gameTime)
    {

    }

    public override void OnDraw(GameTime gameTime)
    {
        
    }
}