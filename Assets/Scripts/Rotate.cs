using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float angleSpeed;

    private void Update()
    {
        Vector3 angle = transform.rotation.eulerAngles;
        angle.z += angleSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(angle);
    }
}
