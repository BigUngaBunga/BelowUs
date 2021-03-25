using UnityEngine;
using UnityEngine.Events;

public class HotkeyListener : MonoBehaviour
{
    [SerializeField] private KeyCode pauseHotkey;
    [SerializeField] private UnityEvent Response;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(pauseHotkey))
             Response.Invoke();
    }
}
