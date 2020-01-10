using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public LayerMask collisionMask;
    float speed = 10;
    float damage = 1;

    float lifetime = 3;
    float skinWidth = 0.1f;

    void Start () {
        Destroy (gameObject, lifetime);

        Collider[] initialCollisions = Physics.OverlapSphere (transform.position, 0.1f, collisionMask);
        if (initialCollisions.Length > 0) {
            OnHitObject(initialCollisions[0]);
        }
    }

    public void SetSpeed (float newSpeed) {
        speed = newSpeed;
    }

    void Update () {
        float moveDistance = speed * Time.deltaTime;
        CheckCollitions (moveDistance);
        transform.Translate (Vector3.forward * moveDistance);
    }

    void CheckCollitions (float moveDistance) {
        Ray ray = new Ray (transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast (ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide)) {
            OnHitObject (hit);
        }
    }

    void OnHitObject (RaycastHit hit) {
        IDamagable damageObject = hit.collider.GetComponent<IDamagable> ();
        if (damageObject != null) {
            damageObject.TakeHit (damage, hit);
        }
        GameObject.Destroy (gameObject);
    }

    void OnHitObject(Collider collider) {
        IDamagable damagableObject = collider.GetComponent<IDamagable> ();
        if (damagableObject != null) {
            damagableObject.TakeDamage (damage);
        }
        GameObject.Destroy (gameObject);
    }
}