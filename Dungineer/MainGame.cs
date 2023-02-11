using Engine;
using Engine.Components;
using Engine.Prefabs;
using Engine.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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
        BackgroundColor = new Color(10, 10, 10);
        SceneManager.AddScene(MenuScene);

        var playButtonPrefab = new ButtonPrefab(
            defaultColor:   new Color(50, 50, 50),
            hoverColor:     new Color(75, 75, 75), 
            pressedColor:   new Color(90, 90, 90),
            txtColor:       Color.White, 
            fontName:       "consolas_22", 
            text:           "Play");


        var playButton = playButtonPrefab.Instantiate(this);
        var plytrns = playButton.GetComponent<Transform>();
        plytrns.Position = new Vector2(Width / 2f - plytrns.Size.X / 2f, Height / 2f);
        playButton.GetComponent<Text>().Offset = new Vector2(19, 12);
        playButton.GetComponent<MouseInput>().OnMouseReleased = (mb) => 
        {
            SceneManager.ChangeScene(CharacterSelectScene);
            BackgroundColor = new Color(25, 25, 25);
        };
        SceneManager.AddEntity(MenuScene, playButton);
    }

    public override void OnUpdate(GameTime gameTime)
    {
        //if (KeyState.WasKeyJustDown(Microsoft.Xna.Framework.Input.Keys.OemTilde)) //todo
        //{
        //    Debug = !Debug;
        //}
    }

    public override void OnDraw(GameTime gameTime)
    {
        if (!Debug) return;


    }
}