using UnityEngine;
using System.Collections.Generic;

public enum JointType
{
    LeftWrist,
    RightWrist,
    LeftIndexTip,
    RightIndexTip,
    LeftAnkle,
    RightAnkle
}

public class SkeletonTracker : MonoBehaviour
{
    public GameObject fingerTipObject;
    public Camera mainCamera;
    public bool showDebugLines = true;

    private Dictionary<JointType, Vector3> trackedJoints = new Dictionary<JointType, Vector3>();

    public void UpdateJointData(JointType joint, float x, float y)
    {
        Vector3 screenPos = new Vector3(x * Screen.width, y * Screen.height, mainCamera.nearClipPlane + 5f);
        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            trackedJoints[joint] = hit.point;

            if (joint == JointType.RightIndexTip && fingerTipObject != null)
            {
                fingerTipObject.transform.position = hit.point;
            }
        }
    }

    void Update()
    {
        if (showDebugLines)
        {
            foreach (var joint in trackedJoints.Values)
            {
                Debug.DrawLine(mainCamera.transform.position, joint, Color.green);
            }
        }
    }
}
