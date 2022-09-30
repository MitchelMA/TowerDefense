using UnityEngine;

namespace CameraScripts
{
    public class ShowScene : MonoBehaviour
    {
        [SerializeField] private Vector3 startPos;
        [SerializeField] private Vector3 endPos;
        [SerializeField] private float speed;
        [SerializeField] private float errorMargin = 1f;

    
        // Start is called before the first frame update
        private void Start()
        {
            transform.position = startPos;
        }

        // Update is called once per frame
        private void Update()
        {
            if (!Arrived())
            {
                transform.position = Vector3.MoveTowards(transform.position, endPos, speed * Time.deltaTime);
            }
            else
            {
                Destroy(this);
            }
        }

        private bool Arrived()
        {
            return Vector3.Distance(transform.position, endPos) <= errorMargin;
        }
    }
}
