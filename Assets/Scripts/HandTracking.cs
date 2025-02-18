using UnityEngine;
using UnityEngine.XR;

public class HandTrackingMapper : MonoBehaviour
{
    public Transform leftHandTarget; // Assign the avatar's left hand bone
    public Transform rightHandTarget; // Assign the avatar's right hand bone

    void Update()
    {
        // Track left hand position & rotation
        InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        if (leftHand.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 leftPos))
            leftHandTarget.position = leftPos;
        if (leftHand.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion leftRot))
            leftHandTarget.rotation = leftRot;

        // Track right hand position & rotation
        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (rightHand.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 rightPos))
            rightHandTarget.position = rightPos;
        if (rightHand.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rightRot))
            rightHandTarget.rotation = rightRot;
    }
}
