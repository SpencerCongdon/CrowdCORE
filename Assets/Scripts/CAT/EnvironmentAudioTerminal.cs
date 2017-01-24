using UnityEngine;
using System.Collections;

public class EnvironmentAudioTerminal : MonoBehaviour
{
    public AudioSource[] audioSource;
    public AudioClip[] audioClip;
    public bool gameOver = false;

    private AudioSource nextSource;
    private AudioClip nextClip;

    private float timer;
    private float resetTimer = 0.0f;
    private float randomSoundTimer = 5.0f;

    void Awake()
    {
    }

    void Start()
    {
    }

    void Update()
    {
        if (audioSource.Length > 0 && audioClip.Length > 0 && !gameOver)
        {
            // Increase timer with the time difference between this and the previous frame:
            timer += Time.deltaTime;

            if (timer >= randomSoundTimer)
            {
                PlayItAgainSam();
                randomSoundTimer = ResetRandomTimer();
                timer = resetTimer;
            }
        }

        if (gameOver)
        {
            StopAllChannels();
            GameObject.Destroy(this.gameObject);
        }
    }

    void PlayItAgainSam()
    {
        nextSource = SetAudioSource();
        if (!nextSource.isPlaying)
        {
            nextSource.clip = SetAudioClip();
            nextSource.Play();
        }
    }

    AudioClip SetAudioClip()
    {
        AudioClip nextClip = audioClip[Random.Range(0, audioClip.Length)] as AudioClip;
        return nextClip;
    }

    AudioSource SetAudioSource()
    {
        AudioSource nextSource = audioSource[Random.Range(0, audioSource.Length)] as AudioSource;
        return nextSource;
    }

    float ResetRandomTimer()
    {
        float newTime = (float)Random.Range(8, 16);
        Debug.Log(newTime);
        return newTime;
    }

    public void StopAllChannels()
    {
        foreach(AudioSource audio in audioSource)
        {
            if (audio.isPlaying)
            {
                audio.Stop();
            }
        }
    }
}