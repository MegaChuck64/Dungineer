using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine;

public static class SceneManager
{
    private static Dictionary<string, List<Entity>> scenes = new Dictionary<string, List<Entity>>();

    public static string CurrentScene { get; private set; } = string.Empty;

    public static IEnumerable<Entity> Entities => scenes.ContainsKey(CurrentScene) ? scenes[CurrentScene] : Array.Empty<Entity>();

    public static void ChangeScene(string sceneName) => CurrentScene = sceneName; 
    
    public static void AddScene(string sceneName) => scenes.Add(sceneName, new List<Entity>());
    public static void AddEntity(string sceneName, Entity entity) => scenes[sceneName].Add(entity);

}

