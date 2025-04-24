using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform _target;

    [Space(1)]
    [Header("Offset")]
    public Vector3 _offset = Vector3.zero;

    [Space(1)]
    [Header("Limits")]
    public Vector2 _limits = new Vector2(5, 3);

    [Space(1)]
    [Header("Smooth Damp Time")]
    [Range(0, 1)]
    public float _smoothTime;

    private Vector3 _velocity = Vector3.zero;

    private void Update()
    {
        if (!Application.isPlaying)
        {
            transform.localPosition = _offset;
        }

        FollowTarget(_target);
    }

    private void LateUpdate()
    {
        Vector3 localPos = transform.localPosition;
        transform.localPosition = new Vector3(Mathf.Clamp(localPos.x, -_limits.x, _limits.x), Mathf.Clamp(localPos.y, -_limits.y, _limits.y), localPos.z);
    }

    public void FollowTarget(Transform t)
    {
        Vector3 localPos = transform.localPosition;
        Vector3 targetLocalPos = t.transform.localPosition;
        transform.localPosition = Vector3.SmoothDamp(localPos, new Vector3(targetLocalPos.x + _offset.x, targetLocalPos.y + _offset.y, localPos.z), ref _velocity, _smoothTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(-_limits.x, -_limits.y, transform.position.z), new Vector3(_limits.x, -_limits.y, transform.position.z));
        Gizmos.DrawLine(new Vector3(-_limits.x, _limits.y, transform.position.z), new Vector3(_limits.x, _limits.y, transform.position.z));
        Gizmos.DrawLine(new Vector3(-_limits.x, -_limits.y, transform.position.z), new Vector3(-_limits.x, _limits.y, transform.position.z));
        Gizmos.DrawLine(new Vector3(_limits.x, -_limits.y, transform.position.z), new Vector3(_limits.x, _limits.y, transform.position.z));
    }
}
