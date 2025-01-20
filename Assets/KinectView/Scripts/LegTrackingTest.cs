using UnityEngine;

public class LegTrackingTest : MonoBehaviour
{
    public BodySourceView bodySourceView;

    void Update()
    {
        if (bodySourceView == null)
        {
            return;
        }

        // Get all tracked IDs
        foreach (ulong trackingId in bodySourceView.GetTrackedIds())
        {
            LegTrackingData legData = bodySourceView.GetLegData(trackingId);
            if (legData != null)
            {
                Debug.Log("Left Knee Position: " + legData.LeftKnee);
                Debug.Log("Right Foot Position: " + legData.RightFoot);
            }
        }
    }
}
