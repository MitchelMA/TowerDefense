using MouseControl;

namespace PathFinding
{
    public class CameraEnd : BasePathNode
    {
        public override void CallBack(PathTraverser traverser)
        {
            if (traverser.TryGetComponent<MouseControlledCamera>(out var cam))
                cam.FreeFollow = true;
            
            Destroy(traverser);
        }
    }
}