using UnityEngine;
using System.Collections;

public class NodeTypeData : MonoBehaviour
{
    public bool RandomVolume = false;
    public float MinVolume = 1.0f;
    public float MaxVolume = 1.0f;

    public int SelectedArea = 0;

    public bool RandomizeDelay = false;
    public float InitialDelayMin = 0.0f;
    public float InitialDelayMax = 0.0f;

    public bool RandomPitch = false;
    public float MinPitch = 1.0f;
    public float MaxPitch = 1.0f;

    public bool Loop = false;
    public bool LoopInfinite = false;
    public bool RandomizeLoops = false;
    public byte MinIterations = 0;
    public byte MaxIterations = 0;

    public bool OverrideAttenuation = false;
    public float MinDistance = 1;
    public float MaxDistance = 500;
    public AudioRolloffMode RolloffMode = AudioRolloffMode.Logarithmic;
    public AnimationCurve FalloffCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.1f, 1), new Keyframe(1,0));

    /*public RandomType Random;

    public enum RandomType {
        Shuffle, Standard
    }*/
}
 
