using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongScaler : MonoBehaviour
{
    [SerializeField]
    private float _timeMultiplier;
    [SerializeField]
    private float _scale = 1;
    

    void Update()
    {
        var sineTime = Mathf.Sin(Time.time * _timeMultiplier) + _scale +1;
        var sineScale = new Vector3(sineTime , sineTime , sineTime);

        if (sineScale.magnitude >= Vector3.one.magnitude)
        {
            transform.localScale = sineScale;
        }
    }
}
