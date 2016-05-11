using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class CameraOverlayChange : MonoBehaviour
{
    private ScreenOverlay cameraOverlay;
    private bool onDiscoFloor;

    void Start()
    {
        cameraOverlay = Camera.main.GetComponent<ScreenOverlay>();
        onDiscoFloor = false;
    }

    void Update()
    {
        if (!onDiscoFloor)
        {
            cameraOverlay.intensity = Mathf.Lerp(cameraOverlay.intensity, 0, 1 * Time.deltaTime);
        }
        else
        {
            cameraOverlay.intensity = Mathf.Lerp(cameraOverlay.intensity, 0.5f, 1 * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            onDiscoFloor = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        onDiscoFloor = false;
    }
}
