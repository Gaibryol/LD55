using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioSystem : MonoBehaviour
{
	[SerializeField] private AudioSource musicSource;
	[SerializeField] private AudioSource sfxSource;

	[SerializeField, Header("Music")] private AudioClip song1;
	[SerializeField] private AudioClip song1Cut;
	[SerializeField] private AudioClip mainMenuTheme;
	[SerializeField] private AudioClip song2;
	[SerializeField] private AudioClip song3;
	[SerializeField] private AudioClip song1Preview;
	[SerializeField] private AudioClip song2Preview;
	[SerializeField] private AudioClip song3Preview;

	[SerializeField, Header("SFX")]private AudioClip testSound;
	[SerializeField] private AudioClip book;
	[SerializeField] private AudioClip buttonClick;
	[SerializeField] private AudioClip death;

	private float musicVolume;
	private float sfxVolume;

	private AudioClip oldMusic;
	private float oldTime;

	private Dictionary<string, AudioClip> music = new Dictionary<string, AudioClip>();
	private Dictionary<string, AudioClip> sfx = new Dictionary<string, AudioClip>();

	private EventBrokerComponent eventBrokerComponent = new EventBrokerComponent();

	private Coroutine playPauseCo;
	private Coroutine playMusicFadeCoroutine;

	private void Awake()
	{
		// Set up music and sfx dictionaries
		music.Add(Constants.Audio.Music.Song1, song1);
		music.Add(Constants.Audio.Music.Song1Cut, song1Cut);
		music.Add(Constants.Audio.Music.MainMenuTheme, mainMenuTheme);
		music.Add(Constants.Audio.Music.Song2, song2);
		music.Add(Constants.Audio.Music.Song3, song3);
		music.Add(Constants.Audio.Music.Song1Preview, song1Preview);
		music.Add(Constants.Audio.Music.Song2Preview, song2Preview);
		music.Add(Constants.Audio.Music.Song3Preview, song3Preview);

		sfx.Add(Constants.Audio.SFX.TestSound, testSound);
		sfx.Add(Constants.Audio.SFX.BookFlip, book);
		sfx.Add(Constants.Audio.SFX.ButtonClick, buttonClick);
		sfx.Add(Constants.Audio.SFX.Death, testSound);

	}

	private void OnEnable()
	{
		eventBrokerComponent.Subscribe<AudioEvents.PlayMusic>(PlayMusicHandler);
		eventBrokerComponent.Subscribe<AudioEvents.PlaySFX>(PlaySFXHandler);
		eventBrokerComponent.Subscribe<AudioEvents.ChangeMusicVolume>(ChangeMusicVolumeHandler);
		eventBrokerComponent.Subscribe<AudioEvents.ChangeSFXVolume>(ChangeSFXVolumeHandler);
		eventBrokerComponent.Subscribe<AudioEvents.PlayTemporaryMusic>(PlayTemporaryMusicHandler);
		eventBrokerComponent.Subscribe<AudioEvents.StopTemporaryMusic>(StopTemporaryMusicHandler);
		eventBrokerComponent.Subscribe<AudioEvents.GetSongLength>(GetSongLengthHandler);
		eventBrokerComponent.Subscribe<AudioEvents.StopMusic>(StopMusicHandler);
		eventBrokerComponent.Subscribe<SongEvents.SongEnded>(SongEnded);

		float musicLevel = PlayerPrefs.GetFloat(Constants.Audio.MusicVolumePP, Constants.Audio.DefaultMusicVolume);
		float sfxLevel = PlayerPrefs.GetFloat(Constants.Audio.SFXVolumePP, Constants.Audio.DefaultSFXVolume);

		musicVolume = musicLevel;
		sfxVolume = sfxLevel;
		musicSource.volume = musicLevel;
		sfxSource.volume = sfxLevel;
	}

	private void OnDisable()
	{
		eventBrokerComponent.Unsubscribe<AudioEvents.PlayMusic>(PlayMusicHandler);
		eventBrokerComponent.Unsubscribe<AudioEvents.PlaySFX>(PlaySFXHandler);
		eventBrokerComponent.Unsubscribe<AudioEvents.ChangeMusicVolume>(ChangeMusicVolumeHandler);
		eventBrokerComponent.Unsubscribe<AudioEvents.ChangeSFXVolume>(ChangeSFXVolumeHandler);
		eventBrokerComponent.Unsubscribe<AudioEvents.PlayTemporaryMusic>(PlayTemporaryMusicHandler);
		eventBrokerComponent.Unsubscribe<AudioEvents.StopTemporaryMusic>(StopTemporaryMusicHandler);
		eventBrokerComponent.Unsubscribe<AudioEvents.GetSongLength>(GetSongLengthHandler);
		eventBrokerComponent.Unsubscribe<AudioEvents.StopMusic>(StopMusicHandler);
		eventBrokerComponent.Unsubscribe<SongEvents.SongEnded>(SongEnded);
	}

	public void PlayButtonClickSFX()
	{
		if (sfx.ContainsKey(Constants.Audio.SFX.ButtonClick))
		{
			sfxSource.PlayOneShot(sfx[Constants.Audio.SFX.ButtonClick]);
		}
		else
		{
			Debug.LogError("Cannot find sfx named " + Constants.Audio.SFX.ButtonClick);
		}
	}

	private void ChangeMusicVolumeHandler(BrokerEvent<AudioEvents.ChangeMusicVolume> inEvent)
	{
		musicVolume = inEvent.Payload.NewVolume;
		musicSource.volume = musicVolume;

		PlayerPrefs.SetFloat(Constants.Audio.MusicVolumePP, musicVolume);
	}

	private void ChangeSFXVolumeHandler(BrokerEvent<AudioEvents.ChangeSFXVolume> inEvent)
	{
		sfxVolume = inEvent.Payload.NewVolume;
		sfxSource.volume = sfxVolume;

		PlayerPrefs.SetFloat(Constants.Audio.SFXVolumePP, sfxVolume);
	}

	private void PlayMusicHandler(BrokerEvent<AudioEvents.PlayMusic> inEvent)
	{
		if (playMusicFadeCoroutine != null)
		{
			StopCoroutine(playMusicFadeCoroutine);
		}

		if (inEvent.Payload.Transition)
		{
			playMusicFadeCoroutine = StartCoroutine(FadeToSong(inEvent.Payload.MusicName));
		}
		else
		{
			PlayMusic(inEvent.Payload.MusicName);
		}
	}

	private void PlaySFXHandler(BrokerEvent<AudioEvents.PlaySFX> inEvent)
	{
		if (sfx.ContainsKey(inEvent.Payload.SFXName))
		{
			sfxSource.PlayOneShot(sfx[inEvent.Payload.SFXName]);
		}
		else
		{
			Debug.LogError("Cannot find sfx named " + inEvent.Payload.SFXName);
		}
	}

	private void PlayTemporaryMusicHandler(BrokerEvent<AudioEvents.PlayTemporaryMusic> inEvent)
	{
		oldMusic = musicSource.clip;
		oldTime = musicSource.time;
		StartCoroutine(FadeToSong(inEvent.Payload.MusicName));
	}

	private void StopTemporaryMusicHandler(BrokerEvent<AudioEvents.StopTemporaryMusic> inEvent)
	{
		StartCoroutine(FadeToSong(oldMusic, oldTime));
	}

	private void GetSongLengthHandler(BrokerEvent<AudioEvents.GetSongLength> inEvent)
	{
		if (music.ContainsKey(inEvent.Payload.Title))
		{
			inEvent.Payload.ProcessData.DynamicInvoke(music[inEvent.Payload.Title].length);
		}
		else
		{
			Debug.LogError("Cannot find music named " + inEvent.Payload.Title);
		}
	}

	private void StopMusicHandler(BrokerEvent<AudioEvents.StopMusic> inEvent)
	{
		musicSource.Stop();
	}

	private void SongEnded(BrokerEvent<SongEvents.SongEnded> inEvent)
	{
		musicSource.Stop();
	}

	private void PlayMusic(string song, float time = 0f)
	{
		if (music.ContainsKey(song))
		{
			musicSource.Stop();
			musicSource.clip = music[song];
			musicSource.loop = false;
			musicSource.Play();
			musicSource.time = time;
		}
		else
		{
			Debug.LogError("Cannot find music named " + song);
		}
	}

	private void PlayMusic(AudioClip song, float time = 0f)
	{
		musicSource.Stop();
		musicSource.clip = song;
		musicSource.loop = false;
		musicSource.Play();
		musicSource.time = time;
	}

	private IEnumerator FadeToSong(string song, float time = 0f)
	{
		while (musicSource.volume > 0)
		{
			musicSource.volume -= Constants.Audio.MusicFadeSpeed * Time.deltaTime;
			yield return null;
		}

		PlayMusic(song, time);

		while (musicSource.volume < musicVolume)
		{
			musicSource.volume += Constants.Audio.MusicFadeSpeed * Time.deltaTime;
			yield return null;
		}
	}

	private IEnumerator FadeToSong(AudioClip song, float time = 0f)
	{
		while (musicSource.volume > 0)
		{
			musicSource.volume -= Constants.Audio.MusicFadeSpeed * Time.deltaTime;
			yield return null;
		}

		PlayMusic(song, time);

		while (musicSource.volume < musicVolume)
		{
			musicSource.volume += Constants.Audio.MusicFadeSpeed * Time.deltaTime;
			yield return null;
		}
	}
}