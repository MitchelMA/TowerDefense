using UnityEngine;
using UnityEngine.UI;

namespace Health
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] private Color fullColour;
        [SerializeField] private Color depletedColour;
        private HealthController _controller;

        private Slider _slider;

        private Image _healthColour;
        // Start is called before the first frame update
        private void Start()
        {
            _slider = GetComponent<Slider>();
            _healthColour = transform.GetChild(1).GetComponent<Image>();
            _controller = GameObject.FindWithTag("HealthController").GetComponent<HealthController>();
            _controller.HealthChanged += UpdateHealth;
        }

        // Update is called once per frame
        private void Update()
        {
        
        }

        private void UpdateHealth(object sender, HealthController.HealthChangeEventArgs args)
        {
            _slider.value = args.NewAmount / (float)_controller.StartHealth;
            _healthColour.color = Color.Lerp(depletedColour, fullColour, _slider.value);
        }
    }
}
