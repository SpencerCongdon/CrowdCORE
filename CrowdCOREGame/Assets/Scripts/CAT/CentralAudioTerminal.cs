using UnityEngine;
using System.Collections;

public class CentralAudioTerminal : Singleton<CentralAudioTerminal>
{
    Object[] myMusic;
    public AudioClip intro;
    public AudioClip rampUp;
    public AudioClip[] chunks;
    //public AudioClip[] breaks;

    private AudioClip currentClip;
    private AudioClip nextClip;

    public AudioSource[] audioChannel;
    private AudioSource currentChannel;
    private AudioSource nextChannel;
    private AudioSource previousChannel;

    //public bool loop = true;

    private float timer;
    private float currentClipLength;
    private float nextClipLength;

    private int iterator;
    public bool gameOver = false;

    public override void Awake()
    {
        currentChannel = audioChannel[0];
        currentChannel.loop = true;
        nextChannel = audioChannel[1];
        nextChannel.loop = true;

        currentChannel.clip = intro;
        currentClipLength = currentChannel.clip.length;
        currentClip = currentChannel.clip;

        nextChannel.clip = rampUp;
        nextClipLength = nextChannel.clip.length;
        nextClip = currentChannel.clip;

        base.Awake();
    }

    void Start()
    {
        //audio.Play();
        audioChannel[0].Play();
    }

    void Update()
    {
        if (chunks.Length > 0 && !gameOver)
        {
            // Increase timer with the time difference between this and the previous frame:
            timer += Time.deltaTime;

            if (timer >= currentClipLength * 2.0f)
            {
                currentChannel.Stop();
                nextChannel.Play();
                currentClipLength = nextClipLength;
                timer = 0;

                //JuggleChannels prev/current/next
                previousChannel = currentChannel;
                
                currentChannel = nextChannel;
                currentClip = nextClip;
                //currentAudioClipLength = nextAudioClipLength;
                
                nextChannel = previousChannel;
                SetNextClip();
                nextChannel.clip = nextClip;
                nextClipLength = nextClip.length;
            }
        }

        if (gameOver)
        {
            StopAllChannels();
            GameObject.Destroy(this.gameObject);
        }
    }

    void SetNextClip()
    {
        nextClip = chunks[Random.Range(0, chunks.Length)] as AudioClip;
        if (nextClip == currentClip)
        {
            SetNextClip();
        }
    }

    public void StopAllChannels()
    {
        currentChannel.Stop();
        nextChannel.Stop();
        if (previousChannel != null)
        {
            previousChannel.Stop();
        }
    }
}