using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Towers
{
    [Serializable]
    public enum TowerType
    {
        Unset,
        Fire,
        Grass,
        Water,
    }
    public abstract class BaseTower : MonoBehaviour
    {
        #region Adjustable Stats
        
        [SerializeField] private int baseDamage = 10;
        [SerializeField] private float baseSpeed = 1f;
        [SerializeField] private int reachRadius = 5;
        
        #endregion
        
        #region Xp Stats
        
        // Xp stats
        [SerializeField] private int currentXp;
        // Xp needed to lvl up
        [SerializeField] private int neededXp;
        // Xp increment to the neededXp after leveling up
        [SerializeField] private int xpIncOnLvlUp;

        protected int lvlPoints = 0;
        protected int currentLvl = 1;
        
        #endregion

        #region Privates

        private TowerType _type = TowerType.Unset;
        
        #endregion

        #region Public Properties
        
        public int BaseDamage => baseDamage;
        public float BaseSpeed => baseSpeed;
        public int ReachRadius => reachRadius;
        
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
        
        public int CurrentXp => currentXp;
        public int NeededXp => neededXp;
        public int XpIncOnLvlUp => xpIncOnLvlUp;
        public int LvlPoints => lvlPoints;
        public int CurrentLvl => currentLvl;
        #endregion

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
            currentXp += xp;
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
            while (currentXp >= neededXp)
            {
                lvlups++;
                currentXp -= neededXp;
                neededXp += xpIncOnLvlUp;
                
                // update the current levels
                currentLvl++;
                lvlPoints++;
                // call the lvlup callback
                AfterLevelUp(lvlups);
            }
            return lvlups;
        }
    }
}
