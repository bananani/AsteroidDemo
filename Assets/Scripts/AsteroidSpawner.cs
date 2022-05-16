using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField]
    private Asteroid _bigAsteroid;
    [SerializeField]
    private Asteroid _smallAsteroid;
    [SerializeField]
    private float _numberOfAsteroids = 200;

    private IEnumerator Start()
    {
        for (int i = 0; i < _numberOfAsteroids / 2; i++)
        {
            var asteroid = Instantiate(_bigAsteroid, GetRandomPosition(), GetRandomRotation());
            yield return null;
        }

        for (int i = 0; i < _numberOfAsteroids / 2; i++)
        {
            var asteroid = Instantiate(_smallAsteroid, GetRandomPosition(), GetRandomRotation());
            
            yield return null;
        }
    }

    Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(-250, 250), Random.Range(-250, 250), Random.Range(-250, 250));
    }

    Quaternion GetRandomRotation()
    {
        return Quaternion.Euler(new Vector3(Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360)));
    }

}
