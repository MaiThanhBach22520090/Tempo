using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float health = 10000f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float acceleration = 1f;

    [Header("References for AI Movement")]
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private float timeBetweenWaypoints = 3f;

    private float currentSpeed = 0f;
    private int currentWaypointIndex = 0;
    private bool isMoving = false;

    private void Start()
    {
        if (waypoints.Count == 0)
        {
            Debug.LogError("No waypoints assigned to Enemy.");
            return;
        }

        currentWaypointIndex = 0;
        transform.position = waypoints[currentWaypointIndex].position;
        StartCoroutine(MoveToNextWaypoint());
    }

    private void Update()
    {
        if (isMoving)
        {
            Move();
        }
    }

    private void Move()
    {
        // Move towards the next waypoint
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector2 direction = (targetWaypoint.position - transform.position).normalized;
        currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);
        transform.position += (Vector3)direction * currentSpeed * Time.deltaTime;

        // Check if the enemy has reached the waypoint
        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentSpeed = 0f;
            isMoving = false;
            StartCoroutine(MoveToNextWaypoint());
        }
    }

    private IEnumerator MoveToNextWaypoint()
    {
        // Wait for the specified time before moving to the next waypoint
        yield return new WaitForSeconds(timeBetweenWaypoints);

        // Choose the next waypoint randomly but not the same as the current one
        int nextWaypointIndex = Random.Range(0, waypoints.Count);
        while (nextWaypointIndex == currentWaypointIndex)
        {
            nextWaypointIndex = Random.Range(0, waypoints.Count);
        }

        currentWaypointIndex = nextWaypointIndex;
        isMoving = true;
    }
}
