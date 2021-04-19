using UnityEngine;

public class Station : MonoBehaviour
{
    [SerializeField] private CameraController controller;
    [SerializeField] [TagSelector] private string playerTag;
    [SerializeField] [TagSelector] private string stationTag;

    public void CheckCollision(Collision2D collision)
    {
        if (collision.collider.CompareTag(playerTag))
            controller.SwitchTarget(stationTag);
    }
}
