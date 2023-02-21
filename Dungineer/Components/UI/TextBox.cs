using Engine;
using Microsoft.Xna.Framework;

namespace Dungineer.Components.UI;

public class TextBox : Component
{
    public string FontName { get; set; }
    public string Text { get; set; }
    public Color TextColor { get; set; }
    public float Layer { get; set; }
}