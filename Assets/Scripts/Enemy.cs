using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof (NavMeshAgent))]
public class Enemy : LivingEntity {

    public enum State { Idle, Chasing, Attacking }
    State currentState;

    public ParticleSystem deathEffect;

    NavMeshAgent pathfinder;
    LivingEntity targetEntity;
    Transform target;
    Material skinMaterial;

    Color originalColor;

    float attackDistanceThreshold = 0.5f;
    float timeBetweenAttacks = 1;
    float damage = 1;

    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;

    protected override void Start () {
        base.Start ();
        pathfinder = GetComponent<NavMeshAgent> ();
        skinMaterial = GetComponent<Renderer> ().material;
        originalColor = skinMaterial.color;

        if (GameObject.FindGameObjectWithTag ("Player") != null) {
            currentState = State.Chasing;
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag ("Player").transform;
            targetEntity = target.GetComponent<LivingEntity> ();
            targetEntity.OnDeath += OnTargetDeath;

            myCollisionRadius = GetComponent<CapsuleCollider> ().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider> ().radius;

            StartCoroutine (UpdatePath ());
        }
    }

    void Update () {
        if (hasTarget && Time.time > nextAttackTime) {
            float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
            float attackDistanceThresholdTmp = attackDistanceThreshold + myCollisionRadius + targetCollisionRadius;
            if (sqrDstToTarget < attackDistanceThresholdTmp * attackDistanceThresholdTmp) {
                nextAttackTime = Time.time + timeBetweenAttacks;
                StartCoroutine (Attack ());
            }
        }
    }

    public override void TakeHit (float damage, Vector3 hitPoint, Vector3 hitDirection) {
        if (damage >= health) {
            Destroy (Instantiate (deathEffect, hitPoint, Quaternion.FromToRotation (Vector3.forward, hitDirection)) as ParticleSystem, deathEffect.main.startLifetime.constant);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }

    void OnTargetDeath () {
        hasTarget = false;
        currentState = State.Idle;
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
        bool hasAppliedDamage = false;

        while (percent <= 1) {

            if (percent >= 0.5f && !hasAppliedDamage) {
                hasAppliedDamage = true;
                targetEntity.TakeDamage (damage);
            }

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

        while (hasTarget) {
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