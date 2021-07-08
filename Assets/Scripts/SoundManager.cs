using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static AudioClip playerHurtSound, playerJumpSound, playerShootSound, playerAttackSound, playerDashSound;
    static AudioSource audioSrc;
    // Start is called before the first frame update
    void Start()
    {
        playerHurtSound = GetParent("Scripts").Audio.Character.Load<AudioClip> ("playerHurt");
        playerJumpSound = Assets.Audio.Character.Load<AudioClip> ("playerJump");
        playerShootSound = Assets.Audio.Character.Load<AudioClip> ("playerShoot");
        playerAttackSound = Assets.Audio.Character.Load<AudioClip> ("playerAttack");
        playerDashSound = Assets.Audio.Character.Load<AudioClip> ("playerDash");

        audioSrc = GetComponent<AudioSource> ();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlaySound (string clip){
        switch (clip){
            case "playerHurt":
                audioSrc.PlayOneShot (playerHurtSound);
                break;
            case "playerJump":
                audioSrc.PlayOneShot (playerJumpSound);
                break;
            case "playerShoot":
                audioSrc.PlayOneShot (playerShootSound);
                break;
            case "playerAttack":
                audioSrc.PlayOneShot (playerAttackSound);
                break;
            case "playerDash":
                audioSrc.PlayOneShot (playerDashSound);
                break;
        }
    }
}
