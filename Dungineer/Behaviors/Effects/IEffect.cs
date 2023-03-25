using Dungineer.Models;

namespace Dungineer.Behaviors.Effects;

public interface IEffect
{
    public EffectType GetEffectType();
    public int TurnsLeft { get; }
}