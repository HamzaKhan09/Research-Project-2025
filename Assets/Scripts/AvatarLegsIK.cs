using UnityEngine;
using Kinect=Windows.Kinect;

public class AvatarLegsIK : MonoBehaviour
{
    public BodySourceManager BodySourceManager;
    private Windows.Kinect.Body trackedBody;

    public Transform leftLegIKTarget, rightLegIKTarget;
    public Transform leftKneeIKHint, rightKneeIKHint;
    public Transform avatarRoot; // The main transform of the avatar (spine or hips)
    
    private Vector3 lastSpinePosition = Vector3.zero;

    void Update()
    {
        if (BodySourceManager == null)
        {
            Debug.LogError("BodySourceManager is not assigned!");
            return;
        }

        Kinect.Body[] data = BodySourceManager.GetData();
        if (data == null)
        {
            Debug.LogWarning("No Kinect data received.");
            return;
        }

        foreach (var body in data)
        {
            if (body != null && body.IsTracked)
            {
                trackedBody = body;
                break;
            }
        }

        if (trackedBody != null)
        {
            AlignAvatarWithKinect();
            UpdateIKTargets();
        }
    }

    /// <summary>
    /// Moves the avatar to match the Kinect-tracked body position
    /// </summary>
    void AlignAvatarWithKinect()
    {
        if (trackedBody == null || avatarRoot == null) return;

        Vector3 spineBase = ConvertKinectToUnity(trackedBody.Joints[Kinect.JointType.SpineBase]);

        // Smooth transition to avoid sudden jumps
        avatarRoot.position = Vector3.Lerp(avatarRoot.position, spineBase, Time.deltaTime * 5f);

        // Adjust avatar rotation to face Kinect correctly
        Vector3 hipLeft = ConvertKinectToUnity(trackedBody.Joints[Kinect.JointType.HipLeft]);
        Vector3 hipRight = ConvertKinectToUnity(trackedBody.Joints[Kinect.JointType.HipRight]);

        Vector3 forward = (hipRight - hipLeft).normalized;
        if (forward != Vector3.zero)
        {
            avatarRoot.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }

        lastSpinePosition = avatarRoot.position; // Store last position to prevent jumps
    }

    /// <summary>
    /// Updates the position of the IK targets based on Kinect tracking
    /// </summary>
    void UpdateIKTargets()
    {
        if (trackedBody == null) return;

        Vector3 leftFootPos = ConvertKinectToUnity(trackedBody.Joints[Kinect.JointType.AnkleLeft]);
        Vector3 rightFootPos = ConvertKinectToUnity(trackedBody.Joints[Kinect.JointType.AnkleRight]);

        Vector3 leftKneePos = ConvertKinectToUnity(trackedBody.Joints[Kinect.JointType.KneeLeft]);
        Vector3 rightKneePos = ConvertKinectToUnity(trackedBody.Joints[Kinect.JointType.KneeRight]);

        // Smooth position updates to avoid jittering
        if (leftLegIKTarget != null) leftLegIKTarget.position = Vector3.Lerp(leftLegIKTarget.position, leftFootPos, Time.deltaTime * 5f);
        if (rightLegIKTarget != null) rightLegIKTarget.position = Vector3.Lerp(rightLegIKTarget.position, rightFootPos, Time.deltaTime * 5f);

        // Set knee hint positions for smoother leg bending
        if (leftKneeIKHint != null) leftKneeIKHint.position = Vector3.Lerp(leftKneeIKHint.position, leftKneePos, Time.deltaTime * 5f);
        if (rightKneeIKHint != null) rightKneeIKHint.position = Vector3.Lerp(rightKneeIKHint.position, rightKneePos, Time.deltaTime * 5f);
    }

    /// <summary>
    /// Converts Kinect Joint Positions to Unity World Positions
    /// </summary>
    private Vector3 ConvertKinectToUnity(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 2f, joint.Position.Y * 2f, -joint.Position.Z * 2f);
    }
}
