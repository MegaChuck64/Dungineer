using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dungineer;

public static class Settings
{
    //tilesize works best for 27 - 36
    //designed for 32
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

public struct TileInfo
{
    public string Name { get; set; }
    public string TextureName { get; set; }

    [JsonConverter(typeof(ContentLoader.RectangleJsonConverter))]
    public Rectangle Source { get; set; }

    public bool Solid { get; set; }
}
public enum TileType
{
    Grass,
    Water,
    PineTree,
    DungeonWall,
    DungeonFloor,
    DungeonFloorHole,
}

public struct MapObjectInfo
{
    public string Name { get; set; }

    public string TextureName { get; set; }

    [JsonConverter(typeof(ContentLoader.RectangleJsonConverter))]
    public Rectangle Source { get; set; }
    public string Description { get; set; }

    public int LotteryValue { get; set; }

}

public enum MapObjectType
{
    Human,
    Ghost,
    Arcanium
}

public struct WardrobeInfo
{
    public string Name { get; set; }
    public string TextureName { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public WardrobeSlot Slot { get; set; }

    public string Description { get; set; }
    public int Cost { get; set; }
    public float HealthMod { get; set; }
    public float StaminaMod { get; set; }
    public float MoveSpeedMod { get; set; }


    [JsonConverter(typeof(ContentLoader.RectangleJsonConverter))]
    public Rectangle Source { get; set; }
}

public enum WardrobeSlot
{
    Hair,
    Head,
    Beard,
    Body,
    Arm,
    Leg,
    Shoe,
}

public enum WardrobeType
{
    BasicRobe,
    ProperRobe,
    GrimRobe,
    MasterRobe
}


//public class WardrobeSlotJsonConverter : JsonConverter<WardrobeSlot>
//{
//    public override WardrobeSlot Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//    {
//        var slotPropertyName = System.Text.Encoding.UTF8.GetBytes("Slot");

//        var slot = WardrobeSlot.Hair;

//        while (reader.Read())
//        {
//            switch (reader.TokenType)
//            {
//                case JsonTokenType.PropertyName:

//                    if (reader.ValueTextEquals(slotPropertyName))
//                    {
//                        reader.Read();
//                        var str = reader.GetString();
//                        slot = Enum.Parse<WardrobeSlot>(str);
//                        return slot;
//                    }
//                    break;
//                default:
//                    break;
//            }

//        }

//        return slot;
//    }

//    public override void Write(Utf8JsonWriter writer, WardrobeSlot value, JsonSerializerOptions options)
//    {
//        var strVal = value.ToString();
//        writer.WriteStringValue(strVal);
//    }
//}