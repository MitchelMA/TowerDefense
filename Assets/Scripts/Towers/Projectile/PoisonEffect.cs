using Monsters;
using Towers.Poison;

namespace Towers.Projectile
{
    public class PoisonEffect : BaseEffect
    {
        public PoisonEffect(PoisonTower.EffectStats effectStats, BaseTower.TowerType type)
        {
            Value = effectStats.damage;
            Interval = effectStats.interval;
            CurrentTimeout = Interval;
            Duration = effectStats.duration;
            TargetColour = effectStats.effectColour;
            Type = type;
        }
        public override void ApplyEffect(BaseMonster monster)
        {
            monster.GainDamage((int)Value, null);
        }
        
    }
}