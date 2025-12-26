using UnityEngine;
using UnityEngine.UI;

public class WebcamTest : MonoBehaviour
{
    public RawImage display;
    public int webcamIndex = 0;

    private WebCamTexture cam;

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.LogError("No Webcam");
            return;
        }

        Debug.Log("Webcams:");
        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log(i + " : " + devices[i].name);
        }

        cam = new WebCamTexture(devices[webcamIndex].name, 640, 480, 30);
        cam.Play();

        if (display != null)
        {
            display.texture = cam;
            //display.rectTransform.localRotation = Quaternion.Euler(0, 0, cam.videoRotationAngle);
        }
    }

    void Update()
    {
        if (cam != null && cam.isPlaying && display != null)
        {
            float scaleY = cam.videoVerticallyMirrored ? -1.0f : 1.0f;
            display.rectTransform.localScale = new Vector3(1, scaleY, 1);
        }
    }

    void OnDestroy()
    {
        if (cam != null && cam.isPlaying)
            cam.Stop();
    }
}
