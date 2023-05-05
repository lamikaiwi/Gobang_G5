using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSetting : MonoBehaviour
{
    public AudioSource AS;
    public Slider slider;

    public void SetVolumeFromSlider() {
        float vol = slider.value;

        AS.volume = vol;




    }
}
