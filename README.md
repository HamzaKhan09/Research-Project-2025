# Research Project: Full-Body Tracking in VR

Unity prototype for full-body avatar tracking in VR, built as a research project at TU Ilmenau. It combines a Meta Quest headset (head and hands) with a Kinect v2 depth camera (body and legs) to drive a full-body avatar.

## Features

- **Kinect body tracking:** streams body, colour, depth and infrared data into Unity and visualises the skeleton.
- **Hand tracking:** drives the avatar's hands and fingers from Meta Quest hand tracking.
- **Leg tracking:** IK-based legs on the avatar fed by the Kinect lower-body data.
- **Avatar control:** head and hand IK on a rigged avatar, with a Meta Avatar setup.

## Requirements

- Unity `6000.0.37f1`
- Meta Quest headset with the Meta XR SDK (installed via the Package Manager)
- Kinect v2 sensor and the Kinect for Windows SDK 2.0 (Windows only)

## Getting started

1. Clone the repository and open it with Unity `6000.0.37f1`.
2. Let Unity import the packages.
3. Open a scene from `Assets/Scenes/`:
   - `Kinect_VR`: Kinect body tracking in VR
   - `Meta tracking`: Meta Quest head and hand tracking
   - `LegTracking`: full-body tracking with legs

## Project structure

- `Assets/Scripts/`: avatar control, IK, and the Kinect source managers and views
- `Assets/Scenes/`: the Unity scenes above
- `Assets/KinectView/`: Kinect visualisation assets

## Third-party content

This repository includes third-party assets: the Meta XR / Oculus SDK, Microsoft's Kinect Unity add-in, Unity Standard Assets and Starter Assets, and Mixamo-style character models. Each remains under its own licence and is not covered by any licence for the original code in this repository.
