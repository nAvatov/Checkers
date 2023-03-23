using UnityEngine;
using System.Collections.Generic;

public enum SoundTypes {
    move,
    attack
}

public class SoundController : MonoBehaviour
{
    [SerializeField] private List<AudioClip> _audioClipsSerialized;
    private static List<AudioClip> _audioClips;
    private static AudioSource _audioSource;

    private void Awake() {
        _audioSource = GetComponent<AudioSource>();
        _audioClips = _audioClipsSerialized;
    }

    public static void PlaySound(SoundTypes soundType) {
        _audioSource.PlayOneShot(_audioClips[(int)soundType]);
    }
}
