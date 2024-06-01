using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class LipsyncToAnim : MonoBehaviour
{
    [Header("Required Assets")]
    [Tooltip("The lipsync file to use")]
    public AudioClip audioClip;
    [Tooltip("The animation clip to modify (contents will be preserved)")]
    public AnimationClip animationClip;

    [Header("Oculus Tweaks")]
    [Tooltip("Whether to run the Oculus Lipsync locally or not")]
    public bool offline = false;

    [Header("Animation Tweaks")]
    [Tooltip("The mesh to be animated")]
    public SkinnedMeshRenderer mouthObject;
    [Tooltip("How much to shift the animation by (in milliseconds)")]
    public float timeShift = 0;

    [Header("Viseme Names")]
    [Tooltip("This will be placed before every viseme name")]
    public string prefix = "V.";
    public string[] visemeNames = Enum.GetNames(typeof(OVRLipSync.Viseme));

    [Header("Viseme Tweaks")]
    [Tooltip("Minimum value to set a viseme to")]
    [Range(0, 100)]
    public float minValue = 15;
    [Tooltip("How much to multiply blendshapes by")]
    [Range(0, 100)]
    public float gain = 100;
    [Tooltip("How aggresively to remove keyframes (Recommended values: 1 - 10000)")]
    public float decimateValue = 3000;
    [Tooltip("Optimize keyframes (significantly improves performance in game)")]
    public bool optimize = true;

    [Space]
    [Tooltip("Click here to convert the lipsync to an animation")]
    public bool convert;

    void OnValidate()
    {
        if (convert)
        {
            Convert();
        }
        convert = false;
    }

    [ContextMenu("Convert Lipsync")]
    void Convert()
    {
        int keyframes = 0;

        Debug.Log("Beginning Conversion...");
        
        string path = GetRelativePath(transform, mouthObject.transform);

        
        MatchCollection mc = Regex.Matches(path, @"\b(?<=" + this.gameObject.name + @"\/)[\s\S]*");

        if (mc.Count == 0)
        {
            Debug.LogError("Error: No suitable path to selected viseme object");
        } else
        {
            path = mc[0].Value;
            Debug.Log("Path: " + path);
        }

        Debug.Log("Path: " + path);

        if (audioClip.loadState != AudioDataLoadState.Loaded)
            audioClip.LoadAudioData();
        OVRLipSyncSequence ovrLipSyncSequence = OVRLipSyncSequence.CreateSequenceFromAudioClip(audioClip, offline);

        float fps = ovrLipSyncSequence.entries.Count/ovrLipSyncSequence.length;
        float dt = 1 / fps;

        float viseme = 0;

        EditorCurveBinding[] editorCurves = new EditorCurveBinding[15];
        AnimationCurve[] animationCurves = new AnimationCurve[15];

        float[] prevSlopes = new float[15];
        float[] prevVisemes = new float[15];

        for(int i = 0; i < 15; i++)
        {
            editorCurves[i] = EditorCurveBinding.FloatCurve(
                path,
                typeof(SkinnedMeshRenderer),
                "blendShape." + prefix + visemeNames[i]
                );

            animationCurves[i] = new AnimationCurve();
        }

        float prevTime = 0;

        foreach (OVRLipSync.Frame frame in ovrLipSyncSequence.entries)
        {
            float time = frame.frameNumber * dt;

            for (int i = 0; i < 15; i++)
            {

                AnimationCurve curve = animationCurves[i];

                viseme = Mathf.Clamp(frame.Visemes[i] * gain, 0, 100);

                if (viseme < minValue)
                {
                    viseme = 0;
                }

                if (optimize)
                {
                    if (curve.length > 2)
                    {
                        float slope = viseme - prevVisemes[i];
                        slope *= fps; //fps = 1/dt. This is an optimization. Think of this as slope /= dt instead.

                        if (Mathf.Sign(slope) == Mathf.Sign(prevSlopes[i])) {
                            float curvature = slope - prevSlopes[i];
                            curvature *= fps;

                            if (curvature < decimateValue)
                            {
                                prevVisemes[i] = viseme;
                                continue;
                            }
                        }

                        prevSlopes[i] = slope;
                    }
                }

                keyframes++;
                curve.AddKey(prevTime + (0.001f*timeShift), prevVisemes[i]);

                prevVisemes[i] = viseme;
            }

            prevTime = time;
        }

        for(int i = 0; i < 15; i++)
        {
            AnimationCurve curve = animationCurves[i];

            for(int j = 0; j < curve.length; j++)
            {
                AnimationUtility.SetKeyLeftTangentMode(curve, j, AnimationUtility.TangentMode.Linear);
                AnimationUtility.SetKeyRightTangentMode(curve, j, AnimationUtility.TangentMode.Linear);
            }

            AnimationUtility.SetEditorCurve(animationClip, editorCurves[i], curve);
        }

        Debug.Log(keyframes + " keyframes were generated");
        Debug.Log("Conversion complete!");
    }

    private static string GetRelativePath(Transform parent, Transform child)
    {
        string path = child.name;
        while (child.parent != null)
        {
            child = child.parent;
            if (child == parent) return path;
            path = child.name + "/" + path;
        }
        throw new KeyNotFoundException("Unable to locate parent from child");
    }
}
#endif
