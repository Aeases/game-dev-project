using TMPro;
using UnityEngine;

public class Animation : MonoBehaviour
{
    private Vector3 originalPos;
    public float floatHeight = 10f;  // Pixels up/down
    public float speed = 2f;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    void Update()
    {
        float bob = Mathf.Sin(Time.time * speed) * floatHeight;
        transform.localPosition = originalPos + Vector3.up * bob;
    }
}
