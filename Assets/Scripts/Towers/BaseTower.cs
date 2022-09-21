using System;
using System.Collections.Generic;
using System.Reflection;
using Monsters;
using Towers.Projectile;
using UnityEngine;

namespace Towers
{
    [RequireComponent(typeof(CircleCollider2D))]
    public abstract class BaseTower : MonoBehaviour
    {
        #region Enums

        [Serializable]
        public enum TowerType
        {
            Unset,
            Fire,
            Poison,
            Ice,
        }

        #endregion
        
        #region Structs
        
        [Serializable]
        public struct Stats
        {
            public int damage;
            public float fireRate;
            public float projectileSpeed;
            public float radius;
        }

        [Serializable]
        public struct XpStats
        {
            public int currentXp;
            public int neededXp;
            public int xpIncreaseOnLevelUp;
            public int lvlPoints;
            public int currentLvl;
        }

        #endregion
        
        [SerializeField] protected Projectile.Projectile projectilePrefab;
        [SerializeField] protected Transform projectileParent;
        [SerializeField] protected string enemyTag;
        [SerializeField] protected int integralSteps = 10;
        public Stats baseStats;
        public XpStats baseXpStats;
        
        private TowerType _type = TowerType.Unset;
        protected CircleCollider2D Collider;

        protected Type effectType;

        #region Public Properties
        
        public TowerType Type
        {
            get => _type;
            set
            {
                if (_type != TowerType.Unset)
                    return;
                _type = value;
            }
        }
        
        #endregion

        #region Currents

        protected Stats CurrentStats;
        protected XpStats CurrentXpStats;
        protected float currentShootTimeout;

        #endregion

        /// <summary>
        /// The Start method of the BaseTower should always be called with
        /// `base.Start()` when overriding. This is because the Start method
        /// of this base class initializes protected fields
        /// </summary>
        protected virtual void Start()
        {
            CurrentStats = new Stats
            {
                damage = baseStats.damage,
                fireRate = baseStats.fireRate,
                projectileSpeed = baseStats.projectileSpeed,
                radius = baseStats.radius,
            };

            CurrentXpStats = new XpStats
            {
                currentLvl = baseXpStats.currentLvl,
                currentXp = baseXpStats.currentXp,
                lvlPoints = baseXpStats.lvlPoints,
                neededXp = baseXpStats.neededXp,
                xpIncreaseOnLevelUp = baseXpStats.xpIncreaseOnLevelUp,
            };

            Collider = GetComponent<CircleCollider2D>();
            SetRadius(baseStats.radius);
            currentShootTimeout = 0;
        }

        protected virtual void Update()
        {
            if (currentShootTimeout >= 0)
                currentShootTimeout -= Time.deltaTime;
        }

        protected void OnTriggerStay2D(Collider2D other)
        {
            if (!other.tag.Equals(enemyTag))
                return;

            // Code that has to do with firing at the enemy:
            if (!other.TryGetComponent(out BaseMonster monster))
                return;
            
            ShootAt(monster);
        }

        /// <summary>
        /// Method that automatically gets called after the tower gained a level
        /// </summary>
        /// <param name="successiveCount">The amount of times this function was called in a row thusfar</param>
        protected virtual void AfterLevelUp(int successiveCount) { }
        
        /// <summary>
        /// Method that automatically gets called after gaining xp
        /// </summary>
        /// <param name="levelsGained">The amount of levels the tower gained</param>
        /// <param name="xpGained">The amount of xp the tower gained</param>
        protected virtual void AfterGainingXp(int levelsGained, int xpGained) { }

        protected virtual bool ShootAt(BaseMonster monster)
        {
            if (currentShootTimeout >= 0)
                return false;

            // calculating the aim-direction
            Vector3 monDir = monster.UDir;
            Vector3 monPos = monster.transform.position;
            float monSpeed = monster.Speed;
            float dist = (monPos - transform.position).magnitude;
            float dur = dist / CurrentStats.projectileSpeed;

            Vector3 max = monPos + monDir * monSpeed * dur;
            float timeDiff = CalcTimeDiff(monster, max);
            // almost impossible to hit monster
            // if (timeDiff > 0.17)
            //     return false;

            Vector3 min = monPos + monDir * monSpeed * (dur + timeDiff);
            Vector3 middle = Vector3.Lerp(min, max, 0.5f);
            for (int i = 0; i < integralSteps; i++)
            {
                float diff = Math.Abs(CalcTimeDiff(monster, middle) - timeDiff);
                if (diff < 0.0001)
                    break;

                timeDiff = CalcTimeDiff(monster, middle);
                // overshot
                if (timeDiff < 0)
                {
                    max = middle;
                    middle = Vector3.Lerp(min, max, 0.5f);
                }
                // undershot
                else
                {
                    min = middle;
                    middle = Vector3.Lerp(min, max, 0.5f);
                }
            }

            
            Vector3 predictionDir = (middle - transform.position).normalized;
            
            // setup of the projectile with the Effect and other data:
            bool hadEffect = CreateEffect(out BaseEffect effect);
            
            var clone = Instantiate(projectilePrefab, projectileParent);
            clone.transform.position = transform.position;
            clone.Setup(predictionDir, CurrentStats.projectileSpeed, CurrentStats.damage, effect, this);

            // reset the timeout
            currentShootTimeout = 1 / CurrentStats.fireRate;
            return true;

            float CalcTimeDiff(BaseMonster mon, Vector3 bestPrediction) =>
                Vector3.Distance(transform.position, bestPrediction) / CurrentStats.projectileSpeed -
                Vector3.Distance(mon.transform.position, bestPrediction) / mon.Speed;
        }


        /// <summary>
        /// Adds the given XP amount to the current xp.
        /// Also checks if the tower can level-up
        /// </summary>
        /// <param name="xp"></param>
        /// <returns>The amount of levels the tower went up after gaining the xp</returns>
        public int XpUp(int xp)
        {
            CurrentXpStats.currentXp += xp;
            int gainedLevels = CheckLvlUp();
            
            AfterGainingXp(gainedLevels, xp);
            return gainedLevels;
        }

        public void XpUpVoid(int xp)
        {
            XpUp(xp);
        }

        /// <summary>
        /// Checks if the Tower can level-up
        /// </summary>
        /// <returns>The amount of levels the tower gained</returns>
        private int CheckLvlUp()
        {
            int lvlups = 0;
            while (CurrentXpStats.currentXp >= CurrentXpStats.neededXp)
            {
                lvlups++;
                CurrentXpStats.currentXp -= CurrentXpStats.neededXp;
                CurrentXpStats.neededXp += CurrentXpStats.xpIncreaseOnLevelUp;
                
                // update the current levels
                CurrentXpStats.currentLvl++;
                CurrentXpStats.lvlPoints++;
                // call the lvlup callback
                AfterLevelUp(lvlups);
            }
            return lvlups;
        }

        protected void ResetTowerType()
        {
            _type = TowerType.Unset;
        }

        protected void SetRadius(float newRadius)
        {
            CurrentStats.radius = newRadius;
            Collider.radius = newRadius;
        }

        protected abstract bool CreateEffect(out BaseEffect effect);
    }
}
