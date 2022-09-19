using Monsters;
using UnityEngine;

namespace Towers
{
    public class Effect
    {
        private int Damage { get; }

        private float Interval { get; }

        public float Duration { get; private set; }

        private Color ColorIncrease { get; }

        public BaseTower.TowerType Type { get; }

        public float CurrentTimeout { get; private set; }


        public Effect(int damage, float interval, float duration, Color colorIncrease, BaseTower.TowerType type)
        {
            Damage = damage;
            Interval = interval;
            CurrentTimeout = interval;
            Duration = duration;
            ColorIncrease = colorIncrease;
            Type = type;
        }

        public void ApplyEffect(BaseMonster monster)
        {
            monster.GainDamage(Damage);
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
                spriteRenderer.color += ColorIncrease;
            }
        }

        public void WearOf(BaseMonster monster)
        {
            if (monster.TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                spriteRenderer.color -= ColorIncrease;
            }
        }
    }
}