using UnityEngine;

public class RotateCar : MonoBehaviour
{
    public float rotateSpeed = 20f;

    void Update()
    {
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }
}