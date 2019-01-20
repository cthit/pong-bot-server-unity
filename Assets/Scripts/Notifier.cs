using System.Collections.Generic;

using TMPro;

using UnityEngine;

public class Notifier : MonoBehaviour
{
    public GameObject notificationPrefab;

    struct Notification
    {
        public float time;
        public GameObject view;

        public Notification(string message, float time)
        {
            this.time = time;
            view = (GameObject)Instantiate(notifier.notificationPrefab);
            view.transform.SetParent(notifier.transform);
            view.GetComponentInChildren<TextMeshProUGUI>().text = message;
            view.GetComponent<RectTransform>().anchoredPosition = new Vector2(120, -Screen.height);
        }

        public Notification(GameObject view, float time)
        {
            this.view = view;
            this.time = time;
        }
    }

    List<Notification> notifications = new List<Notification>();

    static Notifier notifier = null;
    void OnEnable()
    {
        if (notifier == null)
            notifier = this;
        else
        {
            Debug.LogError("Please only have on instance of a Notifier present at any one time.");
            DestroyImmediate(this);
        }
    }

    void OnDestroy()
    {
        if (notifier == this)
            notifier = null;
    }

    void Update()
    {
        for (int i = 0; i < notifications.Count; i++)
        {
            if (notifications[i].time <= 0)
            {
                Destroy(notifications[i].view);
                notifications.RemoveAt(i);
            }
            else
            {
                notifications[i] = new Notification(notifications[i].view, notifications[i].time - Time.deltaTime); // ARRHHGHGH
                Vector2 targetPosition = new Vector2(notifications[i].time <= 1f ? -400 : (120 + ((i % 2) == 0 ? 10 : -10)), -100 - i * 60);
                RectTransform rt = notifications[i].view.GetComponent<RectTransform>();
                rt.anchoredPosition += (targetPosition - rt.anchoredPosition) * Time.deltaTime;
            }
        }
    }

    public static void Notify(string message, float time)
    {
        notifier.notifications.Add(new Notification(message, time));
    }
}
