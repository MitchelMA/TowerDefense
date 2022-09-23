using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TowerStatsUI : MonoBehaviour
    {
        #region Structs

        [Serializable]
        public struct StandAlone
        {
            public Text typeText;
            public Text pointText;
            public Text xpCounter;
            public Button closeBtn;
            public Button destroyBtn;
        }

        [Serializable]
        public struct StatsText
        {
            public Text damage;
            public Text fireRate;
            public Text projectileSpeed;
            public Text radius;
        }

        [Serializable]
        public struct StatsBtn
        {
            public Button damage;
            public Button fireRate;
            public Button projectileSpeed;
            public Button radius;
        }

        [Serializable]
        public struct EffectStatsText
        {
            public Text value;
            public Text interval;
            public Text duration;
        }

        [Serializable]
        public struct EffectStatsBtn
        {
            public Button value;
            public Button interval;
            public Button duration;
        }

        public StandAlone StandAlones;
        public StatsText StatsTexts;
        public StatsBtn StatsBtns;
        public EffectStatsText EffectStatsTexts;
        public EffectStatsBtn EffectStatsBtns;

        #endregion
    }
}
