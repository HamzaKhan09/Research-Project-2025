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
    //transform.position = new Vector3(0, 0, 2f); // Start slightly in front of Kinect
    //transform.rotation = Quaternion.Euler(0, 180, 0); // Ensure correct facing direction
}



   void Update()
{
    if (BodySourceManager == null) return;

    Kinect.Body[] data = BodySourceManager.GetData();
    if (data == null) return;

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
        MapJointsToAvatar();

        Vector3 footRight = ConvertKinectToUnity(trackedBody.Joints[Windows.Kinect.JointType.FootRight]);
        Vector3 footLeft = ConvertKinectToUnity(trackedBody.Joints[Windows.Kinect.JointType.FootLeft]);

        Debug.Log($"FootRight Position: {footRight}");
        Debug.Log($"Foot Right Position: {footLeft}");
        
    }
}


private void AlignAvatarWithKinect()
{

    if (trackedBody == null) return; // Prevents errors when Kinect loses tracking

    Vector3 spineBase = ConvertKinectToUnity(trackedBody.Joints[Windows.Kinect.JointType.SpineBase]);

    spineBase.y = 0.0f;

        if (trackedBody.Joints[Windows.Kinect.JointType.SpineBase].TrackingState == Windows.Kinect.TrackingState.NotTracked)
    {
        return; // Keeps the last valid position instead of snapping to a bad default
    }

    Vector3 hipLeft = ConvertKinectToUnity(trackedBody.Joints[Windows.Kinect.JointType.HipLeft]);
    Vector3 hipRight = ConvertKinectToUnity(trackedBody.Joints[Windows.Kinect.JointType.HipRight]);

    Vector3 forward = (hipRight - hipLeft).normalized;
    if (forward != Vector3.zero)
    {
        transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }

   
    transform.position = Vector3.Lerp(transform.position, spineBase, Time.deltaTime * 5f);

   // Rotation Offset
    transform.rotation *= Quaternion.Euler(0, -90, 0);

    // Right Leg Position
    //rightUpperLeg.position = new Vector3(rightUpperLeg.position.x, 0.9f, rightUpperLeg.position.z); // Force leg lower
}



private void MapJointsToAvatar()
{
    float smoothFactor = 5f; //smoother motion

    //Spine and Head
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
    if (trackedBody.Joints[startJoint].TrackingState == Windows.Kinect.TrackingState.NotTracked ||
        trackedBody.Joints[endJoint].TrackingState == Windows.Kinect.TrackingState.NotTracked)
    {
        return lastKnownRotations.ContainsKey(startJoint) ? lastKnownRotations[startJoint] : Quaternion.identity;
    }

    Vector3 start = ConvertKinectToUnity(trackedBody.Joints[startJoint]);
    Vector3 end = ConvertKinectToUnity(trackedBody.Joints[endJoint]);

    Vector3 direction = (end - start).normalized;
    if (direction == Vector3.zero) return Quaternion.identity;

    Quaternion newRotation = Quaternion.LookRotation(direction, Vector3.up);

    // Right Foot Flipping Issue**
    if (startJoint == Windows.Kinect.JointType.AnkleLeft)
    {
        newRotation *= Quaternion.Euler(0, 180, 0); // Inverts right foot correctly
    }

    // Apply different rotation fixes for each leg
    if (startJoint == Windows.Kinect.JointType.HipRight || startJoint == Windows.Kinect.JointType.KneeRight)
    {
        newRotation *= Quaternion.Euler(90, 0, 180); // right leg
    }
    else if (startJoint == Windows.Kinect.JointType.HipLeft || startJoint == Windows.Kinect.JointType.KneeLeft)
    {
        newRotation *= Quaternion.Euler(-90, 0, 180); // left leg 
    }

    lastKnownRotations[startJoint] = newRotation;
    return lastKnownRotations[startJoint];

}


private Vector3 ConvertKinectToUnity(Windows.Kinect.Joint joint)
{
    float scaleFactor = 1.0f;

    // Flip Z-axis for depth correction
    return new Vector3(joint.Position.X, joint.Position.Y*scaleFactor, -joint.Position.Z);
}

}
