using System.Collections;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    private readonly AudioSource defaultMusicSource;

    public AudioClip[] SFXClips;
    public AudioClip[] AnnouncerClips;
    private bool isAnnouncerPlaying = false;

    public void PlaySFX(AudioClip clip, AudioSource customSource = null)
    {
        if (clip == null)
        {
            return;
        }

        AudioSource sourceToUse = customSource ?? CreateAudioSource();

        sourceToUse.clip = clip;
        sourceToUse.Play();

        if (customSource == null)
        {
            Destroy(sourceToUse.gameObject, clip.length);
        }
    }

    public void PlaySFX(string clipName, AudioSource customSource = null)
    {
        AudioClip clipToPlay = FindClipByName(clipName, SFXClips);
        PlaySFX(clipToPlay, customSource);
    }

    public void PlayAnnouncer(string clipName)
    {
        // Announcer should only play if it is not playing already
        if (isAnnouncerPlaying)
        {
            return;
        }

        AudioClip clipToPlay = FindClipByName(clipName, AnnouncerClips);
        PlaySFX(clipToPlay);
        isAnnouncerPlaying = true;
        StartCoroutine(SetAnnouncerPlayingFalse(clipToPlay.length));
    }

    public AudioSource PlayAndLoop(AudioClip clip, AudioSource customSource = null)
    {
        if (clip == null)
        {
            return null;
        }

        AudioSource sourceToUse = customSource ?? CreateAudioSource();

        sourceToUse.clip = clip;
        sourceToUse.loop = true;
        sourceToUse.Play();

        return sourceToUse;
    }

    public void PlayMusic(AudioClip clip, bool loop = true, AudioSource customSource = null)
    {
        if (clip == null)
        {
            return;
        }

        AudioSource sourceToUse = customSource ?? defaultMusicSource;

        sourceToUse.loop = loop;
        sourceToUse.clip = clip;
        sourceToUse.Play();
    }

    public void PlayMusic(string clipName, bool loop = true, AudioSource customSource = null)
    {
        AudioClip clipToPlay = FindClipByName(clipName, SFXClips);
        PlayMusic(clipToPlay, loop, customSource);
    }

    public void StopMusic(AudioSource customSource = null)
    {
        AudioSource sourceToUse = customSource ?? defaultMusicSource;
        sourceToUse.Stop();
    }

    public void SetMusicVolume(float volume, AudioSource customSource = null)
    {
        AudioSource sourceToUse = customSource ?? defaultMusicSource;
        sourceToUse.volume = volume;
    }

    private AudioClip FindClipByName(string name, AudioClip[] clips)
    {
        foreach (AudioClip clip in clips)
        {
            if (clip.name == name)
            {
                return clip;
            }
        }
        Debug.LogWarning("Clip not found: " + name);
        return null;
    }

    private AudioSource CreateAudioSource()
    {
        GameObject audioObject = new GameObject("SFX");
        audioObject.transform.parent = transform;
        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        return audioSource;
    }

    IEnumerator StopAudioSource(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.Stop();
    }

    IEnumerator SetAnnouncerPlayingFalse(float delay)
    {
        yield return new WaitForSeconds(delay);
        isAnnouncerPlaying = false;
    }
}
