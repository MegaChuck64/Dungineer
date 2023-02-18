using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;


namespace Dungineer;

public static class Settings
{
    public static int TileSize { get; private set; } = 32;
    public static Dictionary<TileType, TileInfo> TileAtlas { get; private set; }

    public static void LoadTileAtlas(ContentManager content)
    {
        var mapVals = ContentLoader.LoadText("MapValues.txt", content);
        TileAtlas = new Dictionary<TileType, TileInfo>();
        foreach (var val in mapVals)
        {
            var splt = val.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var name = splt[0];
            var txtrName = splt[1];
            var x = int.Parse(splt[2]);
            var y = int.Parse(splt[3]);
            var w = int.Parse(splt[4]);
            var h = int.Parse(splt[5]);
            var solid = bool.Parse(splt[6]);
            var txtr = ContentLoader.LoadTexture(txtrName, content);
            TileAtlas.Add(
                Enum.Parse<TileType>(name.Replace(" ", string.Empty)), 
                new TileInfo 
                { 
                    Name = name, 
                    Source = new Rectangle(x, y, w, h), 
                    Texture = txtr, 
                    Solid = solid 
                });
        }
    }
}

public struct TileInfo
{
    public string Name { get; set; }
    public Texture2D Texture { get; set; }
    public Rectangle Source { get; set; }

    public bool Solid { get; set; }
}
public enum TileType
{
    Grass,
    Water,
    PineTree,
    Human,   
}