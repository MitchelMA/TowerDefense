using UnityEngine;

namespace PathFinding
{
    public class EndPathNode : BasePathNode
    {
        public override void CallBack(PathTraverser traverser)
        {
            traverser.GoBackwards();
            HandleBackwards(traverser);
        }
    }
}
