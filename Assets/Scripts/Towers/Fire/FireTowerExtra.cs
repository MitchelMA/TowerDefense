using System;
using Towers.Projectile;
using UnityEngine;

namespace Towers.Fire
{
    // Additions to the Fire Tower in this file
    public partial class FireTower
    {
        [Serializable]
        public struct EffectStats
        {
            public int damage;
            public float interval;
            public float duration;
            public Color effectColour;
        }

        public EffectStats baseEffectStats;

        private EffectStats _currentEffectStats;
        protected override void Start()
        {
            // Call the base class' Start method
            base.Start();
            // set the effect stats
            _currentEffectStats = new EffectStats
            {
                damage = baseEffectStats.damage,
                duration = baseEffectStats.duration,
                effectColour = baseEffectStats.effectColour,
                interval = baseEffectStats.interval,
            };
            
            // Set the corresponding type
            Type = TowerType.Fire;
            
            Debug.Log($"Type: {Type}");
        }
    }
}