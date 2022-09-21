using System;
using System.Collections.Generic;
using Currency;
using MouseControl;
using PathFinding;
using Towers;
using Towers.Projectile;
using UnityEngine;

namespace Monsters
{
    [RequireComponent(typeof(PathTraverser))]
    [RequireComponent(typeof(EnemySelectable))]
    public abstract class BaseMonster : MonoBehaviour
    {
        #region Enums

        public enum MonsterType
        {
            Easy,
            Medium,
            Hard,
        }

        #endregion

        #region Structs

        [Serializable]
        public struct Stats
        {
            public float speed;
            public int hp;
        }

        #endregion

        [SerializeField] protected MonsterType type;
        [SerializeField] protected WaveController waveController;
        [SerializeField] protected CurrencyController currencyController;
        public Stats baseStats;

        public MonsterType Type => type;

        protected PathTraverser PathTraverser;
        protected EnemySelectable Selectable;
        protected CircleCollider2D collider;

        protected readonly List<BaseEffect> Effects = new List<BaseEffect>();
        protected Stats CurrentStats;

        protected bool WasKilled = false;
        protected readonly List<BaseTower> HitBy = new List<BaseTower>();

        private Vector3 _lastpos;
        private Vector3 _curpos;
        public Vector3 UDir => (_curpos - _lastpos).normalized;
        public float Speed => PathTraverser.Speed;
    
        // Start is called before the first frame update
        protected virtual void Start()
        {
            PathTraverser = GetComponent<PathTraverser>();
            Selectable = GetComponent<EnemySelectable>();
            
            SetSpeed(baseStats.speed);
            CurrentStats.hp = baseStats.hp;
            collider = GetComponent<CircleCollider2D>();
            _curpos = transform.position;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            // calculating the moving direction
            _lastpos = _curpos;
            _curpos = transform.position;
            
            List<BaseEffect> woreOff = new List<BaseEffect>();
            foreach (BaseEffect effect in Effects)
            {
                if (effect.CurrentTimeout > 0)
                {
                    effect.DecreaseTimeout(Time.deltaTime);
                    continue;
                }

                if (effect.Duration <= 0)
                {
                    woreOff.Add(effect);
                    continue;
                }
                
                // effect's wait-timeout passed
                effect.ApplyEffect(this);
                effect.ResetTimeout();
            }
            
            // remove the worn off
            foreach (BaseEffect wornOff in woreOff)
            {
                wornOff.WornOff(this);
                Effects.Remove(wornOff);
            }
        }

        protected virtual void OnDestroy()
        {
            Selectable.Deselect();
            waveController.DecreaseLeft();
            // calculate its value only when it was killed
            if (WasKilled)
            {
                int value = (int)(baseStats.hp / 10 + baseStats.speed);
                currencyController.Add(value);
                foreach (BaseTower hit in HitBy)
                    hit.XpUp(value);
            }
        }

        public bool GiveEffect(BaseEffect effect)
        {
            foreach (BaseEffect lEffect in Effects)
            {
                if (lEffect.Type == effect.Type)
                    return false;
            }
            
            // it was not in the list of effects as to not add multiple of one type
            Effects.Add(effect);
            return true;
        }

        public void GainDamage(int amount, BaseTower from)
        {
            CurrentStats.hp -= amount;
            // null is allowed to make the damageGiver anonymous
            if(from is not null)
                AddHitBy(from);
            if (CurrentStats.hp <= 0)
            {
                WasKilled = true;
                Destroy(gameObject);
            }
        }
        
        public void SetSpeed(float value)
        {
            CurrentStats.speed = value;
            PathTraverser.Speed = value;
        }

        private bool AddHitBy(BaseTower tower)
        {
            if (HitBy.Contains(tower))
                return false;
            
            HitBy.Add(tower);
            return true;
        }
    }
}
