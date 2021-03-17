
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [SerializeField] float smoothSpeed = 0.125f;
    public Vector3 offset;

    private void FixedUpdate()
    {
        Vector3 newVelocity = target.GetComponent<Rigidbody2D>().velocity;
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref newVelocity, smoothSpeed * Time.deltaTime);
        //Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
        // funny effect :D
        // transform.LookAt(target);
    }
}
