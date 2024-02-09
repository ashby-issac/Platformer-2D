using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip levelStartClip;
    [SerializeField] AudioClip jumpClip;
    [SerializeField] AudioClip mainThemeClip;
    [SerializeField] AudioClip orbColletionClip;
    [SerializeField] AudioClip spikesClip;
    [SerializeField] AudioClip winClip;

    [SerializeField] private AudioSource audioSource1;
    [SerializeField] private AudioSource audioSource2;
    [SerializeField] private AudioSource audioSource3;

    public static AudioManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void PlayAudioClip(AudioSource audioSource, AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void LevelStartAudio()
    {
        PlayAudioClip(audioSource1, levelStartClip);
        PlayAudioClip(audioSource3, mainThemeClip);
    }

    public void PlayerJumpClip() => PlayAudioClip(audioSource2, jumpClip);
    public void MainThemeClip() => PlayAudioClip(audioSource2, mainThemeClip);
    public void OrbColletionClip() => PlayAudioClip(audioSource2, orbColletionClip);
    public void SpikesClip() => PlayAudioClip(audioSource2, spikesClip);
    public void WinClip()
    {
        PlayAudioClip(audioSource2, winClip);
        
    }
}
