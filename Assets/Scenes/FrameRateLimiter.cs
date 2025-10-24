using UnityEngine;

public class FrameRateLimiter : MonoBehaviour
{
    [Header("Frame Rate Settings")]
    [Tooltip("Target frame rate for the game.")]
    public int targetFrameRate = 300;

    [Header("FPS Counter Settings")]
    public bool showFPS = true;
    public int fontSize = 12; // Small text size
    public Color textColor = Color.white;
    public Vector2 offset = new Vector2(10, 10); // Distance from top-right corner

    private float deltaTime = 0f;

    void Awake()
    {
        // Keep this object between scene loads (optional)
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        QualitySettings.vSyncCount = 0; // Disable VSync so FPS cap works
        Application.targetFrameRate = targetFrameRate;
        Debug.Log($"Frame rate limited to {targetFrameRate} FPS");
    }

    void Update()
    {
        // Smooth FPS calculation
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        if (!showFPS) return;

        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();
        Rect rect = new Rect(w - 100 - offset.x, offset.y, 100, h * 2 / 100);
        style.alignment = TextAnchor.UpperRight;
        style.fontSize = fontSize;
        style.normal.textColor = textColor;

        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = $"{fps:0.} FPS";

        GUI.Label(rect, text, style);
    }
}