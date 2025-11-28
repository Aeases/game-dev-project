using UnityEngine;

public class bullet : MonoBehaviour
{
    private Rigidbody rb;
    public GameObject player;
    public bool isFriendly;
    public float speed = 5;
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
