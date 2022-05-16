using Unity.Mathematics;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField]
    private GameObject _missileGroup;
    [SerializeField]
    private ParticleSystem _particleSystem;
    [SerializeField]
    private Rigidbody _rigidbody;
    [SerializeField]
    private float _rotationSpeed = 50;
    [SerializeField]
    private AnimationCurve _rotationSpeedCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField]
    private float _rotationTimeMultiplier = 0.2f;
    
    [SerializeField]
    private float _forceMultiplier;
    [SerializeField]
    private AnimationCurve _forceMultiplierCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField]
    private float _forceTimeMultiplier = 0.3f;
    
    private Asteroid _asteroid;
    private Vector3 _asteroidPosition => _asteroid.transform.position;
    private Quaternion _lookRotation = quaternion.identity;
    private bool hasExploded;
    private float _currentRotationSpeed;
    private float _currentForceMultiplier;

    public void Setup(Asteroid asteroid)
    {
        _asteroid = asteroid;
    }

    private void Start()
    {
        Destroy(gameObject, 30f);
    }

    private void Update()
    {
        //The missile takes some time to gain speed and control, this means it can miss but I think it's more fun this way :) 
        _currentRotationSpeed = _rotationSpeedCurve.Evaluate(math.lerp(0, 1, Time.time * _rotationTimeMultiplier)) * _rotationSpeed;
        _currentForceMultiplier = _forceMultiplierCurve.Evaluate(math.lerp(0, 1, Time.time * _forceTimeMultiplier)) * _forceMultiplier;
        if (_asteroid == null)
        {
            _rigidbody.AddForce(transform.forward * _currentForceMultiplier);
            return;
        }

        var direction = (_asteroidPosition - transform.position).normalized;

        _lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * _currentRotationSpeed);
        _rigidbody.AddForce(transform.TransformVector(Vector3.forward) * _currentForceMultiplier);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasExploded)
        {
            return;
        }

        if (other.CompareTag("Asteroid"))
        {
            other.GetComponent<Asteroid>().Explode();
            Destroy(gameObject, 5f);
            hasExploded = true;
            _particleSystem.Stop();
            Destroy(gameObject);
        }
    }
}
