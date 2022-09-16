using UnityEngine;

namespace PathFinding
{
    public class NormalPathNode : BasePathNode
    {
        [SerializeField] private BasePathNode nextNode;

        public override void CallBack(PathTraverser traverser)
        {
            // Handle backwards
            if (traverser.Backwards)
            {
                HandleBackwards(traverser);
                return;
            }
            
            traverser.PathHistory.Push(this);
            traverser.TargetNode = nextNode;
        }
    }
}
