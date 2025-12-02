using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header(" Audio Source ")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    
    [Header(" Audio Clips ")]
    public AudioClip background;
    public AudioClip fight;
    public AudioClip shoot;
    public AudioClip hurt;
    public AudioClip eat;
    public AudioClip dash;
    



    private void Start()
    {
       
         
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.PlayOneShot(clip);
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }


}
