using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShakeCamera : MonoBehaviour 
{
    Vector3 originalPosition;
    private bool isShaking;
    [SerializeField] private Slider SFXSlider;

    private void Start()
    {
        originalPosition = transform.localPosition;
    }
    public void ShakeeCamera(float duration, float severity, bool vertical, bool horizontal)
    {
        if (isShaking) { return; }
        float volume = SFXSlider.value;
        StartCoroutine(Shake(duration, severity, vertical, horizontal, volume));
    }
    private IEnumerator Shake(float duration, float severity, bool vertical, bool horizontal, float volume)
    {
        originalPosition = transform.localPosition;

        isShaking = true;

        while (duration > 0)
        {
            Vector3 shakeOffset = Vector3.zero;

            if (horizontal)
            {
                shakeOffset.x = Random.Range(-.5f-volume*2, .5f+volume*2) * severity;
            }

            if (vertical)
            {
                shakeOffset.y = Random.Range(-.5f - volume*2, .5f + volume*2) * severity;
            }

            transform.localPosition = originalPosition + shakeOffset;
            duration -= Time.deltaTime;

            yield return null;
        }
        transform.localPosition = originalPosition;
        isShaking = false;
    }
}