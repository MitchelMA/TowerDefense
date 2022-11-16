using System;
using System.Collections.Generic;
using Currency;
using MouseControl;
using UI;
using UnityEngine;

namespace Towers
{
    [RequireComponent(typeof(TowerSelectable))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class TowerNode : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer indicator;
        [SerializeField] private float minIndicatorSize;
        [SerializeField] private float indicatorAmplitude;
        [SerializeField] private float indicatorSeries;
        private float _seriesProgression = 0;
        private BaseTower _currentTower;
        private bool _indicating = false;

        public bool Indicating
        {
            get => _indicating;
            set
            {
                // don't change when the value is the same
                if(_indicating == value)
                    return;
                
                _indicating = value;
                IndicatingStateChange?.Invoke(this, value);
            }
        }

        public BaseTower CurrentTower => _currentTower;
        
        public bool HasTower => _currentTower;

        private TowerSelectable _selectable;
        public TowerSelectable Selectable => _selectable;

        public event EventHandler<bool> IndicatingStateChange;
        

        // Start is called before the first frame update
        private void Start()
        {
            _selectable = GetComponent<TowerSelectable>();
            _selectable.OnStatusChanged += OnSelect;
            CurrencyController.Instance.MoneyChanged += HandleCurrencyChange;
            IndicatingStateChange += HandleIndicatingStateChange;
            Indicating = true;
        }

        private void HandleCurrencyChange(object sender, ulong newAmount)
        {
            if (HasTower)
                return;

            List<ulong> sorted = CurrencyController.Instance.TowerPrices;
            sorted.Sort();
            ulong lowest = sorted[0];
            Indicating = newAmount >= lowest;
        }

        private void HandleIndicatingStateChange(object sender, bool newStatus)
        {
            if (newStatus)
                IndicatingStart();
            else
                IndicatingEnd();
        }

        private void IndicatingStart()
        {
            indicator.gameObject.SetActive(true);
        }

        private void IndicatingEnd()
        {
            indicator.gameObject.SetActive(false);
        }

        private void OnSelect(object sender, bool selectedStatus)
        {
            if (!selectedStatus)
                return;

            // reset the indicating when the node has a tower
            if (HasTower)
                Indicating = false;
        }

        // Update is called once per frame
        private void Update()
        {
            // show the indication
            if (!_indicating)
                return;

            _seriesProgression = (_seriesProgression + Time.deltaTime) % (Mathf.PI / indicatorSeries);
            float scaleValue = minIndicatorSize + Mathf.Pow( indicatorAmplitude * Mathf.Sin(_seriesProgression * indicatorSeries), 2);
            Vector3 scaleVector = new Vector3(scaleValue, scaleValue);
            indicator.transform.localScale = scaleVector;
        }

        public bool PlaceTower(BaseTower.TowerType type)
        {
            if (!TowerFactory.Instance.CreateTower(type, out GameObject tower))
            {
                return false;
            }

            if (!tower.TryGetComponent(out BaseTower towerScript))
            {
                Debug.LogError($"Tower GameObject didn't contain a component of type {typeof(BaseTower)}");
                return false;
            }

            _currentTower = Instantiate(towerScript, transform);
            // stop indicating
            Indicating = false;
            return true;
        }

        public BaseTower.TowerType RemoveTower()
        {
            BaseTower.TowerType type = BaseTower.TowerType.Unset;
            if (!HasTower)
                return type;
            
            _selectable.Deselect();
            type = _currentTower.Type;
            Destroy(_currentTower.gameObject);
            _currentTower = null;
            return type;
        }

        public bool PlaceTower(int type)
        {
            return PlaceTower((BaseTower.TowerType) type);
        }
    }
}
