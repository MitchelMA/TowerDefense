using System;
using Towers.Projectile;
using UnityEngine;

namespace Towers.Ice
{
    public partial class IceTower : BaseTower
    {
        protected override bool CreateEffect(out BaseEffect effect)
        {
            return base.CreateEffect(out effect);
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
            
        }
    }
}