using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class PlayerController : MonoBehaviour {

    Vector3 velocity;
    Rigidbody playerRigidbody;

    void Start () {
        playerRigidbody = GetComponent<Rigidbody> ();
    }

    public void Move (Vector3 _velocity) {
        velocity = _velocity;
    }

    public void LookAt (Vector3 lookPoint) {
        Vector3 heightCorrectedPoint = new Vector3 (lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt (heightCorrectedPoint);
    }

    void FixedUpdate () {
        playerRigidbody.MovePosition (playerRigidbody.position + velocity * Time.fixedDeltaTime);
    }
}