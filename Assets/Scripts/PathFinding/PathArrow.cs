using UnityEngine;

namespace PathFinding
{
    public class PathArrow : MonoBehaviour
    {
        [SerializeField] private float amplitude = 1f;
        [SerializeField] private float seriesDur = 1f;

        private Vector3 _startpos;
        private float _seriesProgression = 0f;
        // Start is called before the first frame update
        private void Start()
        {
            _startpos = transform.position;
        }

        // Update is called once per frame
        private void Update()
        {
            _seriesProgression = (_seriesProgression + Time.deltaTime) % (Mathf.PI / seriesDur);
            float scaleValue = Mathf.Pow(amplitude * Mathf.Sin(_seriesProgression * seriesDur), 2);
            Vector3 newPos = transform.up * scaleValue + _startpos;
            transform.position = newPos;
        }
    }
}
