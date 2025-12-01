
using UnityEngine;
using static Unity.VisualScripting.Dependencies.Sqlite.SQLite3;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Boss : Enemy
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void Start()
        {
            base.Start(); // This sets health to max health, and loads initial element bullet
            StartCoroutine(bossShoot2());
        }

        // Update is called once per frame
        void Update()
        {

        }
    }


