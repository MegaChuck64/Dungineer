using DiceNotation;
using DiceNotation.Rollers;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using System.Collections.Generic;
using System.IO;

namespace GameCode.Entities;

public static class TileLoader
{
    private static List<TileObject> TileObjects { get; set; }
    public static void Load(ContentManager content, FastRandom rand)
    {
        var dataFolder = Path.Combine(content.RootDirectory, "Data");
        var folders = Directory.EnumerateDirectories(dataFolder);
        foreach (var folder in folders)
        {
            var files = Directory.EnumerateFiles(folder);
            foreach (var file in files)
            {
                var ext = Path.GetExtension(file);
                var lines = File.ReadAllLines(file);

                if (ext == "character")
                    TileObjects.Add(LoadCharacter(lines, rand));
                else if (ext == "tile")
                    TileObjects.Add(LoadGroundTile(lines, rand));
                else if (ext == "weapon")
                    TileObjects.Add(LoadWeapon(lines, rand));

            }
        }
    }

    public static Weapon LoadWeapon(string[] lines, FastRandom rand)
    {
        var wep = new Weapon();

        return wep;
    }

    public static GroundTile LoadGroundTile(string[] lines, FastRandom rand)
    {
        var grd = new GroundTile();

        return grd;
    }
    public static Character LoadCharacter(string[] lines, FastRandom rand)
    {
        var chr = new Character();

        foreach (var line in lines)
        {
            var vl = line[(line.IndexOf(':') + 1)..];

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
        var pck = rand.Next(0, splt.Length);
        return splt[pck];
    }
}