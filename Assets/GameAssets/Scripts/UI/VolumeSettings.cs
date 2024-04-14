using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();
    private void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat(Constants.Audio.MusicVolumePP, Constants.Audio.DefaultMusicVolume);
        sfxSlider.value = PlayerPrefs.GetFloat(Constants.Audio.SFXVolumePP, Constants.Audio.DefaultSFXVolume);
        SetMusicVolume();
        SetSFXVolume();
    }
    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        eventBroker.Publish(this, new AudioEvents.ChangeMusicVolume(volume));

    }
    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        eventBroker.Publish(this, new AudioEvents.ChangeSFXVolume(volume));
    }

    public void PlayMusic()
    {
        eventBroker.Publish(this, new AudioEvents.PlayMusic(Constants.Audio.Music.Song1Cut));
    }
    public void PlaySFX()
    {
        eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.TestSound));
    }
}
