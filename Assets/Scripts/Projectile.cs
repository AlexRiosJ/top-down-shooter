﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    float speed = 10;

    public void SetSpeed(float newSpeed) {
        speed = newSpeed;
    }

    void Update() {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }
}
