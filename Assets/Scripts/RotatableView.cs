using UnityEngine;

namespace RenderingExperiments
{
    public class RotatableView : MonoBehaviour
    {
        [SerializeField] private bool _randomizeAxisOnStart;
        [SerializeField] private Vector3 _axis;
        [SerializeField] private float _speed = 100;
        
        private const int MinRandomAngle = -100;
        private const int MaxRandomAngle = 100;

        private void Start()
        {
            if (_randomizeAxisOnStart)
            {
                RandomizeAxis();
            }
        }

        private void Update()
        {
            transform.Rotate(_axis.normalized * Time.deltaTime * _speed);
        }

        private void RandomizeAxis()
        {
            _axis = new Vector3
            (
                Random.Range(MinRandomAngle, MaxRandomAngle),
                Random.Range(MinRandomAngle, MaxRandomAngle),
                Random.Range(MinRandomAngle, MaxRandomAngle)
            ).normalized;
        }
    }
}