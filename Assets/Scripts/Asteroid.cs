using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _particleSystem;
    private MeshRenderer _meshRenderer;
    private Rigidbody _rigidbody;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.AddForce(transform.TransformVector(Vector3.forward)* Random.Range(-50   , 50));
    }

    public bool IsVisible => _meshRenderer.isVisible;

    public void Explode()
    {
        _meshRenderer.enabled = false;
        Instantiate(_particleSystem, transform.position, quaternion.identity).Play();
        GetComponent<Collider>().enabled = false;
        Destroy(this);
    }
}
