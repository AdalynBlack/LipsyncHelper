## Introduction
This is a simple script which automatically converts Oculus Lipsync data into a Unity animation file to aid in creating lipsync animations

## Generating the lipsync data
- Install the [Oculus Lipsync Unity Integration](https://developer.oculus.com/downloads/package/oculus-lipsync-unity/) package from the Oculus website
- Use the [lipsync precompute feature](https://developer.oculus.com/documentation/unity/audio-ovrlipsync-precomputed-unity/) to generate the required OVRLipSyncSequence file

## Converting the lipsync data
1. Drag the Converter prefab into a scene
2. Select the prefab in the scene
3. Drag the lipsync sequence into "Ovr Lip Sync Sequence"
4. Drag the animation clip you wish to add lipsync to into "Animation Clip" (create a new animation clip if you don't want to modify one)
5. Setup the viseme names. A prefix option is provided for convenience, leave it blank if your visemes do not use a prefix
6. Click the convert button
7. If all went well, you should see the messages "Beginning Conversion..." and "Conversion complete!" back to back with no errors inbetween

## Issues
If you have any issues, report the issue on github
