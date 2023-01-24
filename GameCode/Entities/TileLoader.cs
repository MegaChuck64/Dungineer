using DiceNotation;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GameCode.Entities;

public static class TileLoader
{
    public static List<TileObject> TileObjects { get; set; }
    public static void Load(ContentManager content, FastRandom rand)
    {
        TileObjects = new List<TileObject>();
        var dataFolder = Path.Combine(content.RootDirectory, "Data");
        var folders = Directory.EnumerateDirectories(dataFolder);
        foreach (var folder in folders)
        {
            var files = Directory.EnumerateFiles(folder);
            foreach (var file in files)
            {
                var ext = Path.GetExtension(file);
                var lines = File.ReadAllLines(file)
                    .Where(y=>!string.IsNullOrWhiteSpace(y))
                    .ToArray();

                if (ext == ".character")
                    TileObjects.Add(LoadCharacter(lines, rand, content));
                else if (ext == ".tile")
                    TileObjects.Add(LoadGroundTile(lines, rand, content));
                else if (ext == ".weapon")
                    TileObjects.Add(LoadWeapon(lines, rand, content));

            }
        }
    }

    public static Weapon LoadWeapon(string[] lines, FastRandom rand, ContentManager content)
    {
        var wep = new Weapon();
        var spr = string.Empty;

        foreach (var line in lines)
        {
            var vl = line[(line.IndexOf(':') + 1)..].Trim();
            var splt = line.ToLower().Split(":", System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);
            var prop = splt[0];

            switch (prop)
            {
                case "name": wep.Name = PickRandomOption(vl, rand); break;
                case "description": wep.Description = PickRandomOption(vl, rand); break;
                case "damage": wep.Damage = PickRandomOption(vl, rand);break;
                case "rarity": wep.Rarity = PickRandomOption(vl, rand); break;
                case "weight": wep.Weight = int.Parse(vl); break;
                case "range": wep.Range = vl; break;
                case "flag":
                    wep.Flags ??= new List<string>();
                    wep.Flags.Add(vl);
                    break;
                case "requires":
                    wep.Requirements ??= new List<string>();
                    wep.Requirements.Add(vl);
                    break;
                case "sprite": spr = vl; break;
                case "frame":
                    if (!string.IsNullOrEmpty(spr))
                    {
                        wep.Sprite =
                            LoadTexture(
                                spr,
                                ParseFrame(PickRandomOption(vl, rand)),
                                content);
                    }
                    break;

            }
        }


        return wep;
    }

    public static GroundTile LoadGroundTile(string[] lines, FastRandom rand, ContentManager content)
    {
        var grd = new GroundTile();
        var spr = string.Empty;
        foreach (var line in lines)
        {
            var vl = line[(line.IndexOf(':') + 1)..].Trim();

            var splt = line.ToLower().Split(":", System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);
            var prop = splt[0];

            switch (prop)
            {
                case "name": grd.Name = PickRandomOption(vl, rand); break;
                case "speedmod": grd.SpeedMod = float.Parse(PickRandomOption(vl, rand)); break;
                case "sprite": spr = vl; break;
                case "frame":
                    if (!string.IsNullOrEmpty(spr))
                    {
                        grd.Sprite =
                            LoadTexture(
                                spr,
                                ParseFrame(PickRandomOption(vl, rand)),
                                content);
                    }
                    break;
                case "flag":
                    grd.Flags ??= new List<string>();
                    grd.Flags.Add(vl);
                    break;
                default:
                    break;
            }
        }

        return grd;
    }
    public static Character LoadCharacter(string[] lines, FastRandom rand, ContentManager content)
    {
        var chr = new Character();
        var spr = string.Empty;
        foreach (var line in lines)
        {
            var vl = line[(line.IndexOf(':') + 1)..].Trim();

            var splt = line.ToLower().Split(":", System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);

            var prop = splt[0];

            switch (prop)
            {
                case "name": chr.Name = PickRandomOption(vl, rand); break;
                case "description": chr.Description = PickRandomOption(vl, rand); break;
                case "race": chr.Race = vl; break;
                case "class": chr.Class = vl; break;
                case "health":
                    chr.MaxHealth = NotationToInt(vl);
                    chr.Health = chr.MaxHealth;
                    break;
                case "healthregen": chr.HealthRegen = NotationToInt(vl); break;
                case "stamina":
                    chr.MaxStamina = NotationToInt(vl);
                    chr.Stamina = chr.MaxStamina;
                    break;
                case "staminaregen": chr.StaminaRegen = NotationToInt(vl); break;
                case "strength": chr.Strength = NotationToInt(vl); break;
                case "speed": chr.Speed = NotationToInt(vl); break;
                case "armor": chr.Armor = NotationToInt(vl); break;
                case "size": chr.Size = vl; break;
                case "flag":
                    chr.Flags ??= new List<string>();
                    chr.Flags.Add(vl);
                    break;
                case "weapon":
                    chr.Weapon = PickRandomOption(vl, rand); break;
                case "sprite": spr = vl; break;
                case "frame": 
                    if (!string.IsNullOrEmpty(spr))
                    {
                        chr.Sprite = 
                            LoadTexture(
                                spr, 
                                ParseFrame(PickRandomOption(vl, rand)),
                                content);
                    }
                    break;
                default:
                    break;
            }
        }

        return chr;
    }


    public static int NotationToInt(string not)
    {
        var parser = new DiceParser();
        var dice = parser.Parse(not);
        var res = dice.Roll();
        return res.Value;
    }

    public static string PickRandomOption(string val, FastRandom rand)
    {
        var splt = val.Split(';', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);
        var pck = rand.Next(0, splt.Length - 1);
        return splt[pck];
    }

    public static Texture2D LoadTexture(string sprite, (int x, int y) frame, ContentManager content)
    {
        return Sprite.TextureFromSpriteAtlas(sprite, new Rectangle(frame.x * 32, frame.y * 32, 32, 32), content);
    }

    public static (int x, int y) ParseFrame(string frm)
    {
        if (!string.IsNullOrEmpty(frm))
        {
            var spl = frm.Split(',', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);
            return (int.Parse(spl[0]), int.Parse(spl[1]));
        }
        else
            return (0, 0);
    }
}