using Monsters;
using Towers.Ice;

namespace Towers.Projectile
{
    public class IceEffect : BaseEffect
    {
        public IceEffect(IceTower.EffectStats effectStats, BaseTower.TowerType type)
        {
            Value = effectStats.slowness;
            Interval = 0.1f;
            CurrentTimeout = Interval;
            Duration = effectStats.duration;
            TargetColour = effectStats.effectColour;
            Type = type;
        }
        
        public override void ApplyEffect(BaseMonster monster)
        {
            // do absolutely nothing, here
        }

        public override bool WearOn(BaseMonster monster)
        {
            bool successful = base.WearOn(monster);
            if (!successful)
                return false;
            
            // add the slowness effect
            monster.SetSpeed(monster.Speed / Value);
            return true;
        }

        public override void WornOff(BaseMonster monster)
        {
            base.WornOff(monster);
            monster.SetSpeed(monster.Speed * Value);
        }
    }
}