using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : StaticInstance<AudioManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXSource;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Audio Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;

    [Header("Level Settings")]
    public bool isInMainMenu = false;

    private byte currentTrackIndex = 0;
    private float backgroundTrackTime = 0f;
    private Coroutine returnCoroutine;


    [Header("============== AudioClips ==============")]
    public AudioClip[] bgMusic;
    public AudioClip mainMenuMusic;
    public AudioClip[] footsteps;
    public AudioClip[] spearImpacts;

    private void Start()
    {
        musicSource.clip = isInMainMenu ? mainMenuMusic : bgMusic[Random.Range(0, bgMusic.Length)];
        if (bgMusic.Length > 0) PlayCurrentBackgroundTrack();
        musicSource?.Play();


        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume();
        }
    }

    private void Update()
    {
        if (!musicSource.isPlaying && musicSource.clip == bgMusic[currentTrackIndex])
        {
            NextBackgroundTrack();
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || bgMusic.Length == 0) return;

        backgroundTrackTime = musicSource.time;

        if (musicSource.clip == bgMusic[currentTrackIndex])
        {
            backgroundTrackTime = musicSource.time;
        }

        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
        }

        musicSource.clip = clip;
        musicSource.Play();

        returnCoroutine = StartCoroutine(ReturnToBackgroundAfter(clip.length));
    }

    private IEnumerator ReturnToBackgroundAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayBackgroundTrackAt(currentTrackIndex, backgroundTrackTime);
    }

    private void PlayCurrentBackgroundTrack()
    {
        PlayBackgroundTrackAt(currentTrackIndex, 0f);
    }

    private void PlayBackgroundTrackAt(int index, float startTime)
    {
        if (index < 0 || index >= bgMusic.Length) return;

        musicSource.clip = bgMusic[index];
        musicSource.time = startTime;
        musicSource.Play();
    }

    private void NextBackgroundTrack()
    {
        currentTrackIndex = (byte)((currentTrackIndex + 1) % bgMusic.Length);
        PlayCurrentBackgroundTrack();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
        SFXSource.pitch = Random.Range(.9f, 1.08f);
        SFXSource.priority = 128;
    }

    public void PlaySFXArray(AudioClip[] sfxArray)
    {
        PlaySFX(sfxArray[Random.Range(0, sfxArray.Length)]);
    }

    public bool SFXSourceIsPlaying() { return SFXSource.isPlaying; }

    #region Volume Settings

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        audioMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }


    private void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume", .25f);
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume", .4f);

        SetMusicVolume();
        SetSFXVolume();
    }

    #endregion
}