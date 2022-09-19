using System;
using MouseControl;
using PathFinding;
using UnityEngine;

namespace Monsters
{
    [RequireComponent(typeof(PathTraverser))]
    [RequireComponent(typeof(EnemySelectable))]
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

        [SerializeField] protected MonsterType type;

        public MonsterType Type => type;

        protected PathTraverser PathTraverser;
        protected EnemySelectable Selectable;
    
        // Start is called before the first frame update
        protected virtual void Start()
        {
            // set inactive at start
            PathTraverser = GetComponent<PathTraverser>();
            Selectable = GetComponent<EnemySelectable>();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
        
        }

        protected virtual void OnDestroy()
        {
            Selectable.Deselect();
        }
    }
}
