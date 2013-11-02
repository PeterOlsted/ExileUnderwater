using UnityEngine;
using System.Collections;

public class Spotlight : MonoBehaviour
{

    [SerializeField] private float _startAngle;
    [SerializeField] private float _targetAngle;
    [SerializeField] private AnimationCurve _targetCurve;
    [SerializeField] private float _duration;

    private Light light;
    private float startTime;
    private float startAngle;

    [SerializeField] private AudioClip[] Clips = new AudioClip[0];

    void OnEnable()
    {
        light = GetComponent<Light>();
        light.enabled = false;
        light.spotAngle = _startAngle;
        //Activate();
    }

    public void Activate()
    {
        light.enabled = true;
        startAngle = light.spotAngle;
        startTime = Time.time;
        StartCoroutine(LightAnimation());
        var source = GetComponent<AudioSource>();
        source.clip = Clips.RandomElement();
        source.Play();
    }

    IEnumerator LightAnimation()
    {
        float endTime = startTime + _duration;
        while (Time.time < endTime)
        {
            float stepPercentage = ((endTime - Time.time) / _duration);
            light.spotAngle = Mathf.Lerp(startAngle, _targetAngle, _targetCurve.Evaluate(stepPercentage));
            yield return null;
        }
    }
}
