using System;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
     
    [SerializeField] public Wave[] waves;

    void Start()
    {

        for (int i = 0; i < waves[0].enemies.Length; i++)
        {
            Instantiate(waves[0].enemies[i], transform.position, transform.rotation);
            
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(waves[0].enemies.Length);


        
    }


}

[System.Serializable]
public class Wave
{
    public Enemy[] enemies;
}
