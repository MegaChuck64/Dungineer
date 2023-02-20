using Dungineer.Prefabs;
using Dungineer.Prefabs.Scenes;
using Dungineer.Systems;
using Engine;
using Engine.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Dungineer;

public class MainGame : BaseGame
{
    //1440x896
    public MainGame() : base(Settings.Seed, Settings.TileSize * 45, Settings.TileSize * 28)
    {
        BackgroundColor = new Color(20, 20, 20);
        IsMouseVisible = false;
    }

    public override void Init()
    {
        //ToggleFullscreen();
    }

    public override void Load(ContentManager content)
    {
        Settings.LoadTileAtlas(content);
        Settings.LoadMapObjectAtlas(content);
        Settings.LoadWardrobeAtlas(content);

        AddSystems();

        BuildMenuScene();
        BuildCharacterSelectScene();
        BuildPlayScene();

        SceneManager.ChangeScene("Menu");
    }

    private void AddSystems()
    {
        //mouse input
        Systems.Add(new MouseInputSystem(this));

        Systems.Add(new CharacterCreationSystem(this));

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
            "cursor_16",
            "symbols_32",
            "robes_32",
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

        //Systems.Add(new PlayerSystem(this));


    }
    private void BuildMenuScene()
    {
        SceneManager.AddScene("Menu");

        var scenePrefab = new MenuScene();
        var ents = scenePrefab.Instantiate(this);
        foreach (var ent in ents)
        {
            SceneManager.AddEntity("Menu", ent);
        }
    }

    private void BuildCharacterSelectScene()
    {
        SceneManager.AddScene("CharacterCreation");

        var scenePrefab = new CharacterCreationScene();
        var ents = scenePrefab.Instantiate(this);
        foreach (var ent in ents)
        {
            SceneManager.AddEntity("CharacterCreation", ent);
        }
    }

    private void BuildPlayScene()
    {
        SceneManager.AddScene("Play");

        var scenePrefab = new PlayScene();
        var ents = scenePrefab.Instantiate(this);
        foreach (var ent in ents)
        {
            SceneManager.AddEntity("Play", ent);
        }
    }


    public override void OnUpdate(GameTime gameTime)
    {

    }

    public override void OnDraw(GameTime gameTime)
    {
        
    }
}