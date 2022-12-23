using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class AudioManage : MonoBehaviour
{
    public AudioSource source;
    public AudioClip click;
    public AudioClip jump;
    public AudioClip slide;

    public void Click()
    {
        source.PlayOneShot(click);
    }
    public void Jump()
    {
        source.PlayOneShot(jump);
    }
    public void Slide()
    {
        source.PlayOneShot(slide);
    }
}
