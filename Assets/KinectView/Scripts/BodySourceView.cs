using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

public class BodySourceView : MonoBehaviour 
{
    public Material BoneMaterial; // Material for leg bones visualization
    public GameObject BodySourceManager; // Reference to the BodySourceManager script

    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;

    // Mapping of leg joints (bones connecting leg joints)
    private Dictionary<Kinect.JointType, Kinect.JointType> _LegBoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
    };

    void Update() 
    {
        if (BodySourceManager == null)
        {
            return;
        }
        
        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }
        
        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }
        
        List<ulong> trackedIds = new List<ulong>();
        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }
        
        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);

        // Remove untracked bodies
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }

        // Process tracked bodies
        foreach (var body in data)
        {
            if (body == null || !body.IsTracked)
            {
                continue;
            }
            
            if (!_Bodies.ContainsKey(body.TrackingId))
            {
                _Bodies[body.TrackingId] = CreateLegsBodyObject(body.TrackingId);
            }
            
            RefreshLegsBodyObject(body, _Bodies[body.TrackingId]);
        }
    }
    
    // Create a body object for legs visualization
    private GameObject CreateLegsBodyObject(ulong id)
    {
        GameObject body = new GameObject("LegsBody:" + id);
        
        foreach (Kinect.JointType jt in _LegBoneMap.Keys)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            
            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.material = BoneMaterial;
            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;
            
            jointObj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
        }
        
        return body;
    }
    
    // Refresh leg data in the body object
    private void RefreshLegsBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        foreach (var bone in _LegBoneMap)
        {
            Kinect.JointType sourceJoint = bone.Key;
            Kinect.JointType targetJoint = bone.Value;

            Kinect.Joint source = body.Joints[sourceJoint];
            Kinect.Joint target = body.Joints[targetJoint];

            Transform jointObj = bodyObject.transform.Find(sourceJoint.ToString());
            jointObj.localPosition = GetVector3FromJoint(source);

            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if (source.TrackingState != Kinect.TrackingState.NotTracked &&
                target.TrackingState != Kinect.TrackingState.NotTracked)
            {
                lr.enabled = true;
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, GetVector3FromJoint(target));
                lr.startColor = GetColorForState(source.TrackingState);
                lr.endColor = GetColorForState(target.TrackingState);
            }
            else
            {
                lr.enabled = false;
            }
        }
    }

    // Get the appropriate color based on the joint's tracking state
    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
            case Kinect.TrackingState.Tracked:
                return Color.green;

            case Kinect.TrackingState.Inferred:
                return Color.red;

            default:
                return Color.black;
        }
    }
    
    // Convert Kinect joint position to Unity world space
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }
}
