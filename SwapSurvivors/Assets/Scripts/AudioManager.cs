using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    AudioSource sfxSource;
    [SerializeField] List<AudioClip> enemyHurtSFXClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> playerStepSFXClips = new List<AudioClip>();   

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            sfxSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayEnemyHurtSFX()
    {
        AudioClip clip = enemyHurtSFXClips[Random.Range(0, enemyHurtSFXClips.Count)];
        sfxSource.pitch = Random.Range(0.8f, 1.2f);
        sfxSource.PlayOneShot(clip, 1f);
    }

    public void PlayPlayerStepSFX()
    {
        AudioClip clip = playerStepSFXClips[Random.Range(0, playerStepSFXClips.Count)];
        sfxSource.pitch = Random.Range(0.8f, 1.2f);
        sfxSource.PlayOneShot(clip, 0.35f);
    }
}
