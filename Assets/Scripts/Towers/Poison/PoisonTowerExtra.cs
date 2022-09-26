using System;
using UnityEngine;

namespace Towers.Poison
{
    public partial class PoisonTower
    {
        [Serializable]
        public struct EffectStats
        {
            public int damage;
            public float interval;
            public float duration;
            public Color effectColour;
        }

        [Serializable]
        public struct EffectStatsUp
        {
            public int damage;
            public float interval;
            public float duration;
        }

        public struct EffectStatsDiff
        {
            public EffectStats Previous;
            public EffectStats Current;
        }

        public EffectStats baseEffectStats;

        private EffectStats _currentEffectStats;

        [SerializeField] private EffectStatsUp effectStatsIncrease;

        public event EventHandler<EffectStatsDiff> EffectStatsChanged;

        protected override void Start()
        {
            base.Start();

            _currentEffectStats = new EffectStats
            {
                damage = baseEffectStats.damage,
                duration = baseEffectStats.duration,
                effectColour = baseEffectStats.effectColour,
                interval = baseEffectStats.interval,
            };

            Type = TowerType.Poison;

            EffectStatsChanged += EffectStatsHaveChanged;
            
            // simulate on parentSelect
            OnParentSelect(parentNode.Selectable, parentNode.Selectable.IsSelected);
        }
    }
}