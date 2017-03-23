using UnityEngine;
using System.Collections;

public class TriggerMovingPlatform : MonoBehaviour
{

    [SerializeField]
    [Tooltip("[missing description]")]
    private MovingPlatformKinematic platform;

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerCharacter>() != null)
        {
            this.platform.passWaypoint();
            this.gameObject.SetActive(false);
        }
    }
}