using UnityEngine;

namespace PathFinding
{
    public class SplitNonRepeat : BasePathNode
    {
        [SerializeField] private BasePathNode[] paths = new BasePathNode[2];
        public override void CallBack(PathTraverser traverser)
        {
            if (traverser.Backwards)
            {
                HandleBackwards(traverser);
                return;
            }

            BasePathNode nextNode = GetNext(traverser);
            traverser.PathHistory.Push(this);
            traverser.TargetNode = nextNode;
        }

        private BasePathNode GetNext(PathTraverser traverser)
        {
            BasePathNode next = default;
            while (next is null)
            {
                BasePathNode test = paths[Random.Range(0, paths.Length)];
                if (traverser.PathHistory.Contains(test)) continue;

                next = test;
            }

            return next;
        }
    }
}