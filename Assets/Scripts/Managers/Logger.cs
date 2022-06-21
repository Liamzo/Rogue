using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Logger : MonoBehaviour
{
    public static Logger instance;

    public float timer = 0f;
    //public float deleteTimer = 0f;

    public Queue<string> pendingLogs;
    public Queue<GameObject> completedLogs;

    public GameObject logPrefab;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        pendingLogs = new Queue<string>();
        completedLogs = new Queue<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        // Increment timers
        timer -= Time.deltaTime;
        // if (completedLogs.Count > 0) {
        //     deleteTimer += Time.deltaTime;

        //     if (deleteTimer > 4f) {
        //         GameObject log = completedLogs.Dequeue();
        //         Destroy(log);
        //         deleteTimer = 0f;
        //     }
        // } else {
        //     deleteTimer = 0f;
        // }

        // Check if more logs can be displayed
        if (pendingLogs.Count > 0 && timer <= 0f) {
            GameObject log = Instantiate(logPrefab, this.transform);
            log.GetComponent<TextMeshProUGUI>().text = pendingLogs.Dequeue();
            completedLogs.Enqueue(log);
            timer = 0.25f;
        }

        // Clear logs if too many
        if (completedLogs.Count > 5) {
            GameObject log = completedLogs.Dequeue();
            Destroy(log);
        }
    }

    public void AddLog(string logMessage) {
        pendingLogs.Enqueue(logMessage);
    }
}
