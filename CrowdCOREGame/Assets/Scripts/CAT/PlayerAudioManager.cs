using UnityEngine;
using System.Collections;

public class PlayerAudioManager : MonoBehaviour
{
    private AudioSource source;
    
    public AudioClip bulletShootSound;
    public AudioClip shiftShipShapeSound;
    public AudioClip hitSound;

    void Start()
    {
        source = this.gameObject.GetComponent<AudioSource>();
        if (source == null)
        {
            GameLog.LogError("Player Is Missing AUDIOSOURCE Component", GameLog.Category.Audio);
        }
    }

    public void PlayBulletSound()
    {        
        source.PlayOneShot(bulletShootSound, 0.35f);
    }

    public void PlayShiftShapeSound()
    {
        source.PlayOneShot(shiftShipShapeSound);
    }

    public void PlayerHitSound()
    {
        source.PlayOneShot(hitSound);
    }
}
