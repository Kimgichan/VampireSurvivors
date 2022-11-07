using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public AudioController audioController;

    [SerializeField] private int bgmVolume;
    [SerializeField] private int sfxVolume;

    public int BGM_Volume
    {
        get
        {
            if (bgmVolume < 0) return 0;
            if (bgmVolume > 100) return 100;
            return bgmVolume;
        }

        set
        {
            if (value < 0) value = 0;
            else if (value > 100) value = 100;
            bgmVolume = value;
        }
    }

    public int SFX_Volume
    {
        get
        {
            if (sfxVolume < 0) return 0;
            if (sfxVolume > 100) return 100;
            return sfxVolume;
        }

        set
        {
            if (value < 0) value = 0;
            else if (value > 100) value = 100;
            sfxVolume = value;
        }
    }

    public static AudioManager Instance => instance;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static AudioController GetAudioController()
    {
        if (Instance == null || Instance.audioController == null)
        {
            return null;
        }
        return Instance.audioController;
    }

    public void AMReset()
    {
        audioController = null;
    }
}
