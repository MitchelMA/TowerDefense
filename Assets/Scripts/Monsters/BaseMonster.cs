using System;
using System.Collections.Generic;
using MouseControl;
using PathFinding;
using Towers;
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
        public Stats baseStats;

        public MonsterType Type => type;

        protected PathTraverser PathTraverser;
        protected EnemySelectable Selectable;

        protected readonly List<Effect> _effects = new List<Effect>();
        protected Stats currentStats;
    
        // Start is called before the first frame update
        protected virtual void Start()
        {
            PathTraverser = GetComponent<PathTraverser>();
            Selectable = GetComponent<EnemySelectable>();
            
            SetSpeed(baseStats.speed);
            currentStats.hp = currentStats.hp;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            List<Effect> woreOff = new List<Effect>();
            foreach (Effect effect in _effects)
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
            foreach (Effect wornOff in woreOff)
            {
                wornOff.WearOf(this);
                _effects.Remove(wornOff);
            }
        }

        protected virtual void OnDestroy()
        {
            Selectable.Deselect();
            waveController.DecreaseLeft();
        }

        public bool GiveEffect(Effect effect)
        {
            foreach (Effect lEffect in _effects)
            {
                if (lEffect.Type == effect.Type)
                    return false;
            }
            
            // it was not in the list of effects as to not add multiple of one type
            _effects.Add(effect);
            return true;
        }

        public void GainDamage(int amount)
        {
            currentStats.hp -= amount;
            if(currentStats.hp <= 0)
                Destroy(gameObject);
        }

        public float GetSpeed() => PathTraverser.Speed;

        public void SetSpeed(float value)
        {
            currentStats.speed = value;
            PathTraverser.Speed = value;
        }
    }
}
