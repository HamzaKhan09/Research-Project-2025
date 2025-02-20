using UnityEngine;

public class AvatarHandIK : MonoBehaviour
{
    public Animator animator; // Avatar's Animator
    public Transform vrLeftHandTarget; // VR Left Hand Target
    public Transform vrRightHandTarget; // VR Right Hand Target

    public float ikWeight = 1.0f; // How strongly IK follows the target
    public Transform vrHeadTarget;

void OnAnimatorIK(int layerIndex)
{
    if (animator)
    {
        // Get Avatar’s root position
        Vector3 avatarPosition = transform.position;

        // Apply IK to Left Hand (Fix Inverted Position)
        if (vrLeftHandTarget != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

            // Mirror the X position & Flip Z to fix movement direction
            Vector3 leftHandLocalOffset = vrLeftHandTarget.position - Camera.main.transform.position;
            leftHandLocalOffset.x = -leftHandLocalOffset.x; // Fix Left-Right
            leftHandLocalOffset.z = -leftHandLocalOffset.z; // Fix Forward-Backward

            animator.SetIKPosition(AvatarIKGoal.LeftHand, avatarPosition + leftHandLocalOffset);

            // Fix rotation so the palm faces correctly
            Quaternion leftHandRotationFix = vrLeftHandTarget.rotation * Quaternion.Euler(0, 180, 180);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandRotationFix);
        }

        // Apply IK to Right Hand (Fix Inverted Position)
        if (vrRightHandTarget != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

            // Mirror the X position & Flip Z to fix movement direction
            Vector3 rightHandLocalOffset = vrRightHandTarget.position - Camera.main.transform.position;
            rightHandLocalOffset.x = -rightHandLocalOffset.x; // Fix Left-Right
            rightHandLocalOffset.z = -rightHandLocalOffset.z; // Fix Forward-Backward

            animator.SetIKPosition(AvatarIKGoal.RightHand, avatarPosition + rightHandLocalOffset);

            // Fix rotation so the palm faces correctly
            Quaternion rightHandRotationFix = vrRightHandTarget.rotation * Quaternion.Euler(0, 180, 180);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandRotationFix);
        }

        // Apply IK to Head (Track Head Movement)
        if (vrHeadTarget != null)
        {
            animator.SetLookAtWeight(ikWeight);
            animator.SetLookAtPosition(vrHeadTarget.position);
        }
    }
}


}
