using UnityEngine;
using UnityEngine.Serialization;

public class LearningPortalTracker: MonoBehaviour
{
    public Transform playerTransform;
    public Transform portalTransform;


    private void Update()
    {
        if (!playerTransform || !portalTransform)
            return;

        var direction = portalTransform.position - playerTransform.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
