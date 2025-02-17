using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _193645
{
    public class WaypointFollower : MonoBehaviour
    {
        [SerializeField] private GameObject[] waypoints;
        [SerializeField] private float speed = 1.0f;
        private int currentWaypont = 0;

        void Update()
        {
            if (Vector2.Distance(this.transform.position, waypoints[currentWaypont].transform.position) < 0.1f)
            {
                currentWaypont = ++currentWaypont % waypoints.Length;
            }

            this.transform.position = Vector2.MoveTowards(this.transform.position, waypoints[currentWaypont].transform.position, speed * Time.deltaTime);
        }
    }
}