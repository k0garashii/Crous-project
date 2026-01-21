using UnityEngine;
using UnityEngine.UI;

public class BuildProgressUI : MonoBehaviour
{
    public Slider slider;
    public Vector3 offset = new Vector3(0, 2f, 0);

    private Transform target;

    public void Init(Transform followTarget, float duration)
    {
        target = followTarget;
        slider.maxValue = duration;
        slider.value = 0;
    }

    public void SetProgress(float value)
    {
        slider.value = value;
    }

    void LateUpdate()
    {
        if (target)
            transform.position = target.position + offset;
    }
}
