using UnityEngine;

namespace PathFinding
{
    public abstract class BasePathNode : MonoBehaviour
    {
        // Start is called before the first frame update
        protected virtual void Start()
        {
        
        }

        // Update is called once per frame
        protected virtual void Update()
        {
        
        }

        public abstract void CallBack(PathTraverser traverser);

        protected static void HandleBackwards(PathTraverser traverser)
        {
            
        }
    }
}
