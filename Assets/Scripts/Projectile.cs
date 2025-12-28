using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 5f;

    private Vector3 _direction;

    public void Init(Vector3 direction)
    {
        _direction = direction;
    }

    private void Update()
    {
        transform.position += _direction * (speed * Time.deltaTime);

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f)
            Destroy(gameObject);
    }
}