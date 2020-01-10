using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof (NavMeshAgent))]
public class Enemy : LivingEntity {

    public enum State { Idle, Chasing, Attacking }
    State currentState;

    NavMeshAgent pathfinder;
    Transform target;
    Material skinMaterial;

    Color originalColor;

    float attackDistanceThreshold = 0.5f;
    float timeBetweenAttacks = 1;

    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;

    protected override void Start () {
        base.Start ();
        pathfinder = GetComponent<NavMeshAgent> ();
        skinMaterial = GetComponent<Renderer> ().material;
        originalColor = skinMaterial.color;
        currentState = State.Chasing;
        target = GameObject.FindGameObjectWithTag ("Player").transform;
        myCollisionRadius = GetComponent<CapsuleCollider> ().radius;
        targetCollisionRadius = target.GetComponent<CapsuleCollider> ().radius;
        StartCoroutine (UpdatePath ());
    }

    void Update () {
        if (Time.time > nextAttackTime) {
            float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
            float attackDistanceThresholdTmp = attackDistanceThreshold + myCollisionRadius + targetCollisionRadius;
            if (sqrDstToTarget < attackDistanceThresholdTmp * attackDistanceThresholdTmp) {
                nextAttackTime = Time.time + timeBetweenAttacks;
                StartCoroutine (Attack ());
            }
        }
    }

    IEnumerator Attack () {
        currentState = State.Attacking;
        pathfinder.enabled = false;

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);

        float attackSpeed = 3;
        float percent = 0;

        skinMaterial.color = Color.red;

        while (percent <= 1) {
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-percent * percent + percent) * 4;
            transform.position = Vector3.Lerp (originalPosition, attackPosition, interpolation);
            yield return null;
        }

        skinMaterial.color = originalColor;
        currentState = State.Chasing;
        pathfinder.enabled = true;
    }

    IEnumerator UpdatePath () {
        float refreshRate = 0.25f;

        while (target != null) {
            if (currentState == State.Chasing) {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2f);
                if (!dead) {
                    pathfinder.SetDestination (targetPosition);
                }
            }
            yield return new WaitForSeconds (refreshRate);
        }
    }
}