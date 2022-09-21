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
    }
}