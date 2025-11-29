using UnityEngine;

public class OpenShop : MonoBehaviour
{
    public GameObject shopUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            shopUI.gameObject.SetActive(true);
        }   
    }
}
