using Oculus.Interaction.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabPoint : MonoBehaviour
{
    // 5.1 Calculate the point to concact with the object on hand
    public Hand hand;
    public Vector3 Position {
        get {
            Pose output;
            hand.GetJointPose(HandJointId.HandMiddle1, out output);
            return output.position;
        }
    }

    private void Update() {
        this.transform.position = Position;
    }
}
