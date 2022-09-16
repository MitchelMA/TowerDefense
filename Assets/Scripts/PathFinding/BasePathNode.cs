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
            if (traverser.PathHistory.TryPop(out BasePathNode last))
            {
                traverser.TargetNode = last;
                return;
            }
            
            // when the path-history is empty, force the traverser to go forwards
            traverser.GoForwards();
        }
    }
}
