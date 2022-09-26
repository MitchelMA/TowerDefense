using System;
using Towers.Projectile;
using UnityEngine;

namespace Towers.Ice
{
    public partial class IceTower : BaseTower
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
    }
}