using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class LipsyncToAnim : MonoBehaviour
{
    [Header("Required Assets")]
    [Tooltip("The lipsync file to use")]
    public OVRLipSyncSequence ovrLipSyncSequence;
    [Tooltip("The animation clip to modify (contents will be preserved)")]
    public AnimationClip animationClip;

    [Header("Animation Tweaks")]
    [Tooltip("Mouth Oject Path")]
    public string mouthPath = "";

    [Header("Viseme Names")]
    [Tooltip("This will be placed before every viseme name")]
    public string prefix = "V.";
    public string[] visemeNames = Enum.GetNames(typeof(OVRLipSync.Viseme));

    [Header("Viseme Tweaks")]
    [Tooltip("Min Value to have")]
    public float minValue = 0;
    [Tooltip("How Much To Multiply Blendshapes By")]
    public float gain = 100;

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
        Debug.Log("Beginning Conversion...");

        float fps = ovrLipSyncSequence.entries.Count/ovrLipSyncSequence.length;

        string path = mouthPath ?? "";
        float viseme = 0;

        EditorCurveBinding[] editorCurves = new EditorCurveBinding[15];
        AnimationCurve[] animationCurves = new AnimationCurve[15];

        for(int i = 0; i < 15; i++)
        {
            editorCurves[i] = EditorCurveBinding.FloatCurve(
                path,
                typeof(SkinnedMeshRenderer),
                "blendShape." + prefix + visemeNames[i]
                );

            animationCurves[i] = new AnimationCurve();
        }

        foreach (OVRLipSync.Frame frame in ovrLipSyncSequence.entries)
        {
            float time = frame.frameNumber/fps;

            for (int i = 0; i < 15; i++)
            {
                viseme = Mathf.Clamp(frame.Visemes[i] * gain, 0, 100);

                if (viseme < minValue)
                {
                    viseme = 0;
                }

                AnimationCurve curve = animationCurves[i];
                curve.AddKey(time, viseme);
            }
        }

        for(int i = 0; i < 15; i++)
        {
            AnimationUtility.SetEditorCurve(animationClip, editorCurves[i], animationCurves[i]);
        }

        Debug.Log("Conversion complete!");
    }

    private static string GetGameObjectPath(Transform transform)
    {
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }
}
#endif
