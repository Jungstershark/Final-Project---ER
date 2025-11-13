using UnityEngine;
using Oculus.Interaction.Locomotion;

public class StepInputController : MonoBehaviour
{
    public StepLocomotionBroadcaster stepBroadcaster;

    void Update()
    {
        Vector2 stick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        if (stick.y > 0.5f)
            stepBroadcaster.StepForward();
        else if (stick.y < -0.5f)
            stepBroadcaster.StepBackward();
        else if (stick.x > 0.5f)
            stepBroadcaster.StepRight();
        else if (stick.x < -0.5f)
            stepBroadcaster.StepLeft();
    }
}
