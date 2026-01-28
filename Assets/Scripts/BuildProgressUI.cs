using UnityEngine;
using UnityEngine.UI;

public class BuildProgressUI : MonoBehaviour
{
    public Slider slider;

    public void Init(float duration)
    {
        slider.maxValue = duration;
        slider.value = 0;
    }

    public void SetProgress(float value)
    {
        slider.value = value;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                             Camera.main.transform.rotation * Vector3.up);
    }
}
