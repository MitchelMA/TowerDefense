using Monsters;
using UnityEngine;

namespace Towers.Projectile
{
    public abstract class BaseEffect
    {
        public int Value { get; protected set; }

        public float Interval { get; protected set; }

        public float Duration { get; protected set; }

        public Color TargetColour { get; protected set; }

        public BaseTower.TowerType Type { get; protected set; }

        public float CurrentTimeout { get; protected set; }

        public abstract void ApplyEffect(BaseMonster monster);
        // {
        //     // from is set to null to indicate that this is from an effect
        //     monster.GainDamage(Damage, null);
        // }

        public void DecreaseTimeout(float amount)
        {
            CurrentTimeout -= amount;
            Duration -= amount;
        }

        public void ResetTimeout()
        {
            CurrentTimeout = Interval;
        }

        public virtual void WearOn(BaseMonster monster)
        {
            if (!monster.GiveEffect(this))
                return;
            // get the sprite of the monster
            if (monster.TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                spriteRenderer.color -= Color.white - TargetColour;
            }
        }

        public virtual void WearOf(BaseMonster monster)
        {
            if (monster.TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                spriteRenderer.color += Color.white - TargetColour;
            }
        }
    }
}