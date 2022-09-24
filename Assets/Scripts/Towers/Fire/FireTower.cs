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

        protected override void UpdateStatsDisplay()
        {
            base.UpdateStatsDisplay();
            // show the effect stats
            statsUI.EffectStatsTexts.value.text = $"Damage: {_currentEffectStats.damage}";
            statsUI.EffectStatsTexts.interval.text = $"Interval: {(1 / _currentEffectStats.interval):0.00}p/s";
            statsUI.EffectStatsTexts.duration.text = $"Duration: {_currentEffectStats.duration:0.00}s";
        }

        public override void UpdateStatsBtns()
        {
            base.UpdateStatsBtns();
            
            statsUI.EffectStatsBtns.duration.onClick.RemoveAllListeners();
            statsUI.EffectStatsBtns.duration.onClick.AddListener(delegate
            {
                IncreaseEffectStats(0, effectStatsIncrease.duration, 0);
                CurrentXpStats.lvlPoints--;
                UpdateStatsBtns();
            });
            
            statsUI.EffectStatsBtns.interval.onClick.RemoveAllListeners();
            statsUI.EffectStatsBtns.interval.onClick.AddListener(delegate
            {
                IncreaseEffectStats(0, 0, effectStatsIncrease.interval);
                CurrentXpStats.lvlPoints--;
                UpdateStatsBtns();
            });
            
            statsUI.EffectStatsBtns.value.onClick.RemoveAllListeners();
            statsUI.EffectStatsBtns.value.onClick.AddListener(delegate
            {
                IncreaseEffectStats(effectStatsIncrease.damage, 0, 0);
                CurrentXpStats.lvlPoints--;
                UpdateStatsBtns();
            });
        }
        

        protected override void AfterLevelUp(int successiveCount)
        {
            base.AfterLevelUp(successiveCount);
            
            IncreaseEffectStats(effectStatsIncrease.damage, effectStatsIncrease.duration, effectStatsIncrease.interval);
        }

        private void IncreaseEffectStats(int damage, float duration, float interval)
        {
            EffectStats old = new EffectStats
            {
                damage = _currentEffectStats.damage,
                duration = _currentEffectStats.duration,
                effectColour = new Color(_currentEffectStats.effectColour.a, _currentEffectStats.effectColour.g,
                    _currentEffectStats.effectColour.b),
                interval = _currentEffectStats.interval,
            };
            _currentEffectStats.damage += damage;
            _currentEffectStats.duration += duration;
            _currentEffectStats.interval = 1 / (1 / _currentEffectStats.interval + interval);
            EffectStats newS = new EffectStats()
            {
                damage = _currentEffectStats.damage,
                duration = _currentEffectStats.duration,
                effectColour = new Color(_currentEffectStats.effectColour.a, _currentEffectStats.effectColour.g,
                    _currentEffectStats.effectColour.b),
                interval = _currentEffectStats.interval,
            };

            EffectStatsDiff diff = new EffectStatsDiff
            {
                Previous = old,
                Current = newS
            };
            
            EffectStatsChanged?.Invoke(this, diff);
        }
    }
}