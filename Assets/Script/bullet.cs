using UnityEngine;

public class bullet : MonoBehaviour
{
    private Rigidbody rb;
    public GameObject player;
    public float baseDamage;
    public elementType bulletElement;
    public bool isFriendly;
    public float speed = 2;
    public float existTime = 10f; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void Update()
    {

    }
    
}
