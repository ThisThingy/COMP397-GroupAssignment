using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private const string Key = "MASTER_VOLUME";

    private void Awake()
    {
        if (slider == null) slider = GetComponent<Slider>();

        float v = PlayerPrefs.GetFloat(Key, 1f);
        AudioListener.volume = v;

        slider.SetValueWithoutNotify(v);
        slider.onValueChanged.AddListener(OnChanged);
    }

    private void OnChanged(float v)
    {
        AudioListener.volume = v;
        PlayerPrefs.SetFloat(Key, v);
    }
}