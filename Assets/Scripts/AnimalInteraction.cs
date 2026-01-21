using UnityEngine;

public class AnimalInteraction : MonoBehaviour
{
    [Header("Animal Info")]
    public string animalName = "Animal";
    
    [Header("Interaction")]
    public float interactionRadius = 0.6f;
    public Color normalColor = Color.white;
    public Color touchColor = Color.green;
    public float scaleMultiplier = 1.3f;
    
    [Header("Flying (Butterfly)")]
    public bool canFly = false;
    public float flySpeed = 2f;
    public Vector3 flyAreaMin = new Vector3(-5, 1, 2);
    public Vector3 flyAreaMax = new Vector3(5, 3, 8);
    
    [Header("Audio")]
    public AudioClip touchSound;
    
    private Vector3 originalScale;
    private Vector3 flyTarget;
    private Renderer[] renderers;
    private Material[] materials;
    private Color[] originalColors;
    private bool isTouched = false;
    private AudioSource audioSource;
    
    void Start()
    {
        originalScale = transform.localScale;
        
        renderers = GetComponentsInChildren<Renderer>();
        materials = new Material[renderers.Length];
        originalColors = new Color[renderers.Length];
        
        for (int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].material;
            originalColors[i] = materials[i].color;
        }
        
        if (touchSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.clip = touchSound;
            audioSource.spatialBlend = 1f;
        }
        
        if (canFly)
        {
            SetNewFlyTarget();
        }
        
        Debug.Log($"{animalName} is ready! (Interaction Radius: {interactionRadius})");
    }
    
    void Update()
    {
        if (canFly)
        {
            Fly();
        }
        
        CheckHandInteraction();
        UpdateVisuals();
    }
    
    void Fly()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            flyTarget,
            flySpeed * Time.deltaTime
        );
        
        if (Vector3.Distance(transform.position, flyTarget) < 0.5f)
        {
            SetNewFlyTarget();
        }
        
        Vector3 dir = (flyTarget - transform.position).normalized;
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                Time.deltaTime * 3f
            );
        }
    }
    
    void SetNewFlyTarget()
    {
        flyTarget = new Vector3(
            Random.Range(flyAreaMin.x, flyAreaMax.x),
            Random.Range(flyAreaMin.y, flyAreaMax.y),
            Random.Range(flyAreaMin.z, flyAreaMax.z)
        );
    }
    
    void CheckHandInteraction()
    {
        if (HandLandmarkToWorld.Instance == null) return;
        
        bool touched = false;
        
        if (HandLandmarkToWorld.Instance.leftHandDetected)
        {
            float leftDist = Vector3.Distance(
                transform.position,
                HandLandmarkToWorld.Instance.leftHandWorldPos
            );
            
            if (leftDist < interactionRadius)
            {
                touched = true;
            }
        }
        
        if (HandLandmarkToWorld.Instance.rightHandDetected)
        {
            float rightDist = Vector3.Distance(
                transform.position,
                HandLandmarkToWorld.Instance.rightHandWorldPos
            );
            
            if (rightDist < interactionRadius)
            {
                touched = true;
            }
        }
        
        if (touched && !isTouched)
        {
            OnTouchStart();
        }
        else if (!touched && isTouched)
        {
            OnTouchEnd();
        }
    }

    void OnTouchStart()
    {
        isTouched = true;
    
        Debug.Log($"{animalName} has been touched");
    
        if (canFly && (animalName.Contains("Butterfly") || animalName.Contains("Fly")))
        {
            CatchButterfly();
            return;
        }
    
        if (audioSource == null)
        {
            Debug.LogWarning($"{animalName}:Creating AudioSource ...");
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }
    
        AudioListener listener = FindObjectOfType<AudioListener>();
        if (listener == null)
        {
            Debug.LogError("NO AUDIO LISTENER! Adding to Main Camera...");
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                mainCam.gameObject.AddComponent<AudioListener>();
                Debug.Log("Audio Listener added to Main Camera");
            }
        }
    
        if (audioSource != null && touchSound != null)
        {
            Debug.Log($"Playing audio: {touchSound.name}");
            audioSource.volume = 1.0f;
            audioSource.PlayOneShot(touchSound);
        }
        else if (audioSource != null)
        {
            Debug.LogWarning($"{animalName}: No touchSound. Playing default BEEP");
            PlayDefaultSound();
        }
    }

    void CatchButterfly()
    {
        Debug.Log($"{animalName} Catched! Added to pocket...");
    
    
        StartCoroutine(ButterflyDisappear());
    }

    System.Collections.IEnumerator ButterflyDisappear()
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 startPos = transform.position;
    
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
        
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
        
            transform.position = startPos + Vector3.up * t * 2f;
        
            transform.Rotate(Vector3.up, Time.deltaTime * 720f);
        
            yield return null;
        }
    
        Debug.Log($"🦋 {animalName} disapperad!");
    
        yield return new WaitForSeconds(3f);
    
        transform.localScale = startScale;
        SetNewFlyTarget();
        transform.position = flyTarget;
    
        Debug.Log($"{animalName} has apperad!");
    }
    
    void PlayDefaultSound()
    {
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.volume = 0.5f;
        
        // Hayvan tipine göre farklı pitch
        if (animalName.Contains("Kelebek"))
        {
            audioSource.pitch = 1.5f; // Yüksek ses
        }
        else if (animalName.Contains("Kedi"))
        {
            audioSource.pitch = 1.0f; // Orta ses
        }
        else if (animalName.Contains("İnek"))
        {
            audioSource.pitch = 0.6f; // Alçak ses
        }
        
        //default beep
        audioSource.PlayOneShot(AudioClip.Create("beep", 4410, 1, 44100, false));
    }
    
    void OnTouchEnd()
    {
        isTouched = false;
        Debug.Log($"{animalName} has released");
    }
    
    void UpdateVisuals()
    {
        Color targetColor = isTouched ? touchColor : normalColor;
        
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = Color.Lerp(
                materials[i].color,
                targetColor,
                Time.deltaTime * 10f
            );
        }
        
        Vector3 targetScale = isTouched ? originalScale * scaleMultiplier : originalScale;
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * 10f
        );
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = isTouched ? Color.green : new Color(1, 1, 0, 0.5f);
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
