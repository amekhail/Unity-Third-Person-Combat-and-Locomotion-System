using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCivilian : EntityContext
{
    [Header("Patrol Settings")]
    public Transform[] waypoints;
    public bool useRandomPath = false;
    public bool loop = true;

    private int currentIndex = 0;
    private bool isWaiting = false;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (waypoints.Length == 0)
        {
            Debug.LogWarning($"[{name}] has no patrol points!");
            return;
        }

        GoToNextPoint();
    }
    
    private void Update()
    {
        if (isWaiting || waypoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(IdleThenMove());
        }
        Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);
        float forward = localVelocity.z / agent.speed;
        float strafe = localVelocity.x / agent.speed;

        animator.SetFloat("Vertical", forward, 0.2f, Time.deltaTime);
        animator.SetFloat("Horizontal", strafe, 0.2f, Time.deltaTime);
    }
    
    private IEnumerator IdleThenMove()
    {
        isWaiting = true;
        agent.isStopped = true;
        animator.SetFloat("Speed", 0f);
        animator.SetFloat("Vertical", 0f);
        animator.SetFloat("Horizontal", 0f);

        yield return new WaitForSeconds(idleTime);

        GoToNextPoint();
        isWaiting = false;
    }
    
    private void GoToNextPoint()
    {
        agent.isStopped = false;

        if (useRandomPath)
        {
            currentIndex = Random.Range(0, waypoints.Length);
        }
        else
        {
            currentIndex = (currentIndex + 1) % waypoints.Length;
            if (!loop && currentIndex == 0)
            {
                agent.isStopped = true;
                return;
            }
        }

        agent.speed = patrolSpeed;
        agent.SetDestination(waypoints[currentIndex].position);
    }
    
}
