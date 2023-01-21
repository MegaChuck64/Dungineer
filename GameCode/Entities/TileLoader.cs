using DiceNotation;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.IO;

namespace GameCode.Entities;

public static class TileLoader
{
    private static List<TileObject> TileObjects { get; set; }
    public static void Load(ContentManager content)
    {
        var dataFolder = Path.Combine(content.RootDirectory, "Data");
        var folders = Directory.EnumerateDirectories(dataFolder);
        foreach (var folder in folders)
        {
            var files = Directory.EnumerateFiles(folder);
            foreach (var file in files)
            {
                if (Path.GetExtension(file) == "character")
                {
                    var chr = new Character();
                    var lines = File.ReadAllLines(file);
                    foreach (var line in lines)
                    {                        
                        var vl = line[(line.IndexOf(':') + 1)..];
                        
                        var splt = line.ToLower().Split(":", System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);

                        var prop = splt[0];

                        switch (prop)
                        {
                            case "name": chr.Name = vl; break;
                            case "description": chr.Description = vl; break;
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
                                if (vl == "Solid") chr.Solid = true;
                                if (vl == "Playable") chr.Playable = true;
                                if (vl == "Undead") chr.Undead = true;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }

    public static int NotationToInt(string not)
    {
        var parser = new DiceParser();
        var dice = parser.Parse(not);
        var res = dice.Roll();
        return res.Value;
    }
}