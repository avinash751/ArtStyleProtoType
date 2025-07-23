using UnityEngine;
using Game_Manager.Events;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;

namespace Game_Manager
{
    [AddComponentMenu("Game Manager System/GameStateAudioController")]
    public class GameStateAudioController : MonoBehaviour
    {
        /*[SerializeField] List<GameStateAudio> gameStatesAudioControlsList = new List<GameStateAudio>();
        List<Action> soundActions = new List<Action>();

        [System.Serializable]
        public class GameStateAudio
        {
            [SerializeField][HideInInspector] public string Name;
            public GameStateEvent StateEventType;
            public SoundOptions SoundToPlay;
            public List<SoundMixerControl> soundMixerControls = new List<SoundMixerControl>();
        }

        [System.Serializable]
        public class SoundMixerControl
        {
            [SerializeField][HideInInspector] public string Name;
            public AudioMixerGroup AudioMixerGroup;
            public string VolumeProperty;
            [Range(-80, 20)] public float TargetVolume;

            public void SetVolume(float volume)
            {
                AudioMixerGroup.audioMixer.SetFloat(VolumeProperty, volume);
            }
        }

        private void OnValidate()
        {
            for (int i = 0; i < gameStatesAudioControlsList.Count; i++)
            {
                GameStateAudio gameStateAudio = gameStatesAudioControlsList[i];
                gameStateAudio.Name = i + ". " + gameStateAudio.StateEventType + " Sound Control";
                foreach (SoundMixerControl soundMixerControl in gameStatesAudioControlsList[i].soundMixerControls)
                {
                    if (soundMixerControl.AudioMixerGroup != null)
                    {
                        soundMixerControl.Name = soundMixerControl.AudioMixerGroup.name + " Volume Control";
                    }
                }
            }
        }

        private void OnEnable()
        {
            soundActions.Clear();

            for (int i = 0; i < gameStatesAudioControlsList.Count; i++)
            {
                // *** CAPTURE HERE ***
                // Create a local copy of the loop variable's CURRENT value
                // This is necessary to avoid the closure problem in C#
                // This is done so when the event is raised, it uses the correct index
                int capturedIndex = i;

                Action soundAction = () =>
                {
                    CreateSoundCommand(capturedIndex);
                };

                soundActions.Add(soundAction);

                if (gameStatesAudioControlsList[i] != null)
                {
                    GameManagerEventBus.Subscribe(gameStatesAudioControlsList[i].StateEventType, soundAction);
                }

            }
        }

        private void CreateSoundCommand(int index)
        {
            if (index < 0 || index >= gameStatesAudioControlsList.Count || gameStatesAudioControlsList[index] == null)
            {
                Debug.LogError($"SoundCommand received invalid index ({index}) or null list entry.");
                return;
            }

            GameStateAudio gameStateAudio = gameStatesAudioControlsList[index];
            if (gameStateAudio.SoundToPlay != null)
            {
                SoundOptions soundToPlayForAction = gameStateAudio.SoundToPlay;
            }

            if (gameStateAudio.soundMixerControls == null)
            {
                return;
            }

            for (int j = 0; j < gameStateAudio.soundMixerControls.Count; j++)
            {
                if (j < gameStateAudio.soundMixerControls.Count && gameStateAudio.soundMixerControls[j] != null)
                {
                    SoundMixerControl soundMixerControl = gameStateAudio.soundMixerControls[j];
                    if (soundMixerControl.AudioMixerGroup != null && !string.IsNullOrEmpty(soundMixerControl.VolumeProperty))
                    {
                        soundMixerControl.SetVolume(soundMixerControl.TargetVolume);
                    }
                    else
                    {
                        Debug.LogWarning($"Invalid SoundMixerControl config at index j={j} for state index i={index}.");
                    }
                }
            }
        }
        private void OnDisable()
        {
            for (int i = 0; i < gameStatesAudioControlsList.Count && i < soundActions.Count; i++)
            {
                if (gameStatesAudioControlsList[i] != null && soundActions[i] != null)
                {
                    GameManagerEventBus.Unsubscribe(gameStatesAudioControlsList[i].StateEventType, soundActions[i]);
                }
            }
            soundActions.Clear();
        }*/
    }     
}