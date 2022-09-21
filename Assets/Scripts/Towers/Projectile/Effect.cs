using Monsters;
using UnityEngine;

namespace Towers.Projectile
{
    public class Effect
    {
        private int Damage { get; }

        private float Interval { get; }

        public float Duration { get; private set; }

        private Color TargetColour { get; }

        public BaseTower.TowerType Type { get; }

        public float CurrentTimeout { get; private set; }


        public Effect(int damage, float interval, float duration, Color targetColour, BaseTower.TowerType type)
        {
            Damage = damage;
            Interval = interval;
            CurrentTimeout = interval;
            Duration = duration;
            TargetColour = targetColour;
            Type = type;
        }

        public void ApplyEffect(BaseMonster monster)
        {
            // from is set to null to indicate that this is from an effect
            monster.GainDamage(Damage, null);
        }

        public void DecreaseTimeout(float amount)
        {
            CurrentTimeout -= amount;
            Duration -= amount;
        }

        public void ResetTimeout()
        {
            CurrentTimeout = Interval;
        }

        public void WearOn(BaseMonster monster)
        {
            if (!monster.GiveEffect(this))
                return;
            // get the sprite of the monster
            if (monster.TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                spriteRenderer.color -= Color.white - TargetColour;
            }
        }

        public void WearOf(BaseMonster monster)
        {
            if (monster.TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                spriteRenderer.color += Color.white - TargetColour;
            }
        }
    }
}