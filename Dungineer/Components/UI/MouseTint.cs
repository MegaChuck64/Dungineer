using Engine;
using Microsoft.Xna.Framework;


namespace Dungineer.Components.UI;

public class MouseTint : Component
{
    public Color DefaultColor { get; set; }
    public Color HoverColor { get; set; }
    public Color PressedColor { get; set; }
    public Color SelectedColor { get; set; }
    public string SelectionGroup { get; set; }
    public bool Selected { get; set; }

    public MouseTint(bool isActive = true) : base (isActive)
    {
        DefaultColor = Color.White;
        HoverColor = Color.White;
        PressedColor = Color.White;
        SelectedColor = Color.White;
        SelectionGroup = string.Empty;
        Selected = false;
    }

}