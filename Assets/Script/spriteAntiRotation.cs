using UnityEngine;



public class SpriteAntiRotation : MonoBehaviour
{
    private GameObject parentObject;
    private Quaternion originalRotation;

    void Start()
    {
        parentObject = this.transform.parent.gameObject;
        originalRotation = this.transform.rotation;
    }


    void LateUpdate()
    {
        if (parentObject.transform != null)
        {
            transform.rotation = originalRotation;
        }
    }
}