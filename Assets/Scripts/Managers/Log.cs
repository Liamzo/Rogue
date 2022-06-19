using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Log : MonoBehaviour
{
    public float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > 4f) {
            //Logger.instance.completedLogs.Dequeue();
            Destroy(this.gameObject);
        }
    }
}
