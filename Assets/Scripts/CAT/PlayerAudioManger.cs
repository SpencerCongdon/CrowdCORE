using UnityEngine;
using System.Collections;

public class PlayerAudioManger : MonoBehaviour
{
    private AudioSource audio;
    private int counter = 0;
    
    public AudioClip bulletShootSound;
    public AudioClip shiftShipShapeSound;
    public AudioClip hitSound;

    void Start()
    {
        audio = this.gameObject.GetComponent<AudioSource>();
        if (audio == null)
        {
            Debug.LogError("Player Is Missing AUDIOSOURCE Component");
        }
    }

    public void PlayBulletSound()
    {        
        audio.PlayOneShot(bulletShootSound, 0.35f);
    }

    public void PlayShiftShapeSound()
    {
        audio.PlayOneShot(shiftShipShapeSound);
    }

    public void PlayerHitSound()
    {
        audio.PlayOneShot(hitSound);
    }
}
