using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nodes;

public class AudioController : MonoBehaviour
{
    [SerializeField] private List<NAudioClip> sfxList;
    [SerializeField] private List<NAudioClip> bgmList;

    private Dictionary<string, NAudioSource> sfxAudioTable;
    [SerializeField] private NAudioSource bgmAudio;

    private IEnumerator Start()
    {
        while (AudioManager.Instance == null)
            yield return null;

        if(AudioManager.Instance.audioController != null)
        {
            Destroy(gameObject);
            yield break;
        }

        AudioManager.Instance.audioController = this;

        sfxAudioTable = new Dictionary<string, NAudioSource>();

        for(int i = 0, icount = sfxList.Count; i<icount; i++)
        {
            GameObject sfxObj = new GameObject($"sfxObj_{i}");
            var audio = sfxObj.AddComponent<AudioSource>();
            audio.clip = sfxList[i].audioClip;
            audio.playOnAwake = false;
            sfxAudioTable.Add(sfxList[i].audioClip.name, new NAudioSource() { audioSource = audio,  soundClip = sfxList[i] });

            sfxObj.transform.parent = transform;
            sfxObj.transform.localPosition = Vector3.zero;
        }

        if (bgmList.Count > 0)
            PlayBGM(bgmList[0].audioClip.name);
    }

    public void PlaySFX(string name)
    {
        if(sfxAudioTable.TryGetValue(name, out NAudioSource audio))
        {
            if (AudioManager.Instance == null) return;
            audio.audioSource.volume = (float)AudioManager.Instance.SFX_Volume*0.01f * audio.soundClip.scale;
            audio.audioSource.clip = audio.soundClip.audioClip;
            audio.audioSource.Play();
        }
        else
        {
            Debug.LogWarning($"{name}은 없는 효과음입니다.");
        }
    }

    public void PlayBGM(string name)
    {
        var clip = bgmList.Find(f => f.audioClip.name.Equals(name));
        if(clip != null)
        {
            bgmAudio.soundClip = clip;
            bgmAudio.audioSource.volume = (float)AudioManager.Instance.BGM_Volume * 0.01f * clip.scale;
            bgmAudio.audioSource.clip = clip.audioClip;
            bgmAudio.audioSource.Play();
        }
    }

    private void Update()
    {
        if (AudioManager.Instance == null) return;
        bgmAudio.audioSource.volume = (float)AudioManager.Instance.BGM_Volume * 0.01f * bgmAudio.soundClip.scale;
    }
}
