using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Towers
{
    
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
            public int speed;
            public int radius;
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
        
        public Stats baseStats;
        public XpStats baseXpStats;
        
        private TowerType _type = TowerType.Unset;

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
                speed = baseStats.speed,
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
            Debug.Log("Stats initialized");
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
    }
}
