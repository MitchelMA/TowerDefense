using System;
using UnityEngine;

namespace MouseControl
{
    public abstract class BaseSelectable : MonoBehaviour
    {
        public enum SelectType
        {
            AutoDeselect,
            DeselectOnSelect,
        };

        [SerializeField] protected Camera targetCamera;
        [SerializeField] protected SelectType selectMode;

        public SelectType SelectMode => selectMode;

        protected bool _selected = false;
        public bool IsSelected => _selected;

        public event EventHandler<bool> OnStatusChanged;
        
        // Start is called before the first frame update
        protected virtual void Start()
        {
        
        }

        // Update is called once per frame
        protected virtual void Update()
        {
        
        }

        public virtual void Select()
        {
            _selected = true;
            OnStatusChanged?.Invoke(this, true);
        }

        public virtual void Deselect()
        {
            _selected = false;
            OnStatusChanged?.Invoke(this, false);
        }
    }
}
