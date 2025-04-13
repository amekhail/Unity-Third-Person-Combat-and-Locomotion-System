using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EntityContext : MonoBehaviour
{
    [Header("References")] 
    public Animator animator;
    public NavMeshAgent agent;
    public Transform playerTarget;

    [Header("General Settings")] 
    public float idleTime = 2f;
    public float patrolSpeed = 2f;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }
}
