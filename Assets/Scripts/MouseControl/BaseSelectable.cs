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
        
        // Start is called before the first frame update
        protected virtual void Start()
        {
        
        }

        // Update is called once per frame
        protected virtual void Update()
        {
        
        }

        public abstract void Select();

        public abstract void Deselect();
    }
}
