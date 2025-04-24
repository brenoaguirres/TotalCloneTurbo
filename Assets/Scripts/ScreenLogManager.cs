using UnityEngine;
using System.Collections.Generic;

public class ScreenLogManager : MonoBehaviour
{
    public static ScreenLogManager Instance { get; private set; }
    public bool debugMode = true;

    private class LogEntry
    {
        public string Message;
        public float TimeToLive;

        public LogEntry(string message, float timeToLive)
        {
            Message = message;
            TimeToLive = timeToLive;
        }
    }

    private List<LogEntry> logMessages = new List<LogEntry>();
    private const float messageDuration = 1f;
    private const int maxMessages = 20;
    private const int fontSize = 18;
    private readonly Color fontColor = Color.red;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LogMessage(string message)
    {
        if (debugMode)
        {
            if (logMessages.Count >= maxMessages)
            {
                logMessages.RemoveAt(0); // Remove the oldest message
            }
            logMessages.Add(new LogEntry(message, Time.time + messageDuration));
        }
    }

    private void Update()
    {
        if (debugMode)
        {
            // Remove expired messages
            logMessages.RemoveAll(entry => Time.time > entry.TimeToLive);
        }
    }

    private void OnGUI()
    {
        if (debugMode)
        {
            GUIStyle style = new GUIStyle
            {
                normal = { textColor = fontColor },
                fontSize = fontSize
            };

            float yOffset = 10f;
            foreach (var logEntry in logMessages)
            {
                GUI.Label(new Rect(10, yOffset, 500, 20), logEntry.Message, style);
                yOffset += 20f; // Move to the next line
            }
        }
    }
}
