using System;
using Currency;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TowerBuyUI : MonoBehaviour
    {
        #region Structs

        [Serializable]
        public struct BuyBtns
        {
            public BuyBtn fire;
            public BuyBtn ice;
            public BuyBtn poison;
        }
        
        #endregion
        public Button CloseBtn;
        public BuyBtns BuyButtons;
    }
}
