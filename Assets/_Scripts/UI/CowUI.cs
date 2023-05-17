using UnityEngine;

namespace _Scripts.UI
{
    public class CowUI : MonoBehaviour
    {
        [SerializeField] Rigidbody2D _rigidbody2D;
        [SerializeField] float _speed;

        void Start()
        {
            _rigidbody2D.velocity = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10)).normalized * _speed;
            _rigidbody2D.AddTorque(Random.Range(-30, 30));
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            _rigidbody2D.AddTorque(Random.Range(-30, 30));
        }
    }
}