using UnityEngine;

public class PortalTracker : MonoBehaviour
{
    private static Transform _playerTransform;
    private static Transform _portalTransform;

    public static void Initialize(Transform player, Transform portal)
    {
        _playerTransform = player;
        _portalTransform = portal;
    }

    private void Update()
    {
        if (!_playerTransform || !_portalTransform)
            return;

        var direction = _portalTransform.position - _playerTransform.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}