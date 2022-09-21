using System;
using System.Reflection;
using Towers.Projectile;
using UnityEngine;

namespace Towers.Fire
{
    // Required implementations only
    public partial class FireTower : BaseTower
    {
        protected override void AfterGainingXp(int levelsGained, int xpGained)
        {
            Debug.Log($"Tower of type {Type} is done Gaining Xp: went from level {CurrentXpStats.currentLvl - levelsGained} to level {CurrentXpStats.currentLvl} after gaining {xpGained} amount of xp\nLvlPoints: {CurrentXpStats.lvlPoints}");
        }

        protected override bool CreateEffect(out BaseEffect effect)
        {
            effect = new FireEffect(_currentEffectStats, Type);
            return true;
        }

        protected override void AfterLevelUp(int successiveCount)
        {
            Debug.Log($"Tower of type {Type} just leveled up: {CurrentXpStats.currentLvl -1} -> {CurrentXpStats.currentLvl}\nCalled in a row: {successiveCount}");
        }
    }
}