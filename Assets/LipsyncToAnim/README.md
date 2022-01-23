## Introduction
This is a simple script which automatically converts Oculus Lipsync data into a Unity animation file to aid in creating lipsync animations

## Generating the lipsync data
- Install the [Oculus Lipsync Unity Integration](https://developer.oculus.com/downloads/package/oculus-lipsync-unity/) package from the Oculus website
- Use the [lipsync precompute feature](https://developer.oculus.com/documentation/unity/audio-ovrlipsync-precomputed-unity/) to generate the required OVRLipSyncSequence file

## Converting the lipsync data
1. Click on the avatar you wish to animate
2. Click "Add Component" and add the component "Lipsync To Anim"
3. Drag the audio into "Audio Clip"
4. Drag the animation clip you wish to add lipsync to into "Animation Clip" (create a new animation clip if you don't want to modify one)
5. Drag the mesh you wish to animate to the "Mouth Object" section
6. Setup the viseme names. A prefix option is provided for convenience, leave it blank if your visemes do not use a prefix
7. Change any [settings](#settings) you wish
8. Click the convert button
9. If all went well, you should see the messages "Beginning Conversion..." and "Conversion complete!" appear

## Settings
- Oculus Tweaks
	- Offline Mode: This toggles the offline mode for the Oculus Lipsync SDK
- Animation Tweaks
	- Time Shift: Shift the timing of the animation. This value is in milliseconds
- Viseme Tweaks:
	- Min Value: The value below which all viseme keyframes are set to zero (0-100 range)
	- Gain: The multiplier applied to the viseme values from the SDK (0-100 range)
	- Decimate Value: Determines how aggresively to remove keyframes. There is no hard limit, but its affects are theoretically present from values 0-369,800. Recommended value are below 20,000
	- Optimize: Whether or not to remove keyframes which are deemed unnecessary. How many are removed is dependant on the decimation value. Using with with min value and decimate value can reduce the number of keyframes by up to 90% !IT IS HIGHLY RECOMMENDED TO KEEP THIS ENABLED!
- Convert:
	- This is not a setting, but rather a button. Clicking this will show no visible feedback, but will run the script with the given settings. You can tell it worked through the message "Conversion Complete" appearing in the console.

## Issues
If you have any issues, report the issue on [Github](https://github.com/TonyGamer/LipsyncHelper/issues)

## Credits
- Mouth Mesh (In the test scene): https://booth.pm/ja/items/1694770
- Test Audio: The FitnessGram™ Pacer Test
