using Engine;
using Microsoft.Xna.Framework;

namespace Dungineer.Components.UI;

public class Image : Component
{
    public Color Tint { get; set; }
    public string TextureName { get; set; }
    public Rectangle Source { get; set; }
    public Point Position { get; set; }
    public Point Size { get; set; }
    public float Layer { get; set; }

    public Rectangle Bounds => new(Position, Size);
}