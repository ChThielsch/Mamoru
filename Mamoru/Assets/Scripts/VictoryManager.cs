using UnityEngine;

public class VictoryManager : MonoBehaviour
{
    public CameraController camController;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag=="Player")
        {
            Debug.Log("ja");
            camController.VictoryZoomOut();
        }
    }
}
