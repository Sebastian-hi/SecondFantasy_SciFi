using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JustRotate : MonoBehaviour
{
    [SerializeField] private float _SpeedRotate = 50f;

    public enum RotateAxis
    {
        RotateX,
        RotateY, 
        RotateZ
    }

    public RotateAxis axis;

    private Vector3 _startPosition;

    private void Start()
    {
        _startPosition = transform.position;
    }

    private void Update()
    {
        if (axis == RotateAxis.RotateX)
        {
            transform.Rotate(_SpeedRotate * Time.deltaTime, 0, 0);
        }
        else if (axis == RotateAxis.RotateY)
        {
            transform.Rotate(0, _SpeedRotate * Time.deltaTime, 0);
        }
        else
        {
            transform.Rotate(0, 0, _SpeedRotate * Time.deltaTime);
        }
    }
}