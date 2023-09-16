using System;
using UnityEngine;

namespace RoyalFortune21.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [Serializable]
        public class Sound
        {
            public string name;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume;
            public bool loop;

            [HideInInspector] public AudioSource source;
        }
        
        [SerializeField] Sound[] StaticSounds;
        [SerializeField] Sound[] DynamicSounds;

        [HideInInspector] public string ActiveSound;

        public static AudioManager instance;

        bool MusicStatus = true;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            foreach (var S in StaticSounds)
            {
                S.source = gameObject.AddComponent<AudioSource>();
                S.source.clip = S.clip;
                S.source.volume = S.volume;
                S.source.loop = S.loop;
            }

            string MusicStatus = PlayerPrefs.GetString("MusicStatus", "ON");
            if (MusicStatus == "OFF")
            {
                SetMusicStatus(false);
            }
            else if (MusicStatus == "ON")
            {
                SetMusicStatus(true);
            }
        }

        public void SetMusicStatus(bool status) => MusicStatus = status;

        public void Play(string name)
        {
            if (!MusicStatus)
                return;
            Sound s = Array.Find(StaticSounds, sound => sound.name == name);
            if (s == null)
            {
                Debug.LogWarning($"Sound {name} not found");
                return;
            }
            s.source.Play();
            ActiveSound = name;
        }

        public void Stop(string name)
        {
            if (!MusicStatus)
                return;
            Sound s = Array.Find(StaticSounds, sound => sound.name == name);
            if (s == null)
            {
                Debug.LogWarning($"Sound {name} not found");
                return;
            }
            s.source.Stop();
            ActiveSound = name;
        }

        public void StopActiveSound()
        {
            if (!MusicStatus)
                return;
            Sound s = Array.Find(StaticSounds, sound => sound.name == ActiveSound);
            if (s == null)
            {
                Debug.LogWarning($"Sound {ActiveSound} not found");
                return;
            }
            s.source.Stop();
        }

        public void PlayNewSound(string name, float destroyTime)
        {
            if (!MusicStatus)
                return;
            Sound s = Array.Find(DynamicSounds, sound => sound.name == name);
            if (s == null)
                return;
            s = AddAudioSource(s);
            s.source.Play();
            Destroy(s.source, destroyTime);
        }

        public void PlayNewSound(string name)
        {
            if (!MusicStatus)
                return;
            Sound s = Array.Find(DynamicSounds, sound => sound.name == name);
            if (s == null)
                return;
            s = AddAudioSource(s);
            s.source.Play();
            Destroy(s.source, s.source.clip.length);
        }

        Sound AddAudioSource(Sound s)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
            return s;
        }

        public void ChangeVolume(float _volume)
        {
            foreach(Sound sound in StaticSounds)
            {
                sound.source.volume = _volume;
            }
        }

        public void MuteSound()
        {
            foreach (Sound sound in StaticSounds)
            {
                sound.source.Stop();
                sound.source.volume = 0;
            }
        }

        public void UnMuteSoundFromMainMenu(float _volume)
        {
            foreach (Sound sound in StaticSounds)
            {
                sound.source.volume = _volume;
            }
            Play("main menu");
        }

    }
}
    