using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Engine;

public static class ContentLoader
{

    public static Texture2D TextureFromSpriteAtlas(string atlasName, Rectangle source, ContentManager content, GraphicsDevice graphicsDevice)
    {
        var atlas = LoadTexture(atlasName, content);

        var tex = new Texture2D(graphicsDevice, source.Width, source.Height);

        var data = new Color[source.Width * source.Height];

        atlas.GetData(0, source, data, 0, data.Length);

        tex.SetData(data);

        return tex;
    }

    public static Texture2D LoadTexture(string name, ContentManager content) =>
        content.Load<Texture2D>(System.IO.Path.Combine("Tiles", name));

    public static SpriteFont LoadFont(string name, ContentManager content) =>
        content.Load<SpriteFont>(System.IO.Path.Combine("Fonts", name));

    public static string[] LoadTextLines(string name, ContentManager content) =>
        System.IO.File.ReadAllLines(System.IO.Path.Combine(content.RootDirectory, "Data", name));

    public static T LoadObjectFromJson<T>(string name, ContentManager content) =>
        JsonSerializer.Deserialize<T>(System.IO.File.ReadAllText(System.IO.Path.Combine(content.RootDirectory, "Data", name)));

    public class RectangleJsonConverter : JsonConverter<Rectangle>
    {
        public override Rectangle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var xPropertyName = Encoding.UTF8.GetBytes("X");
            var yPropertyName = Encoding.UTF8.GetBytes("Y");
            var wPropertyName = Encoding.UTF8.GetBytes("Width");
            var hPropertyName = Encoding.UTF8.GetBytes("Height");

            var rect = new Rectangle();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                var tokenType = reader.TokenType;
                switch (tokenType)
                {
                    case JsonTokenType.PropertyName:
                        if (reader.ValueTextEquals(xPropertyName))
                        {
                            reader.Read();
                            rect.X = reader.GetInt32();
                        }
                        else if (reader.ValueTextEquals(yPropertyName))
                        {
                            reader.Read();
                            rect.Y = reader.GetInt32();
                        }
                        else if (reader.ValueTextEquals(wPropertyName))
                        {
                            reader.Read();
                            rect.Width = reader.GetInt32();
                        }
                        else if (reader.ValueTextEquals(hPropertyName))
                        {
                            reader.Read();
                            rect.Height = reader.GetInt32();
                        }



                        break;
                    default:
                        break;
                }

            }

            return rect;
        }

        public override void Write(Utf8JsonWriter writer, Rectangle value, JsonSerializerOptions options)
        {
            var strVal = value.ToString();
            writer.WriteStringValue(strVal);
        }
    }



}