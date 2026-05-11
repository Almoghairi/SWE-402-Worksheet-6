using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [SerializeField, Range(20f, 160f), Tooltip("Degrees per second for camera orbit.")]
    private float rotationSpeed = 80f;

    private void Update()
    {
        transform.Rotate(Vector3.up, Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime);
    }
}
