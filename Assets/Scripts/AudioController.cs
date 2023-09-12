using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [Tooltip("AudioClip for when the ball hits the paddle")][SerializeField] private AudioClip hitPaddleAudio;
    [Tooltip("AudioClip for when the ball breaks a brick and hits a brick, respectively (requires two AudioClips)")][SerializeField] private List<AudioClip> hitBrickAudioList;
    [Tooltip("AudioClip for when the ball is destroyed")][SerializeField] private AudioClip destroyBallAudio;

    private AudioSource audioSource = null;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void BreakBrick()
    {
        audioSource.PlayOneShot(hitBrickAudioList[0], 0.7f);
    }
    
    public void HitBrick()
    {
        audioSource.PlayOneShot(hitBrickAudioList[1], 0.7f);
    }

    public void HitPaddle()
    {
        audioSource.PlayOneShot(hitPaddleAudio, 0.7f);
    }

    public void DestroyBall()
    {
        audioSource.PlayOneShot(destroyBallAudio, 0.7f);
    }
}
