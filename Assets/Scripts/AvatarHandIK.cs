using UnityEngine;

public class AvatarHandIK : MonoBehaviour
{
    public Animator animator; // Avatar's Animator
       // VR Hand & Elbow Tracking
    public Transform vrLeftHandTarget;
    public Transform vrRightHandTarget;
    public Transform vrLeftElbowTarget;
    public Transform vrRightElbowTarget;

    // VR Head Tracking
    public Transform vrHeadTarget;

    // VR Finger Tracking
    public Transform vrLeftThumb;
    public Transform vrLeftIndex;
    public Transform vrLeftMiddle;
    public Transform vrLeftRing;
    public Transform vrLeftPinky;

    public Transform vrRightThumb;
    public Transform vrRightIndex;
    public Transform vrRightMiddle;
    public Transform vrRightRing;
    public Transform vrRightPinky;
    public float ikWeight = 1.0f; // How strongly IK follows the target

    void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            // Apply IK to Left Hand
            if (vrLeftHandTarget != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, ikWeight);

                // Rotate the left hand to face forward
                Quaternion leftHandRotationFix = vrLeftHandTarget.rotation * Quaternion.Euler(0, 180, 0);
                
                Vector3 mirroredLeftPosition = vrLeftHandTarget.position;
                mirroredLeftPosition.x = -mirroredLeftPosition.x; // Mirror X-axis
                animator.SetIKPosition(AvatarIKGoal.LeftHand, mirroredLeftPosition);

                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandRotationFix);
            }


             if (vrLeftElbowTarget != null)
            {
                animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, ikWeight);
                animator.SetIKHintPosition(AvatarIKHint.LeftElbow, vrLeftElbowTarget.position);
            }


            // Apply IK to Right Hand
            if (vrRightHandTarget != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, ikWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, ikWeight);

                // Rotate the right hand to face forward
                Quaternion rightHandRotationFix = vrRightHandTarget.rotation * Quaternion.Euler(0, 180, 0);

                Vector3 mirroredRightPosition = vrRightHandTarget.position;
                mirroredRightPosition.x = -mirroredRightPosition.x; // Mirror X-axis
                animator.SetIKPosition(AvatarIKGoal.RightHand, mirroredRightPosition);

                animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandRotationFix);
            }

            if (vrRightElbowTarget != null)
            {
                animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, ikWeight);
                animator.SetIKHintPosition(AvatarIKHint.RightElbow, vrRightElbowTarget.position);
            }

                        // Apply IK to Head
            if (vrHeadTarget != null)
            {
                animator.SetLookAtWeight(ikWeight);
                animator.SetLookAtPosition(vrHeadTarget.position);
            }

             if (vrLeftThumb != null)
                animator.SetBoneLocalRotation(HumanBodyBones.LeftThumbProximal, vrLeftThumb.rotation);
            if (vrLeftIndex != null)
                animator.SetBoneLocalRotation(HumanBodyBones.LeftIndexProximal, vrLeftIndex.rotation);
            if (vrLeftMiddle != null)
                animator.SetBoneLocalRotation(HumanBodyBones.LeftMiddleProximal, vrLeftMiddle.rotation);
            if (vrLeftRing != null)
                animator.SetBoneLocalRotation(HumanBodyBones.LeftRingProximal, vrLeftRing.rotation);
            if (vrLeftPinky != null)
                animator.SetBoneLocalRotation(HumanBodyBones.LeftLittleProximal, vrLeftPinky.rotation);

            // Apply Finger Tracking for Right Hand
            if (vrRightThumb != null)
                animator.SetBoneLocalRotation(HumanBodyBones.RightThumbProximal, vrRightThumb.rotation);
            if (vrRightIndex != null)
                animator.SetBoneLocalRotation(HumanBodyBones.RightIndexProximal, vrRightIndex.rotation);
            if (vrRightMiddle != null)
                animator.SetBoneLocalRotation(HumanBodyBones.RightMiddleProximal, vrRightMiddle.rotation);
            if (vrRightRing != null)
                animator.SetBoneLocalRotation(HumanBodyBones.RightRingProximal, vrRightRing.rotation);
            if (vrRightPinky != null)
                animator.SetBoneLocalRotation(HumanBodyBones.RightLittleProximal, vrRightPinky.rotation);

        }
    }
}
