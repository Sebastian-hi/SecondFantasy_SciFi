using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceFlyThings : MonoBehaviour
{
    private float _speedFly = 2f;
    private float _hightFly = 0.5f;


    private Vector3 startPosition;
    private float _randomOffset;

    private void Start()
    {
        startPosition = transform.position;
        _randomOffset = Random.Range(0f, Mathf.PI * 2);
    }

    private void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * _speedFly + _randomOffset) * _hightFly;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}