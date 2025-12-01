using UnityEngine;
using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;


public class enemyanimation : MonoBehaviour
{
    public float waddleAngle = 10f; // degrees to rotate left and right
    public float waddleFrequency = 5f;
    public float wiggleUpDown = 0.1f;
    public float wiggleLeftRight = 0.1f;
    public float wiggleFrequency1 = 5f;
    public float wiggleFrequency2 = 13f;
    private Vector3 startPosition;
    private Quaternion startRotation;
    public Vector3 normalscale = Vector3.one;
    public Vector3 enlargedscale = Vector3.one * 3f;
    public int enlargeframes = 50;
    public float intervalseconds = 1f;
    private Vector3 targetScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = normalscale;
        StartCoroutine(EnlargeRoutine());
        startRotation = transform.localRotation;
        startPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {

        // Calculate rotation angle using sine wave
        float angle = Mathf.Sin(Time.time * waddleFrequency) * waddleAngle;

        // Apply rotation around Y axis (or Z axis depending on model orientation)
        transform.localRotation = startRotation * Quaternion.Euler(0, 0, angle);

        float wiggleX = Mathf.Sin(Time.time * wiggleFrequency1) * wiggleUpDown;
        float wiggleZ = Mathf.Sin(Time.time * wiggleFrequency2) * wiggleLeftRight;
        transform.localPosition = startPosition + new Vector3(wiggleX, 0f, wiggleZ);
    }

    private IEnumerator EnlargeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(intervalseconds);

            // Smoothly enlarge over 0.2 seconds
            yield return ScaleOverTime(enlargedscale, 0.2f);

            // Stay enlarged for 10 frames
            for (int i = 0; i < enlargeframes; i++)
            {
                yield return null;
            }

            // Smoothly shrink back over 0.2 seconds
            yield return ScaleOverTime(normalscale, 0.2f);
        }
    }

    private IEnumerator ScaleOverTime(Vector3 targetscale, float duration)
    {
        Vector3 initialscale = transform.localScale;
        float timer = 0f;

        while (timer < duration)
        {
            transform.localScale = Vector3.Lerp(initialscale, targetscale*2, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetscale*2;
    }
}
