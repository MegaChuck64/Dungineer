using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungineer.Components;

public class Map : Component
{
    public byte[,] Tiles { get; set; }
    public Map(Entity owner, bool isActive = true) : base(owner, isActive)
    {

    }
}