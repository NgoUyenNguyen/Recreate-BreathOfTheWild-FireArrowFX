using UnityEngine;

public class CameraFollowPoint : MonoBehaviour
{
    [SerializeField] private Transform _followTarget;

    private Transform FollowTarget { get => _followTarget;}

    private void Update()
    {
        transform.position = FollowTarget.position;
    }
}
