using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class MediaPipeUDPReceiver : MonoBehaviour
{
    Thread receiveThread;
    UdpClient client;
    public int port = 5005;
    private string lastData = "";
    private bool isNew = false;

    void Start() {
        receiveThread = new Thread(() => {
            client = new UdpClient(port);
            while (true) {
                try {
                    IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = client.Receive(ref anyIP);
                    lastData = Encoding.UTF8.GetString(data);
                    isNew = true;
                } catch { }
            }
        });
        receiveThread.IsBackground = true; receiveThread.Start();
    }

    void Update() {
        if (isNew) {
            string[] hands = lastData.Split('|');
            ParseHand(hands[0], true);
            ParseHand(hands[1], false);
            isNew = false;
        }
    }

    void ParseHand(string data, bool isLeft) {
        string[] p = data.Split(',');
        float x = float.Parse(p[0]);
        float y = float.Parse(p[1]);
        bool active = p[3] == "1";

        Vector3 worldPos = Camera.main.ViewportToWorldPoint(new Vector3(x, 1-y, HandLandmarkToWorld.Instance.handDepth));
        HandLandmarkToWorld.Instance.UpdateFromMediaPipe(
            isLeft ? worldPos : HandLandmarkToWorld.Instance.leftHandWorldPos,
            !isLeft ? worldPos : HandLandmarkToWorld.Instance.rightHandWorldPos,
            isLeft ? active : HandLandmarkToWorld.Instance.leftHandDetected,
            !isLeft ? active : HandLandmarkToWorld.Instance.rightHandDetected
        );
    }
}
