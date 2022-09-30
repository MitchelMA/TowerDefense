using System;
using System.Collections.Generic;
using Currency;
using MouseControl;
using PathFinding;
using Towers;
using Towers.Projectile;
using UnityEngine;
using UnityEngine.UI;

namespace Monsters
{
    [RequireComponent(typeof(PathTraverser))]
    public abstract class BaseMonster : MonoBehaviour
    {
        #region Enums

        public enum MonsterType
        {
            Easy,
            Medium,
            Hard,
        }

        #endregion

        #region Structs

        [Serializable]
        public struct Stats
        {
            public float speed;
            public int hp;
            public int damage;
        }

        #endregion

        [SerializeField] protected MonsterType type;
        protected WaveController waveController;
        [SerializeField] protected Transform overlayCanvas;
        [SerializeField] protected Camera targetCamera;
        [SerializeField] protected int referenceScreenHeight = 800;
        [SerializeField] protected float minAboveMonster = 40;
        [SerializeField] protected GameObject hpSliderPrefab;
        [SerializeField] protected float hpSliderSize = 1;
        [SerializeField] protected Color fullHpColour;
        [SerializeField] protected Color depletedHpColour;
        protected GameObject CurrentHpSlider;
        public Stats baseStats;

        public MonsterType Type => type;

        protected PathTraverser PathTraverser;
        protected CircleCollider2D HitCollider;
        protected CurrencyController CurrencyController;


        protected readonly List<BaseEffect> Effects = new List<BaseEffect>();
        protected Stats CurrentStats;

        protected bool WasKilled = false;
        protected readonly List<BaseTower> HitBy = new List<BaseTower>();

        private Vector3 _lastpos;
        private Vector3 _curpos;
        public Vector3 UDir => (_curpos - _lastpos).normalized;
        public float Speed => PathTraverser.Speed;

        public event EventHandler<int> HpChange; 

        // Start is called before the first frame update
        protected virtual void Start()
        {
            PathTraverser = GetComponent<PathTraverser>();
            waveController = GameObject.FindWithTag("WaveController").GetComponent<WaveController>();
            
            SetSpeed(baseStats.speed);
            CurrentStats.hp = baseStats.hp;
            HitCollider = GetComponent<CircleCollider2D>();
            _curpos = transform.position;
            CurrentHpSlider = Instantiate(hpSliderPrefab, overlayCanvas);
            CurrentHpSlider.transform.localScale = new Vector3(hpSliderSize, hpSliderSize, hpSliderSize);
            CurrencyController = GameObject.FindWithTag("CurrencyController").GetComponent<CurrencyController>();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            // calculating the moving direction
            _lastpos = _curpos;
            _curpos = transform.position;
            
            
            if(Effects.Count > 0) UpdateStatuses();
            
            // update the hp-slider
            if(!WasKilled)
                UpdateHPUI();
        }

        protected void UpdateStatuses()
        {
            List<BaseEffect> woreOff = new List<BaseEffect>();
            foreach (BaseEffect effect in Effects)
            {
                if (effect.CurrentTimeout > 0)
                {
                    effect.DecreaseTimeout(Time.deltaTime);
                    continue;
                }

                if (effect.Duration <= 0)
                {
                    woreOff.Add(effect);
                    continue;
                }
                
                // effect's wait-timeout passed
                effect.ApplyEffect(this);
                effect.ResetTimeout();
            }
            
            // remove the worn off
            foreach (BaseEffect wornOff in woreOff)
            {
                wornOff.WornOff(this);
                Effects.Remove(wornOff);
            }
        }

        protected virtual void OnDestroy()
        {
            waveController.DecreaseLeft();
            // destroy the enemy hp-bar
            Destroy(CurrentHpSlider);
            // calculate its value only when it was killed
            if (WasKilled)
            {
                int value = (int)(baseStats.hp / 2 + baseStats.speed*2);
                CurrencyController.Add(value);
                foreach (BaseTower hit in HitBy)
                    hit.XpUp(value);
            }
        }

        protected void UpdateHPUI()
        {
            Vector3 screenPos = targetCamera.WorldToScreenPoint(transform.position);
            float heightMult = Screen.height / (float)referenceScreenHeight;
            float minAbove = minAboveMonster * heightMult;
            screenPos.y += minAbove;
            CurrentHpSlider.transform.position = screenPos;
            // try to get the slider-component from the hp-bar
            if (!CurrentHpSlider.TryGetComponent(out Slider slider))
            {
                Debug.LogError("UI hp-bar prefab did not contain a Slider component");
                return;
            }

            slider.value = CurrentStats.hp / (float) baseStats.hp;
            // try to get the fore-ground element of the slider (second element, at index 1)
            Transform foreChild = slider.transform.GetChild(1);
            if (!foreChild)
            {
                Debug.LogError("UI hp-bar prefab did not have a foreground child");
                return;
            }

            if (!foreChild.TryGetComponent(out Image foreground))
            {
                Debug.LogError("Foreground child did not contain an Image component");
                return;
            }

            foreground.color = Color.Lerp(depletedHpColour, fullHpColour, slider.value);

        }

        public bool GiveEffect(BaseEffect effect)
        {
            foreach (BaseEffect lEffect in Effects)
            {
                if (lEffect.Type == effect.Type)
                    return false;
            }
            
            // it was not in the list of effects as to not add multiple of one type
            Effects.Add(effect);
            return true;
        }

        public void GainDamage(int amount, BaseTower from)
        {
            CurrentStats.hp -= amount;
            // null is allowed to make the damageGiver anonymous
            if(from is not null)
                AddHitBy(from);
            HpChange?.Invoke(this, CurrentStats.hp);
            if (CurrentStats.hp <= 0)
            {
                WasKilled = true;
                Destroy(gameObject);
            }
        }
        
        public void SetSpeed(float value)
        {
            CurrentStats.speed = value;
            PathTraverser.Speed = value;
        }

        private bool AddHitBy(BaseTower tower)
        {
            if (HitBy.Contains(tower))
                return false;
            
            HitBy.Add(tower);
            return true;
        }
    }
}
