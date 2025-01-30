using UnityEngine;
using Kinect = Windows.Kinect; // Kinect namespace

public class AvatarController : MonoBehaviour
{
    public BodySourceManager BodySourceManager; // Assign in Inspector
    private Kinect.Body trackedBody;
    private Animator animator;

    // Leg bone references
    private Transform leftUpperLeg, leftLowerLeg, leftFoot;
    private Transform rightUpperLeg, rightLowerLeg, rightFoot;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Get the humanoid bone transforms
        leftUpperLeg = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        leftLowerLeg = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);

        rightUpperLeg = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        rightLowerLeg = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
        rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
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
            MapLegJoints();          // Map Kinect data to avatar legs
        }
    }

    /// <summary>
    /// Align the avatar’s root position with the Kinect-tracked SpineBase
    /// </summary>
    private void AlignAvatarWithKinect()
    {
        Vector3 spineBase = ConvertKinectToUnity(trackedBody.Joints[Kinect.JointType.SpineBase]);
        transform.position = spineBase; // Keep avatar positioned with user
    }


    /// Map Kinect leg joints to the avatar’s leg bones
    /// </summary>
private void MapLegJoints()
{
    float smoothFactor = 5f; // Adjust for smoother motion

    // Get Kinect joint positions
    Vector3 hipLeft = ConvertKinectToUnity(trackedBody.Joints[Windows.Kinect.JointType.HipLeft]);
    Vector3 kneeLeft = ConvertKinectToUnity(trackedBody.Joints[Windows.Kinect.JointType.KneeLeft]);
    Vector3 ankleLeft = ConvertKinectToUnity(trackedBody.Joints[Windows.Kinect.JointType.AnkleLeft]);

    Vector3 hipRight = ConvertKinectToUnity(trackedBody.Joints[Windows.Kinect.JointType.HipRight]);
    Vector3 kneeRight = ConvertKinectToUnity(trackedBody.Joints[Windows.Kinect.JointType.KneeRight]);
    Vector3 ankleRight = ConvertKinectToUnity(trackedBody.Joints[Windows.Kinect.JointType.AnkleRight]);

    // Smooth rotations for realistic leg movement
    leftUpperLeg.rotation = Quaternion.Slerp(leftUpperLeg.rotation, GetKinectBoneRotation(hipLeft, kneeLeft), Time.deltaTime * smoothFactor);
    leftLowerLeg.rotation = Quaternion.Slerp(leftLowerLeg.rotation, GetKinectBoneRotation(kneeLeft, ankleLeft), Time.deltaTime * smoothFactor);
    leftFoot.rotation = Quaternion.Slerp(leftFoot.rotation, GetKinectBoneRotation(ankleLeft, ConvertKinectToUnity(trackedBody.Joints[Windows.Kinect.JointType.FootLeft])), Time.deltaTime * smoothFactor);

    rightUpperLeg.rotation = Quaternion.Slerp(rightUpperLeg.rotation, GetKinectBoneRotation(hipRight, kneeRight), Time.deltaTime * smoothFactor);
    rightLowerLeg.rotation = Quaternion.Slerp(rightLowerLeg.rotation, GetKinectBoneRotation(kneeRight, ankleRight), Time.deltaTime * smoothFactor);
    rightFoot.rotation = Quaternion.Slerp(rightFoot.rotation, GetKinectBoneRotation(ankleRight, ConvertKinectToUnity(trackedBody.Joints[Windows.Kinect.JointType.FootRight])), Time.deltaTime * smoothFactor);
}


    /// <summary>
    /// Compute the rotation of a bone based on the direction between two Kinect joints
    /// </summary>
    private Quaternion GetKinectBoneRotation(Vector3 start, Vector3 end)
{
    Vector3 direction = (end - start).normalized;

    // Prevent zero vector errors
    if (direction == Vector3.zero)
    {
        return Quaternion.identity;
    }

    return Quaternion.LookRotation(direction, Vector3.up);
}


    /// <summary>
    /// Convert Kinect joint coordinates to Unity's coordinate system
    /// </summary>
    private Vector3 ConvertKinectToUnity(Windows.Kinect.Joint joint)
    {
        float scaleFactor = 2.0f; // Adjust as needed
        return new Vector3(joint.Position.X, joint.Position.Y, -joint.Position.Z) * scaleFactor;
    }
}
