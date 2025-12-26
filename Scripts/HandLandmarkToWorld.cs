using UnityEngine;

public class HandLandmarkToWorld : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject leftHandCursor;
    public GameObject rightHandCursor;
    public float handDepth = 5f;
    public bool showCursors = true;
    
    [Header("Output")]
    public Vector3 leftHandWorldPos;
    public Vector3 rightHandWorldPos;
    public bool leftHandDetected;
    public bool rightHandDetected;
    
    public static HandLandmarkToWorld Instance { get; private set; }
    
    void Awake() { Instance = this; }

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (leftHandCursor == null || rightHandCursor == null) CreateCursors();
    }

    void Update()
    {
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            UpdateHandPositionsFromMouse();
        }
        
        UpdateCursors();
    }

    void UpdateHandPositionsFromMouse()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            leftHandWorldPos = ray.origin + ray.direction * handDepth;
            leftHandDetected = true;
        }
        if (Input.GetMouseButton(1))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            rightHandWorldPos = ray.origin + ray.direction * handDepth;
            rightHandDetected = true;
        }
    }

    public void UpdateFromMediaPipe(Vector3 lPos, Vector3 rPos, bool lActive, bool rActive)
    {
        if (!Input.GetMouseButton(0))
        {
            leftHandWorldPos = lPos;
            leftHandDetected = lActive;
        }
        if (!Input.GetMouseButton(1))
        {
            rightHandWorldPos = rPos;
            rightHandDetected = rActive;
        }
    }

    void UpdateCursors()
    {
        if (leftHandCursor != null) {
            leftHandCursor.SetActive(leftHandDetected);
            leftHandCursor.transform.position = leftHandWorldPos;
        }
        if (rightHandCursor != null) {
            rightHandCursor.SetActive(rightHandDetected);
            rightHandCursor.transform.position = rightHandWorldPos;
        }
    }

    void CreateCursors()
    {
        leftHandCursor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leftHandCursor.name = "LeftCursor"; leftHandCursor.transform.localScale = Vector3.one * 0.2f;
        Destroy(leftHandCursor.GetComponent<Collider>());
        
        rightHandCursor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rightHandCursor.name = "RightCursor"; rightHandCursor.transform.localScale = Vector3.one * 0.2f;
        Destroy(rightHandCursor.GetComponent<Collider>());
    }

    void OnGUI()
    {
        GUI.color = Color.yellow;
        GUI.Label(new Rect(10, 10, 300, 20), $"Left Hand: {(leftHandDetected ? "✓" : "X")}");
        GUI.Label(new Rect(10, 30, 300, 20), "MOUSE MOD: Click = Force Hand");
    }
}
