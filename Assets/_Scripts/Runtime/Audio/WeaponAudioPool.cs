using System.Collections.Generic;
using UnityEngine;

public static class WeaponAudioPool
{
    static readonly List<AudioSource> Pool = new();

    public static void PlayOneShot(
        AudioClip clip,
        Vector3 position,
        float volume = 1f,
        float pitch = 1f,
        float spatialBlend = 1f,
        float minDistance = 3f,
        float maxDistance = 80f)
    {
        if (clip == null)
            return;

        AudioSource source = GetSource();

        source.transform.position = position;
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.spatialBlend = spatialBlend;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.playOnAwake = false;

        source.Play();
    }

    static AudioSource GetSource()
    {
        foreach (AudioSource s in Pool)
        {
            if (s != null && !s.isPlaying)
                return s;
        }

        GameObject go = new GameObject("PooledWeaponAudio");
        Object.DontDestroyOnLoad(go);

        AudioSource src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;

        Pool.Add(src);
        return src;
    }
}