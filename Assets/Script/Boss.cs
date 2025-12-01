
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Boss : Enemy
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        protected override void Attack()
        {
            Vector3 targetPos = _player.position;
            targetPos.y = transform.position.y; // lock y-axis 
            transform.LookAt(targetPos);

            if (!alreadyAttacked)
            {

                shoot();
                StartCoroutine(bossShoot2());
                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), attackDelay);
            }
        }
    }


