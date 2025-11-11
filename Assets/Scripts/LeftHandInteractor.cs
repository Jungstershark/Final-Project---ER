using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandInteractor : MonoBehaviour
{
    public Transform wristTransform;
    public RightHandInteractor rightHandInteractor;

    public bool isActivated = false;
    public bool isScaling = false;

    public Vector3 offset;
    public GameObject visualHand;


    public float originalDistance;

    public Vector3 originalScale;


    public float minScale = 0.5f;
    public float maxScale = 3.0f;

    public float staticScaleRecord = 1.0f;
    public float staticScaleFactor = 1.0f;

    public float savedDistance;

    public Vector3 leftHandPosition => wristTransform.position;
    public Vector3 rightHandPosition => rightHandInteractor.wristTransform.position;

    public GameObject currentGrabbingObject => rightHandInteractor.currentHitObject;
    public void SetActivateState(bool state)
    {
        if (state != isActivated)
        {
            isActivated = state;
            if (isActivated)
            {

            }
            else
            {

            }
        }
    }

    public void TriggerGrabbing()
    {
        if (isActivated)
        {
            if (!isScaling)
            {
                Vector3 leftHandPosition = wristTransform.position;
                Vector3 rightHandPosition = rightHandInteractor.wristTransform.position;
                originalScale = rightHandInteractor.currentHitObject.transform.localScale;
                originalDistance = Vector3.Distance(leftHandPosition, rightHandPosition);
                savedDistance = originalDistance;
                isScaling = true;
            }
        }
    }

    public void TriggerNonGrabbing()
    {
        if (isScaling)
        {
            isScaling = false;
            staticScaleFactor = currentGrabbingObject.transform.localScale.x / staticScaleRecord;
            originalScale = currentGrabbingObject.transform.localScale;
            originalDistance = savedDistance;
        }
    }





    //  Task 6. Scaling
    //  TODO: Update the scale of the object according to the distance between the two hands.
    public void UpdateScaling(float OriginalDistance, float CurrentDistance, Vector3 OriginalScale, float MinScale, float MaxScale, Vector3 CurrentGrabPoint, Vector3 ObjectOffsetAfterManipulation)
    {
        if (currentGrabbingObject == null) return;

        // Calculate scale ratio based on hand distance change
        float scaleRatio = CurrentDistance / OriginalDistance;
        scaleRatio = Mathf.Clamp(scaleRatio, MinScale, MaxScale);

        // Apply uniform scaling
        Vector3 newScale = OriginalScale * scaleRatio;
        currentGrabbingObject.transform.localScale = newScale;

        // Adjust position so the contact point (grab point) stays fixed
        Vector3 newCenter = CurrentGrabPoint + ObjectOffsetAfterManipulation * scaleRatio;
        currentGrabbingObject.transform.position = newCenter;
    }





    void Update()
    {
        if (isActivated)
        {
            visualHand.transform.position = wristTransform.position + offset;
            visualHand.transform.rotation = wristTransform.rotation;
            if (isScaling)
            {
                Vector3 currentGrabPoint = rightHandInteractor.GrabPoint + rightHandInteractor.handOffset;
                Vector3 objectOffsetWithManipulation = (rightHandInteractor.currentHitObject.transform.position - currentGrabPoint) * staticScaleFactor;
                float currentDistance = Vector3.Distance(leftHandPosition, rightHandPosition);
                UpdateScaling(originalDistance, currentDistance, originalScale, minScale, maxScale, currentGrabPoint, objectOffsetWithManipulation);
                savedDistance = currentDistance;
            }
            else
            {
                Vector3 currentGrabPoint = rightHandInteractor.GrabPoint + rightHandInteractor.handOffset;
                Vector3 objectOffsetWithManipulation = (rightHandInteractor.currentHitObject.transform.position - currentGrabPoint) * staticScaleFactor;
                UpdateScaling(originalDistance, savedDistance, originalScale, minScale, maxScale, currentGrabPoint, objectOffsetWithManipulation);
            }

        }
        else
        {
            if (isScaling)
            {
                isScaling = false;
            }
            visualHand.transform.position = wristTransform.position;
            visualHand.transform.rotation = wristTransform.rotation;

        }

    }

    public void TriggerRightHandGrabbing()
    {

        if (rightHandInteractor.currentHitObject != null)
        {

            offset = rightHandInteractor.handOffset;
            originalScale = rightHandInteractor.currentHitObject.transform.localScale;
            staticScaleFactor = 1f;
            staticScaleRecord = originalScale.x;
            savedDistance = 1f;
            originalDistance = 1f;
            SetActivateState(true);
        }
    }
    public void TriggerRightHandNonGrabbing()
    {
        offset = Vector3.zero;
        SetActivateState(false);
    }
}
