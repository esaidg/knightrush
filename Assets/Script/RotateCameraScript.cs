using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCameraScript : MonoBehaviour
{
    public void rotateCameraLeft()
    {
        transform.Rotate(Vector3.up, 45, Space.Self);
    }

    public void rotateCameraRight()
    {
        transform.Rotate(Vector3.up, -45, Space.Self);
    }
}
