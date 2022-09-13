using UnityEngine;

namespace Towers.Fire
{
    // Required implementations only
    public partial class FireTower : BaseTower
    {
        protected override void AfterGainingXp(int levelsGained, int xpGained)
        {
            Debug.Log($"Tower of type {Type} is done Gaining Xp: went from level {currentLvl - levelsGained} to level {currentLvl} after gaining {xpGained} amount of xp\nLvlPoints: {LvlPoints}");
        }

        protected override void AfterLevelUp(int successiveCount)
        {
            Debug.Log($"Tower of type {Type} just leveled up: {currentLvl -1} -> {currentLvl}\nCalled in a row: {successiveCount}");
        }
    }
}