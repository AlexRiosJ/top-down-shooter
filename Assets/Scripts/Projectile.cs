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
            OnHitObject (initialCollisions[0], transform.position);
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
            OnHitObject (hit.collider, hit.point);
        }
    }

    void OnHitObject (Collider collider, Vector3 hitPoint) {
        IDamagable damagableObject = collider.GetComponent<IDamagable> ();
        if (damagableObject != null) {
            damagableObject.TakeHit (damage, hitPoint, transform.forward);
        }
        GameObject.Destroy (gameObject);
    }
}