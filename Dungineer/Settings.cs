using Engine;
using Dungineer.Models;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Dungineer;

public static class Settings
{
    public static int TileSize { get; private set; } = 32;
    public static int Seed { get; private set; } = 1199172130;//new Random().Next();
    public static Dictionary<TileType, TileInfo> TileAtlas { get; private set; }
    public static Dictionary<MapObjectType, MapObjectInfo> MapObjectAtlas { get; private set; }
    public static Dictionary<WardrobeType, WardrobeInfo> WardrobeAtlas { get; private set; }
    public static Dictionary<string, Texture2D> TextureAtlas { get; private set; }


    public static void SetSeed(int? seed = null)
    {
        if (seed != null)
        {
            Seed = seed.Value;
        }
    }
    public static void LoadMapObjectAtlas(ContentManager content)
    {
        MapObjectAtlas = new Dictionary<MapObjectType, MapObjectInfo>();
        TextureAtlas ??= new Dictionary<string, Texture2D>();

        var mapObjectInfo = ContentLoader.LoadObjectFromJson<List<MapObjectInfo>>("MapObjectInfo.json", content);
        foreach (var minfo in mapObjectInfo)
        {
            var keyName = minfo.Name.Replace(" ", string.Empty);
            var infoType = Enum.Parse<MapObjectType>(keyName);
            if (!TextureAtlas.ContainsKey(minfo.TextureName))
                TextureAtlas.Add(minfo.TextureName, ContentLoader.LoadTexture(minfo.TextureName, content));

            MapObjectAtlas.Add(infoType, minfo);
        }
    }

    public static void LoadTileAtlas(ContentManager content)
    {
        TileAtlas = new Dictionary<TileType, TileInfo>();
        TextureAtlas ??= new Dictionary<string, Texture2D>();

        var tileInfo = ContentLoader.LoadObjectFromJson<List<TileInfo>>("TileInfo.json", content);
        foreach (var tinfo in tileInfo)
        {
            var keyName = tinfo.Name.Replace(" ", string.Empty);
            var infoType = Enum.Parse<TileType>(keyName);
            if (!TextureAtlas.ContainsKey(tinfo.TextureName))
                TextureAtlas.Add(tinfo.TextureName, ContentLoader.LoadTexture(tinfo.TextureName, content));

            TileAtlas.Add(infoType, tinfo);
        }       
    }

    public static void LoadWardrobeAtlas(ContentManager content)
    {
        WardrobeAtlas = new Dictionary<WardrobeType, WardrobeInfo>();
        TextureAtlas ??= new Dictionary<string, Texture2D>();

        var wardrobeInfo = ContentLoader.LoadObjectFromJson<List<WardrobeInfo>>("WardrobeInfo.json", content);
        foreach (var winfo in wardrobeInfo)
        {
            var keyname = winfo.Name.Replace(" ", string.Empty);
            var infoType = Enum.Parse<WardrobeType>(keyname);
            if (!TextureAtlas.ContainsKey(winfo.TextureName))
                TextureAtlas.Add(winfo.TextureName, ContentLoader.LoadTexture(winfo.TextureName, content));

            WardrobeAtlas.Add(infoType, winfo);
        }
    }

}






