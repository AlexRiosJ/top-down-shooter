using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public LayerMask collisionMask;
    float speed = 10;
    float damage = 1;

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

        if (Physics.Raycast (ray, out hit, moveDistance, collisionMask, QueryTriggerInteraction.Collide)) {
            OnHitObject (hit);
        }
    }

    void OnHitObject (RaycastHit hit) {
        IDamagable damageObject = hit.collider.GetComponent<IDamagable> ();
        if(damageObject != null) {
            damageObject.TakeHit(damage, hit);
        }
        GameObject.Destroy (gameObject);
    }
}