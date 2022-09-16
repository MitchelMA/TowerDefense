using UnityEngine;

namespace PathFinding
{
    public class SplitPathNode : BasePathNode
    {
        [SerializeField] private BasePathNode[] paths = new BasePathNode[2];
        public override void CallBack(PathTraverser traverser)
        {
            if (traverser.Backwards)
            {
                HandleBackwards(traverser);
                return;
            }

            BasePathNode nextNode = paths[Random.Range(0, paths.Length)];
            traverser.PathHistory.Push(this);
            traverser.TargetNode = nextNode;
        }
    }
}
