using Oculus.Interaction.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;



public class RightHandInteractor : MonoBehaviour {


    public Transform wristTransform;
    [HideInInspector]
    public Ray ray;
    public float hitObjectCacheTime = 0.2f;
    private float hitObjectCacheTimer = 0.0f;
    private GameObject lastHitObject = null;
    public GameObject controllingObject = null;

    private void SetColor(Color color, GameObject target) {
        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null) {
            renderer.material.color = color;
        }
    }

    public void TriggerNonGrabbing() {
        if (isControling) {
            SetColor(Color.white, currentHitObject);
            currentHitObject = null;
            isControling = false;
            rayCylinder.SetActive(true);
        }

    }

    public GameObject rayCylinder;
    // public GameObject debugSphere;
    public float rayLength = 2.0f;


    public Transform grabPointTransform;
    public Vector3 GrabPoint {
        get {
            return grabPointTransform.position;
        }
    }
    public bool isControling = false;

    public Vector3 handOffset;

    public GameObject visualHand;

    public Vector3 hitPointOffset;

    private Quaternion OriginalObjectRotation;
    private Quaternion OriginalHandRotation;

    [HideInInspector]
    public GameObject currentHitObject;
    [HideInInspector]
    public Vector3 hitPoint;





    //  Task 1. Construct the ray
    //  TODO: Implement the function to construct a ray using the wristTransform's position and orientation.
    public Ray ConstructRay(Transform wristTransform) {

        return new Ray(wristTransform.position, wristTransform.forward);

    }


    private System.Collections.IEnumerator ReportWristPosition() {
        while (true) {
            yield return new WaitForSeconds(1.0f);
            Debug.Log("Wrist Position: " + wristTransform.position.ToString());
        }
    }


    //  Task 2. Render the ray
    //  TODO: Implement the function to render the ray using a cylinder.
    public void RenderRay(Ray ray, float rayLength) {
        // Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.green);
        Vector3 rayDestination = ray.origin + ray.direction * rayLength;
        rayCylinder.transform.position = ray.origin + ray.direction * rayLength / 2;
        rayCylinder.transform.LookAt(rayDestination);
        rayCylinder.transform.localScale = new Vector3(0.05f, 0.05f, rayLength);
        // debugSphere.transform.position = ray.origin + ray.direction * rayLength;
        return;

    }


    //  Task 3. Intersection between the ray and a virtual object
    //  TODO: Check if there exists intersection between the ray and an Interactable virtual object.
    public bool CheckHit(Ray ray, float rayLength)
    {
        // Cast the ray and search for the closest collider tagged "Interactable"
        RaycastHit[] hits = Physics.RaycastAll(ray, rayLength);

        GameObject bestObj = null;
        Vector3 bestPoint = Vector3.zero;
        float closest = Mathf.Infinity;

        foreach (var h in hits)
        {
            GameObject go = h.collider.gameObject;
            if (!go.CompareTag("Interactable")) continue;

            if (h.distance < closest)
            {
                closest = h.distance;
                bestObj = go;
                bestPoint = h.point;
            }
        }

        if (bestObj != null)
        {
            currentHitObject = bestObj;   // remember which object we hit
            hitPoint = bestPoint;         // and the world-space intersection point
            SetColor(Color.green, currentHitObject); // visual feedback (Figure 7)
            return true;
        }
        return false;
    }

    //  Task 4. Trigger object selection by closing your right hand
    //  TODO: Implement the functionality that should be called when the hand is starting to grab (closing fist).
    public void TriggerGrabbing()
    {
        // Only proceed if we have a remembered hit target
        if (currentHitObject == null) return;

        // Update control state
        isControling = true;
        controllingObject = currentHitObject; // (useful for the left-hand scaler if it references this)

        // Visual feedback & hide the ray while manipulating
        SetColor(Color.red, currentHitObject);
        if (rayCylinder != null) rayCylinder.SetActive(false);

        // Record initial orientations
        OriginalHandRotation   = wristTransform.rotation;
        OriginalObjectRotation = currentHitObject.transform.rotation;
    }

    //  Task 5. Manipulate the object by using your right hand (with closed fist)
    //  TODO: Implement the function to calculate the position and orientation of the object being manipulated.
    public Pose UpdateObjectPose(Vector3 CurrentGrabPoint, Vector3 ObjectOffset, Quaternion OriginalObjectRotation, Quaternion OriginalHandRotation, Quaternion CurrentHandRotation) {
        
        // Hand rotation delta from grab start → current frame
        Quaternion handDelta = CurrentHandRotation * Quaternion.Inverse(OriginalHandRotation);

        // Rotate the object by the same delta
        Quaternion newRotation = handDelta * OriginalObjectRotation;

        // Rotate the original (grab→center) offset by the same delta so the contact point stays put
        Vector3 rotatedOffset = handDelta * ObjectOffset;

        // New object center = current grab point + rotated offset
        Vector3 newPosition = CurrentGrabPoint + rotatedOffset;

        return new Pose(newPosition, newRotation);
    }



    //  Task 6. Scaling
    //  Go to LeftHandInteractor.cs for implementing this task.
    public LeftHandInteractor leftHandInteractor;










    void Start() {
        SetColor(Color.blue, rayCylinder);
        // StartCoroutine(ReportWristPosition());
    }

    void Update() {
        if (isControling) {

            Vector3 currentGrabPoint = handOffset + GrabPoint;

            visualHand.transform.position = wristTransform.position + handOffset;
            visualHand.transform.rotation = wristTransform.rotation;
            Pose objPose = UpdateObjectPose(currentGrabPoint, hitPointOffset, OriginalObjectRotation, OriginalHandRotation, wristTransform.rotation);
            currentHitObject.transform.position = objPose.position;
            currentHitObject.transform.rotation = objPose.rotation;

        } else {
            visualHand.transform.position = wristTransform.position;
            visualHand.transform.rotation = wristTransform.rotation;
            ray = ConstructRay(wristTransform);
            RenderRay(ray, rayLength);

            if (CheckHit(ray, rayLength)) {
                // When the ray hits an object, reset the hit object cache timer and update the last hit object.
                hitObjectCacheTimer = hitObjectCacheTime;
                lastHitObject = currentHitObject;
                // Calculate the offset between the wrist and the grab point.
                handOffset = hitPoint - GrabPoint;
                // Calculate the offset between the hit point and the object's center.
                hitPointOffset = currentHitObject.transform.position - hitPoint;

            } else {
                // This part is to cache the last hit object for a short period of time to "remember" the object being pointed at.
                if (hitObjectCacheTimer > 0.0f) {
                    // The timer is still running, keep the last hit object as the current hit object.
                    hitObjectCacheTimer -= Time.deltaTime;
                    currentHitObject = lastHitObject;

                } else {
                    // The timer has expired, clear the last hit object and current hit object.
                    if (lastHitObject != null) {
                        SetColor(Color.white, lastHitObject);
                        lastHitObject = null;
                        handOffset = Vector3.zero;
                        hitPointOffset = Vector3.zero;
                    }
                    currentHitObject = null;
                    handOffset = Vector3.zero;
                }
            }
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Interactable")) {
                if (obj != currentHitObject) {
                    SetColor(Color.white, obj);
                }
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        if (isControling && currentHitObject != null) {
            Gizmos.DrawLine(wristTransform.position, currentHitObject.transform.position);
        } else {
            Gizmos.DrawRay(ray.origin, ray.direction * rayLength);

        }
    }
}
