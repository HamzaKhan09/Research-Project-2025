using UnityEngine;
using Kinect = Windows.Kinect; // Requires Kinect SDK 2.0

public class AvatarController : MonoBehaviour
{
    public BodySourceManager BodySourceManager; // Assign your BodySourceManager GameObject in the Inspector
    private Kinect.Body trackedBody;

    private Animator animator;

    // Bone references for the avatar
    private Transform leftUpperLeg, leftLowerLeg, leftFoot;
    private Transform rightUpperLeg, rightLowerLeg, rightFoot;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Map the avatar's humanoid bones
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

        Kinect.Body[] data = BodySourceManager.GetData();
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
            MapLegJoints();
        }
    }

    private void MapLegJoints()
    {
        // Map Kinect joints to the avatar's leg bones
        Vector3 hipLeft = ConvertKinectToUnity(trackedBody.Joints[Kinect.JointType.HipLeft]);
        Vector3 kneeLeft = ConvertKinectToUnity(trackedBody.Joints[Kinect.JointType.KneeLeft]);
        Vector3 ankleLeft = ConvertKinectToUnity(trackedBody.Joints[Kinect.JointType.AnkleLeft]);

        Vector3 hipRight = ConvertKinectToUnity(trackedBody.Joints[Kinect.JointType.HipRight]);
        Vector3 kneeRight = ConvertKinectToUnity(trackedBody.Joints[Kinect.JointType.KneeRight]);
        Vector3 ankleRight = ConvertKinectToUnity(trackedBody.Joints[Kinect.JointType.AnkleRight]);

        leftUpperLeg.position = hipLeft;
        leftLowerLeg.position = kneeLeft;
        leftFoot.position = ankleLeft;

        rightUpperLeg.position = hipRight;
        rightLowerLeg.position = kneeRight;
        rightFoot.position = ankleRight;
    }

    private Vector3 ConvertKinectToUnity(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X, joint.Position.Y, -joint.Position.Z) * 10; // Scale as needed
    }
}
