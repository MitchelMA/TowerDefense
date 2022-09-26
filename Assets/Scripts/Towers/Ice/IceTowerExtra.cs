using System;
using UnityEngine;

namespace Towers.Ice
{
    public partial class IceTower
    {
        [Serializable]
        public struct EffectStats
        {
            public float slowness;
            public float duration;
            public Color effectColour;
        }

        [Serializable]
        public struct EffectStatsUp
        {
            public float slowness;
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
            // Call the base class' Start method
            base.Start();
            
            // set the effect stats
            _currentEffectStats = new EffectStats
            {
                slowness = baseEffectStats.slowness,
                duration = baseEffectStats.duration,
                effectColour = baseEffectStats.effectColour,
            };
            
            // Set the corresponding type
            Type = TowerType.Ice;

            EffectStatsChanged += EffectStatsHaveChanged;
            
            // simulate on parentSelect
            OnParentSelect(parentNode.Selectable, parentNode.Selectable.IsSelected);
        }
    }
}