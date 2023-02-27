using Dungineer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungineer.Models;

public class GameItem
{
    public string Name { get; set; }
    public List<IEffect> Effects { get; set; }
}