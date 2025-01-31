using UnityEngine;
using Kinect = Windows.Kinect;
using System.Collections.Generic; 


public class AvatarController : MonoBehaviour
{
    public BodySourceManager BodySourceManager;
    private Kinect.Body trackedBody;
    private Animator animator;
    
    // Store last known bone rotations when Kinect loses tracking
    private Dictionary<Windows.Kinect.JointType, Quaternion> lastKnownRotations = new Dictionary<Windows.Kinect.JointType, Quaternion>();

    // Leg bone references
    private Transform spine, head;
    private Transform leftUpperLeg, leftLowerLeg, leftFoot;
    private Transform rightUpperLeg, rightLowerLeg, rightFoot;

void Start()
{
    animator = GetComponent<Animator>();

    // Initialize bone transforms
    leftUpperLeg = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
    leftLowerLeg = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
    leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);

    rightUpperLeg = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
    rightLowerLeg = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
    rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);

    // Set a default position to avoid floating at start
    transform.position = new Vector3(0, 0, 2f); // Start slightly in front of Kinect
    transform.rotation = Quaternion.Euler(0, 180, 0); // Ensure correct facing direction
}



    void Update()
    {
        if (BodySourceManager == null) return;

        Windows.Kinect.Body[] data = BodySourceManager.GetData();
        if (data == null) return;

        // Find the first tracked body
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


            AlignAvatarWithKinect(); // Align avatar with user position
            MapJointsToAvatar();          // Map Kinect data to avatar legs
        }
    }


private void AlignAvatarWithKinect()
{
    Vector3 spineBase = ConvertKinectToUnity(trackedBody.Joints[Windows.Kinect.JointType.SpineBase]);

    // Keep the avatar at a fixed height to avoid unnatural movement
    spineBase.y = 0.0f; // Keeps the avatar on the ground

    // Adjust Kinect's forward direction
    Vector3 hipLeft = ConvertKinectToUnity(trackedBody.Joints[Windows.Kinect.JointType.HipLeft]);
    Vector3 hipRight = ConvertKinectToUnity(trackedBody.Joints[Windows.Kinect.JointType.HipRight]);

    Vector3 forward = (hipRight - hipLeft).normalized;
    if (forward != Vector3.zero)
    {
        transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }

    // Apply smooth position update
    transform.position = Vector3.Lerp(transform.position, spineBase, Time.deltaTime * 5f);
}


private void MapJointsToAvatar()
{
    float smoothFactor = 5f; // Adjust for smoother motion

    // Map Spine and Head
//spine.rotation = Quaternion.Slerp(spine.rotation, GetKinectBoneRotation(Windows.Kinect.JointType.SpineMid, Windows.Kinect.JointType.Head), Time.deltaTime * smoothFactor);
    //head.rotation = Quaternion.Slerp(head.rotation, GetKinectBoneRotation(Windows.Kinect.JointType.Head, Windows.Kinect.JointType.SpineMid), Time.deltaTime * smoothFactor);

    // Map Left Leg
    leftUpperLeg.rotation = Quaternion.Slerp(leftUpperLeg.rotation, GetKinectBoneRotation(Windows.Kinect.JointType.HipLeft, Windows.Kinect.JointType.KneeLeft), Time.deltaTime * smoothFactor);
    leftLowerLeg.rotation = Quaternion.Slerp(leftLowerLeg.rotation, GetKinectBoneRotation(Windows.Kinect.JointType.KneeLeft, Windows.Kinect.JointType.AnkleLeft), Time.deltaTime * smoothFactor);
    leftFoot.rotation = Quaternion.Slerp(leftFoot.rotation, GetKinectBoneRotation(Windows.Kinect.JointType.AnkleLeft, Windows.Kinect.JointType.FootLeft), Time.deltaTime * smoothFactor);

    // Map Right Leg
    rightUpperLeg.rotation = Quaternion.Slerp(rightUpperLeg.rotation, GetKinectBoneRotation(Windows.Kinect.JointType.HipRight, Windows.Kinect.JointType.KneeRight), Time.deltaTime * smoothFactor);
    rightLowerLeg.rotation = Quaternion.Slerp(rightLowerLeg.rotation, GetKinectBoneRotation(Windows.Kinect.JointType.KneeRight, Windows.Kinect.JointType.AnkleRight), Time.deltaTime * smoothFactor);
    rightFoot.rotation = Quaternion.Slerp(rightFoot.rotation, GetKinectBoneRotation(Windows.Kinect.JointType.AnkleRight, Windows.Kinect.JointType.FootRight), Time.deltaTime * smoothFactor);
}




private Quaternion GetKinectBoneRotation(Windows.Kinect.JointType startJoint, Windows.Kinect.JointType endJoint)
{
    // Ensure Kinect is tracking the joints
    if (trackedBody.Joints[startJoint].TrackingState == Windows.Kinect.TrackingState.NotTracked ||
        trackedBody.Joints[endJoint].TrackingState == Windows.Kinect.TrackingState.NotTracked)
    {
        return lastKnownRotations.ContainsKey(startJoint) ? lastKnownRotations[startJoint] : Quaternion.identity;
    }

    // Get joint positions
    Vector3 start = ConvertKinectToUnity(trackedBody.Joints[startJoint]);
    Vector3 end = ConvertKinectToUnity(trackedBody.Joints[endJoint]);

    // Compute direction
    Vector3 direction = (end - start).normalized;
    if (direction == Vector3.zero) return Quaternion.identity;

    // Apply rotation fix for Kinect-Unity coordinate differences
    Quaternion newRotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(-90, 180, 0);

    lastKnownRotations[startJoint] = newRotation;
    return newRotation;
}



    /// <summary>
    /// Convert Kinect joint coordinates to Unity's coordinate system
    /// </summary>
private Vector3 ConvertKinectToUnity(Windows.Kinect.Joint joint)
{
    float scaleFactor = 2.0f; // Adjust as needed

    // Flip Z-axis to ensure depth scaling works correctly
    return new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z * -scaleFactor);
}


}
