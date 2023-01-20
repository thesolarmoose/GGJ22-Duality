using System;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

namespace Audio
{
    [CreateAssetMenu(fileName = "GameSounds", menuName = "Audio/GameSounds", order = 0)]
    public class GameSounds : ScriptableObject
    {
        private static GameSounds _instance;

        public static GameSounds Instance => _instance;

        public AudioClip puzzle1CablesPlugIn;
        public AudioClip puzzle1CablesPlugOut;
        public AudioClip puzzle1Ping0;
        public AudioClip puzzle1Ping1;
        public AudioClip puzzle1DoorCloses;
        public AudioClip puzzle1DoorOpens;
        public AudioClip puzzle1Solved;

        [Space]
        
        public AudioClip puzzle2CutCable;
        public AudioClip puzzle2ConnectCable;
        public AudioClip puzzle2Weld;
        public AudioClip puzzle2Solved;

        [Space]
        
        public AudioClip puzzle3OpenDoor;
        public AudioClip puzzle3CloseDoor;

        void OnEnable()
        {
            if (_instance != null) return;
            _instance = this;
        }

        public void PlaySound(AudioClip clip)
        {
            if (clip)
            {
                AudioSource.PlayClipAtPoint(clip, Vector3.zero);
            }
        }
    }
}