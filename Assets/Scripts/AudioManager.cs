using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    float masterVolumePercent = 0.2f;
    float sfxVolumePercent = 1;
    float musicVolumePercent = 1;

    AudioSource[] musicSources;
    int activeMusicSourceIndex;

    public static AudioManager instance;

    Transform audioListener;
    Transform playerT;

    void Awake () {
        instance = this;
        musicSources = new AudioSource[2];
        for (int i = 0; i < musicSources.Length; i++) {
            GameObject newMusicSource = new GameObject ("Music Source " + (i + 1));
            musicSources[i] = newMusicSource.AddComponent<AudioSource> ();
            newMusicSource.transform.parent = transform;
        }

        audioListener = FindObjectOfType<AudioListener> ().transform;
        playerT = FindObjectOfType<Player> ().transform;
    }

    void Update () {
        if (playerT != null) {
            audioListener.position = playerT.position;
        }
    }

    public void PlayMusic (AudioClip clip, float fadeDuration = 1) {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].Play ();
        StartCoroutine (AnimateMusicCrossFade (fadeDuration));
    }

    public void PlaySound (AudioClip clip, Vector3 position) {
        if (clip != null) {
            AudioSource.PlayClipAtPoint (clip, position, sfxVolumePercent * masterVolumePercent);
        }
    }

    IEnumerator AnimateMusicCrossFade (float duration) {
        float percent = 0;
        while (percent < 1) {
            percent += Time.deltaTime * 1 / duration;
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp (0, musicVolumePercent * masterVolumePercent, percent);
            musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp (musicVolumePercent * masterVolumePercent, 0, percent);
            yield return null;
        }
    }
}