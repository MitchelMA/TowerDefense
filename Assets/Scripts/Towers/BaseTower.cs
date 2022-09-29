using System;
using System.Collections.Generic;
using System.Reflection;
using Currency;
using Monsters;
using MouseControl;
using Towers.Projectile;
using UI;
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

        public struct StatsDiff
        {
            public Stats Previous;
            public Stats Current;
        }

        [Serializable]
        public struct XpStats
        {
            public int currentXp;
            public int neededXp;
            public int xpIncreaseOnLevelUp;
            public int lvlPoints;
            public int currentLvl;
            public int maxLevel;
        }

        [Serializable]
        public struct StatsUp
        {
            public int damage;
            public float fireRate;
            public float projectileSpeed;
            public float radius;
        }

        #endregion
        
        [SerializeField] protected Projectile.Projectile projectilePrefab;
        [SerializeField] protected Transform projectileParent;
        [SerializeField] protected string enemyTag;
        [SerializeField] protected int integralSteps = 10;
        [SerializeField] protected TowerStatsUI statsUI;
        
        public Stats baseStats;
        public XpStats baseXpStats;
        [SerializeField] protected StatsUp statsIncrease;

        [NonSerialized] public GameObject RadiusIndicator;
        
        private TowerType _type = TowerType.Unset;
        protected CircleCollider2D Collider;
        
        protected TowerNode parentNode;
        protected CurrencyController _currencyController;

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
        protected const float ProjectileLifeTime = 5;

        #endregion

        public event EventHandler<StatsDiff> BaseStatsChanged; 

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
                maxLevel = baseXpStats.maxLevel,
                currentXp = baseXpStats.currentXp,
                lvlPoints = baseXpStats.lvlPoints,
                neededXp = baseXpStats.neededXp,
                xpIncreaseOnLevelUp = baseXpStats.xpIncreaseOnLevelUp,
            };

            Collider = GetComponent<CircleCollider2D>();
            SetRadius(baseStats.radius);
            currentShootTimeout = 0;
            parentNode = transform.parent.GetComponent<TowerNode>();
            parentNode.Selectable.OnStatusChanged += OnParentSelect;
            BaseStatsChanged += StatsHaveChanged;
            _currencyController = GameObject.FindWithTag("CurrencyController").GetComponent<CurrencyController>();
        }

        protected void OnParentSelect(object sender, bool selectedStatus)
        {
            if (!selectedStatus) return;
            
            UpdateStatsDisplay();
            UpdateStatsBtns();
        }

        protected void StatsHaveChanged(object sender, StatsDiff diff)
        {
            if(parentNode.Selectable.IsSelected)
                UpdateStatsDisplay();
        }

        protected virtual void OnDestroy()
        {
            parentNode.Selectable.OnStatusChanged -= OnParentSelect;
            BaseStatsChanged -= StatsHaveChanged;
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
        protected virtual void AfterLevelUp(int successiveCount)
        {
            IncreaseBaseStats(statsIncrease.damage, statsIncrease.radius, statsIncrease.fireRate, statsIncrease.projectileSpeed);
            
            // check for the buttons
            if (parentNode.Selectable.IsSelected)
            {
                UpdateStatsBtns();
            }
            
            // show indication of leveling up
            parentNode.Indicating = true;
        }
        
        /// <summary>
        /// Method that automatically gets called after gaining xp
        /// </summary>
        /// <param name="levelsGained">The amount of levels the tower gained</param>
        /// <param name="xpGained">The amount of xp the tower gained</param>
        protected virtual void AfterGainingXp(int levelsGained, int xpGained) { }

        protected virtual bool CreateEffect(out BaseEffect effect)
        {
            effect = default;
            return false;
        }

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
            Destroy(clone.gameObject, ProjectileLifeTime);

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
            if (CurrentXpStats.currentLvl >= CurrentXpStats.maxLevel)
                return 0;
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

        public virtual void UpdateStatsDisplay()
        {
            // set the type
            statsUI.StandAlones.typeText.text = $"Type: {Enum.GetName(typeof(BaseTower.TowerType), Type)}";
            // set the lvl-points
            statsUI.StandAlones.pointText.text = $"Points: {CurrentXpStats.lvlPoints}";
            // set the xp
            statsUI.StandAlones.xpCounter.text = $"Xp:\n{CurrentXpStats.currentXp} / {CurrentXpStats.neededXp}";
            // set the level
            statsUI.StandAlones.levelDisplay.text = $"Level: {CurrentXpStats.currentLvl}";
            // close btn
            statsUI.StandAlones.closeBtn.onClick.RemoveAllListeners();
            statsUI.StandAlones.closeBtn.onClick.AddListener(parentNode.Selectable.Deselect);
            // destroy btn
            statsUI.StandAlones.destroyBtn.onClick.RemoveAllListeners();
            statsUI.StandAlones.destroyBtn.onClick.AddListener(delegate
            {
                TowerType type = parentNode.RemoveTower();
                if (type == TowerType.Unset)
                    return;

                ulong returnPrice = (ulong)(_currencyController.GetPriceOfType(type) * 0.5f);
                _currencyController.Add(returnPrice);
            });

            // update the base-stats
            statsUI.StatsTexts.damage.text = $"Damage: {CurrentStats.damage}";
            statsUI.StatsTexts.fireRate.text = $"Fire-Rate: {CurrentStats.fireRate:0.00}p/s";
            statsUI.StatsTexts.projectileSpeed.text = $"Bullet-Speed: {CurrentStats.projectileSpeed:0.00}m/s";
            statsUI.StatsTexts.radius.text = $"Radius: {CurrentStats.radius}m";

            if (RadiusIndicator)
            {
                RadiusIndicator.transform.localScale = new Vector2(CurrentStats.radius*2, CurrentStats.radius*2);
            }
        }

        public virtual void UpdateStatsBtns()
        {
            int points = CurrentXpStats.lvlPoints;
            statsUI.StatsBtns.damage.interactable = points > 0;
            statsUI.StatsBtns.radius.interactable = points > 0;
            statsUI.StatsBtns.fireRate.interactable = points > 0;
            statsUI.StatsBtns.projectileSpeed.interactable = points > 0;
            
            statsUI.EffectStatsBtns.duration.interactable = points > 0;
            statsUI.EffectStatsBtns.interval.gameObject.SetActive(true);
            statsUI.EffectStatsBtns.interval.interactable = points > 0;
            statsUI.EffectStatsBtns.value.interactable = points > 0;
            
            statsUI.StatsBtns.damage.onClick.RemoveAllListeners();
            statsUI.StatsBtns.damage.onClick.AddListener(delegate
            {
                IncreaseBaseStats(statsIncrease.damage, 0, 0, 0);
                CurrentXpStats.lvlPoints--;
                parentNode.Indicating = false;
                UpdateStatsBtns();
                UpdateStatsDisplay();
            });
            
            statsUI.StatsBtns.radius.onClick.RemoveAllListeners();
            statsUI.StatsBtns.radius.onClick.AddListener(delegate
            {
                IncreaseBaseStats(0, statsIncrease.radius, 0, 0);
                CurrentXpStats.lvlPoints--;
                UpdateStatsBtns();
                UpdateStatsDisplay();
            });
            
            statsUI.StatsBtns.fireRate.onClick.RemoveAllListeners();
            statsUI.StatsBtns.fireRate.onClick.AddListener(delegate
            {
                IncreaseBaseStats(0, 0, statsIncrease.fireRate, 0);
                CurrentXpStats.lvlPoints--;
                parentNode.Indicating = false;
                UpdateStatsBtns();
                UpdateStatsDisplay();
            });
            
            statsUI.StatsBtns.projectileSpeed.onClick.RemoveAllListeners();
            statsUI.StatsBtns.projectileSpeed.onClick.AddListener(delegate
            {
                IncreaseBaseStats(0, 0, 0, statsIncrease.projectileSpeed);
                CurrentXpStats.lvlPoints--;
                parentNode.Indicating = false;
                UpdateStatsBtns();
                UpdateStatsDisplay();
            });
        }

        public virtual void NonInteractebleBtns()
        {
            statsUI.StatsBtns.damage.interactable = false;
            statsUI.StatsBtns.radius.interactable = false;
            statsUI.StatsBtns.fireRate.interactable = false;
            statsUI.StatsBtns.projectileSpeed.interactable = false;
            
            statsUI.EffectStatsBtns.duration.interactable = false;
            statsUI.EffectStatsBtns.interval.interactable = false;
            statsUI.EffectStatsBtns.value.interactable = false;
        }

        protected void IncreaseBaseStats(int damage, float radius, float fireRate, float projectileSpeed)
        {
            Stats old = new Stats
            {
                damage = CurrentStats.damage,
                fireRate = CurrentStats.fireRate,
                projectileSpeed = CurrentStats.projectileSpeed,
                radius = CurrentStats.radius,
            };
            CurrentStats.damage += damage;
            CurrentStats.fireRate += fireRate;
            CurrentStats.projectileSpeed += projectileSpeed;
            SetRadius(CurrentStats.radius + radius);
            Stats newS = new Stats
            {
                damage = CurrentStats.damage,
                fireRate = CurrentStats.fireRate,
                projectileSpeed = CurrentStats.projectileSpeed,
                radius = CurrentStats.radius,
            };
            StatsDiff diff = new StatsDiff
            {
                Previous = old,
                Current = newS,
            };
            BaseStatsChanged?.Invoke(this, diff);
        }
    }
}
