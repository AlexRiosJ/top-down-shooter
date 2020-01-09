using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class PlayerController : MonoBehaviour {

    Vector3 velocity;
    Rigidbody rigidbody;

    void Start () {
        rigidbody = GetComponent<Rigidbody> ();
    }

    public void Move (Vector3 _velocity) {
        velocity = _velocity;
    }

    public void LookAt (Vector3 lookPoint) {
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt (heightCorrectedPoint);
    }

    void FixedUpdate () {
        rigidbody.MovePosition (rigidbody.position + velocity * Time.fixedDeltaTime);
    }
}