using Dungineer.Models;

namespace Dungineer.Interfaces;

public interface IEffect
{
    public ResultData PerformPassive();
    public ResultData PerformActive();
}