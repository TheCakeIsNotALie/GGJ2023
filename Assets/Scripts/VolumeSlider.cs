using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private AudioClip testClip;
    // Start is called before the first frame update
    void Start()
    {
        slider.onValueChanged.AddListener(val => {
            print(testClip);
            SoundManager.instance.ChangeMasterVolume(val);
            SoundManager.instance.PlaySound(testClip);
            }
        );
    }
}
