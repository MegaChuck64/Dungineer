using Dungineer.Components.GameWorld;
using Dungineer.Models;
using Engine;


namespace Dungineer.Behaviors.Effects;

public class FireEffect : IEffect, IBehavior
{
    public int TurnsLeft { get; private set; }

    public EffectType GetEffectType() => EffectType.Fire;

    public FireEffect()
    {
        TurnsLeft = Settings.EffectAtlas[GetEffectType()].Turns;
    }

    public bool TryPerform(Entity performer, Entity inflicted)
    {
        if (TurnsLeft-- > 0)
        {
            if (inflicted.GetComponent<CreatureStats>() is CreatureStats inflicedStats)
            {
                var effectInfo = Settings.EffectAtlas[GetEffectType()];
                inflicedStats.Health -= effectInfo.Damage;
                if (inflicedStats.Health < 0)
                {
                    inflicedStats.Health = 0;
                }

                return true;
            }
        }

        return false;
    }
}