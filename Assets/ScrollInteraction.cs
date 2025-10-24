using UnityEngine;
using TMPro;

public class ScrollInteraction : MonoBehaviour
{
    [Header("Player")]
    public MonoBehaviour playerMovementScript; // drag your movement script here

    [Header("UI Elements")]
    public GameObject inputPanel;
    public TextMeshProUGUI inputText;
    public TextMeshPro scrollText;
    public TextMeshProUGUI promptText;

    [Header("Settings")]
    public float interactDistance = 3f;
    public string interactKey = "e";
    public TMP_FontAsset cursiveFont;
    public int maxCharacters = 18;

    private bool isWriting = false;
    private bool isNearby = false;
    private Transform player;

    private string finalText = ""; // stores saved text
    private bool skipNextInput = false;

    void Start()
    {
        if (scrollText && cursiveFont)
            scrollText.font = cursiveFont;

        inputPanel.SetActive(false);
        promptText.gameObject.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        isNearby = dist < interactDistance;

        // Show or hide prompt
        if (isNearby && !isWriting)
            promptText.gameObject.SetActive(true);
        else
            promptText.gameObject.SetActive(false);

        // Start writing
        if (isNearby && Input.GetKeyDown(interactKey) && !isWriting)
            StartWriting();

        // Handle typing while writing
        if (isWriting)
        {
            HandleTyping();

            // Finish writing on Enter
            if (Input.GetKeyDown(KeyCode.Return))
                FinishWriting();
        }
    }

    void StartWriting()
    {
        isWriting = true;
        promptText.gameObject.SetActive(false);
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        // Load previous text
        scrollText.text = finalText;

        // Skip first keypress (the interact key)
        skipNextInput = true;
    }

    void FinishWriting()
    {
        isWriting = false;
        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        // Save typed text
        finalText = scrollText.text;
    }

    void HandleTyping()
    {
        // Ignore first input frame
        if (skipNextInput)
        {
            skipNextInput = false;
            return;
        }

        foreach (char c in Input.inputString)
        {
            if (c == '\b') // backspace
            {
                if (scrollText.text.Length > 0)
                    scrollText.text = scrollText.text.Substring(0, scrollText.text.Length - 1);
            }
            else if (scrollText.text.Length < maxCharacters)
            {
                scrollText.text += c;
            }
        }

        // Handle space manually
        if (Input.GetKeyDown(KeyCode.Space) && scrollText.text.Length < maxCharacters)
            scrollText.text += " ";
    }
}
