using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Character : MonoBehaviour
{

    private CharacterController characterController;
    public float Speed = 1f;
    public float dashSpeed = 1f;
    public float dashTime = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        characterController.Move(move * Time.deltaTime*Speed);

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(Dash());
        }
    }

IEnumerator Dash()
{
    float startTime = Time.time;
    
    // Always dash in the direction the character is facing
    Vector3 dashDirection = transform.forward;
    
    while (Time.time < startTime + dashTime)
    {
        characterController.Move(dashDirection * dashSpeed * Time.deltaTime);
        yield return null;
    }
}
}
