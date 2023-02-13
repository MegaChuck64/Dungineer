using Dungineer.Prefabs;
using Dungineer.Systems;
using Engine;
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
        BackgroundColor = new Color(10, 10, 10);
        IsMouseVisible = false;
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
        BuildPlayScene();

        SceneManager.ChangeScene(MenuScene);
    }

    private void AddSystems()
    {
        //mouse input
        Systems.Add(new MouseInputSystem(this));


        Systems.Add(new MapSystem(this, Content));

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
            "WizardPortraits_512",
            "cursor_16"
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

        Systems.Add(new UISystem(this, "consolas_14"));

        Systems.Add(new PlayerSystem(this));


    }
    private void BuildMenuScene()
    {
        SceneManager.AddScene(MenuScene);

        var scenePrefab = new MenuScenePrefab();
        var ents = scenePrefab.Instantiate(this);
        foreach (var ent in ents)
        {
            SceneManager.AddEntity(MenuScene, ent);
        }
    }

    private void BuildCharacterSelectScene()
    {
        SceneManager.AddScene(CharacterSelectScene);

        var scenePrefab = new CharacterSelectScenePrefab();
        var ents = scenePrefab.Instantiate(this);
        foreach (var ent in ents)
        {
            SceneManager.AddEntity(CharacterSelectScene, ent);
        }
    }

    private void BuildPlayScene()
    {
        SceneManager.AddScene(PlayScene);

        var scenePrefab = new PlayScenePrefab();
        var ents = scenePrefab.Instantiate(this);
        foreach (var ent in ents)
        {
            SceneManager.AddEntity(PlayScene, ent);
        }
    }


    public override void OnUpdate(GameTime gameTime)
    {

    }

    public override void OnDraw(GameTime gameTime)
    {
        
    }
}