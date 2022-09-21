using UnityEngine;
using Monsters;
using Towers.Fire;

namespace Towers.Projectile
{
    public class FireEffect : BaseEffect
    {
        public FireEffect(FireTower.EffectStats effectStats, BaseTower.TowerType type)
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
            monster.GainDamage(Value, null);
        }

        public override bool WearOn(BaseMonster monster)
        {
            bool successful = base.WearOn(monster);
            if (!successful)
                return false;
            
            // extra code that could be added when effect successfully wore onto the monster:
            return true;
        }

        public override void WornOff(BaseMonster monster)
        {
            base.WornOff(monster);
            // extra code that could be added when effect wore of the monster:
        }
    }
}