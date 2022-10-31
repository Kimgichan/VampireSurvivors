using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private int bgmVolume;
    [SerializeField] private int sfxVolume;

    [SerializeField] private List<AudioClip> sfxList;
    [SerializeField] private List<AudioClip> bgmList;

    private Dictionary<string, AudioSource> sfxAudioTable;
    [SerializeField] private AudioSource bgmAudio;

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

    private IEnumerator Start()
    {
        while (GameManager.Instance == null)
            yield return null;

        if(GameManager.Instance.audioController != null)
        {
            Destroy(gameObject);
            yield break;
        }

        GameManager.Instance.audioController = this;

        sfxAudioTable = new Dictionary<string, AudioSource>();

        for(int i = 0, icount = sfxList.Count; i<icount; i++)
        {
            GameObject sfxObj = new GameObject($"sfxObj_{i}");
            var audio = sfxObj.AddComponent<AudioSource>();
            audio.clip = sfxList[i];
            audio.playOnAwake = false;
            sfxAudioTable.Add(sfxList[i].name, audio);

            sfxObj.transform.parent = transform;
            sfxObj.transform.localPosition = Vector3.zero;
        }

        PlayBGM("Cristian R. Aguiar - Dancing in the South");
    }

    public void PlaySFX(string name)
    {
        if(sfxAudioTable.TryGetValue(name, out AudioSource audio))
        {
            audio.volume = (float)SFX_Volume * 0.01f;
            audio.Play();
        }
    }

    public void PlayBGM(string name)
    {
        var clip = bgmList.Find(f => f.name.Equals(name));
        if(clip != null)
        {
            bgmAudio.clip = clip;
            bgmAudio.Play();
        }
    }

    private void Update()
    {
        bgmAudio.volume = (float)BGM_Volume * 0.01f;
    }
}
