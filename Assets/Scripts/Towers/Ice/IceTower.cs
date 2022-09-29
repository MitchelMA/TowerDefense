using System;
using Towers.Projectile;
using UnityEngine;

namespace Towers.Ice
{
    public partial class IceTower : BaseTower
    {
        protected override bool CreateEffect(out BaseEffect effect)
        {
            effect = new IceEffect(_currentEffectStats, Type);
            return true;
        }

        private void EffectStatsHaveChanged(object sender, EffectStatsDiff diff)
        {
            if(parentNode.Selectable.IsSelected)
                UpdateStatsDisplay();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EffectStatsChanged -= EffectStatsHaveChanged;
        }

        public override void UpdateStatsDisplay()
        {
            base.UpdateStatsDisplay();
            statsUI.EffectStatsTexts.value.text = $"Slowness: {Math.Ceiling(_currentEffectStats.slowness * 100)}%";
            // don't show show the interval as it's not of importance
            statsUI.EffectStatsTexts.interval.text = "";
            statsUI.EffectStatsTexts.duration.text = $"Duration: {_currentEffectStats.duration}";
        }

        public override void UpdateStatsBtns()
        {
            base.UpdateStatsBtns();
            
            statsUI.EffectStatsBtns.duration.onClick.RemoveAllListeners();
            statsUI.EffectStatsBtns.duration.onClick.AddListener(delegate
            {
                IncreaseEffectStats(0, effectStatsIncrease.duration);
                CurrentXpStats.lvlPoints--;
                parentNode.Indicating = false;
                UpdateStatsBtns();
                UpdateStatsDisplay();
            });
            
            // don't show the interval as it's not of importance 
            statsUI.EffectStatsBtns.interval.gameObject.SetActive(false);
            
            statsUI.EffectStatsBtns.value.onClick.RemoveAllListeners();
            statsUI.EffectStatsBtns.value.onClick.AddListener(delegate
            {
                IncreaseEffectStats(effectStatsIncrease.slowness, 0);
                CurrentXpStats.lvlPoints--;
                parentNode.Indicating = false;
                UpdateStatsBtns();
                UpdateStatsDisplay();
            });
        }

        protected override void AfterLevelUp(int successiveCount)
        {
            base.AfterLevelUp(successiveCount);

            IncreaseEffectStats(effectStatsIncrease.slowness, effectStatsIncrease.duration);
        }

        private void IncreaseEffectStats(float slowness, float duration)
        {
            EffectStats old = new EffectStats
            {
                slowness = _currentEffectStats.slowness,
                duration = _currentEffectStats.duration,
                effectColour = _currentEffectStats.effectColour,
            };
            _currentEffectStats.slowness += slowness;
            _currentEffectStats.duration += duration;
            EffectStats newS = new EffectStats
            {
                slowness = _currentEffectStats.slowness,
                duration = _currentEffectStats.duration,
                effectColour = _currentEffectStats.effectColour,
            };

            EffectStatsDiff diff = new EffectStatsDiff
            {
                Previous = old,
                Current = newS,
            };
            
            EffectStatsChanged?.Invoke(this, diff);
        }
    }
}