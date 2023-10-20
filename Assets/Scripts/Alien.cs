using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Alien : MonoBehaviour
{
    public UnityEvent OnDestroy;
    public Transform target;
    private NavMeshAgent agent;
    public float navigationUpdate;
    private float navigationTime = 0;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (target != null)
        {
            navigationTime += Time.deltaTime;
            if (navigationTime > navigationUpdate)
            {
                agent.destination = target.position;
                navigationTime = 0;
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        Die();
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.alienDeath);
    }
    public void Die()
    {
        OnDestroy.Invoke();
        OnDestroy.RemoveAllListeners();
        Destroy(gameObject);
    }
}
