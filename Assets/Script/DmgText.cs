using UnityEngine;
using TMPro;                  // ← This line is enough!
using System.Collections;

public class DmgText : MonoBehaviour
{
    private TextMeshPro textMesh;     // ← THIS IS THE CORRECT TYPE (not TextMeshProTMP)
    public float floatSpeed = 2f;
    public float fadeSpeed = 2f;
    public float lifetime = 1.5f;

    void Awake()  // ← Use Awake instead of Start (safer)
    {
        if (textMesh == null)
            textMesh = GetComponent<TextMeshPro>();

        if (textMesh == null)
            Debug.LogError("TextMeshPro component missing on " + gameObject.name);

        StartCoroutine(Animate());
    }   

    IEnumerator Animate()
    {
        float timer = 0;
        Color startColor = textMesh.color;

        while (timer < lifetime)
        {
            transform.position += Vector3.up * floatSpeed * Time.deltaTime;

            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / lifetime);
            textMesh.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        Destroy(gameObject);
    }

    public void SetDamage(int damage)
    {
        textMesh.text = "-" + damage;
    }
}