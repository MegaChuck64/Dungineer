using Engine;
using System.Collections.Generic;

namespace Dungineer.Components.UI;

public class Terminal : Component
{
    public List<string> Lines { get; set; } = new List<string>();
    public int SelectedLine { get; set; } = 0;
    public string CurrentLine { get; set; } = string.Empty;

    public bool Collapsed { get; set; } = false;
}