using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPCMovement : MonoBehaviour {

    Rigidbody abeRigidBody;
    public float walkSpeed = 5f;

    void Start() {
        abeRigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
    }

    void Update() {
        StartWalking();
        CheckRayCollision();
    }



    void StartWalking() {
        transform.Translate(Vector3.forward * (Time.deltaTime * 1.4f));


    }

    void CheckRayCollision() {

        Vector3 currentRotation = this.transform.localEulerAngles;
        Quaternion currentRotationQua = Quaternion.Euler(currentRotation);

        float newRotation = Random.Range(-110, 110);
        Quaternion newRotationQua = Quaternion.Euler(0, newRotation, 0);

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.SphereCast(ray, 1f, out hit)) {
            if (hit.distance < 10) {
                transform.rotation = Quaternion.Lerp(currentRotationQua, newRotationQua, Time.deltaTime * 15f);
            }
        }
    }
}