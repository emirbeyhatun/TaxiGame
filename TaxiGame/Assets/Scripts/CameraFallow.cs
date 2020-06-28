using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFallow : MonoBehaviour
{
    public Transform fallowTransform;
    private Vector3 tempFallowPosVec;
    private float fallowDistance;

    void Start()
    {
        fallowDistance = Mathf.Abs(transform.position.z - fallowTransform.position.z);
    }
    void LateUpdate()
    {
        if(fallowTransform)
        {
            tempFallowPosVec = transform.position;
            tempFallowPosVec.x = fallowTransform.position.x;
            tempFallowPosVec.z = fallowTransform.position.z;
            tempFallowPosVec.z -= fallowDistance;
            transform.position = tempFallowPosVec;
        }
    }
}
