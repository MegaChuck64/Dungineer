using Dungineer.Prefabs.Scenes;
using Dungineer.Systems;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace Dungineer;

public class MainGame : BaseGame
{
    //1440x768
    public MainGame() : base(Settings.Seed, Settings.TileSize * 45, Settings.TileSize * 24)
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
        Settings.Load(this, content);

        AddSystems();

        BuildScenes();

        SceneManager.ChangeScene("Menu");
    }

    private void AddSystems()
    {
        var mapSystem = new MapSystem(this, Content);
        Systems.Add(mapSystem);
        
        var sightSystem = new SightSystem(this);
        Systems.Add(sightSystem);

        Systems.Add(new MapDrawSystem(this, Content, mapSystem, sightSystem));

        Systems.Add(new UISystem(this));
    }

    private void BuildScenes()
    {
        BuildScene("Menu", new MenuScene());
        BuildScene("CharacterCreation", new CharacterCreationScene());
        BuildScene("Play", new PlayScene());
    }
    private void BuildScene(string sceneName, IPrefab<List<Entity>> scene)
    {
        SceneManager.AddScene(sceneName);

        var ents = scene.Instantiate(this);
        foreach (var ent in ents)
        {
            SceneManager.AddEntity(sceneName, ent);
        }
    }

    public override void OnUpdate(GameTime gameTime)
    {

    }

    public override void OnDraw(GameTime gameTime)
    {

    }
}