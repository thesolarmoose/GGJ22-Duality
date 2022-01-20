using UnityEngine;

namespace Audio
{
    [CreateAssetMenu(fileName = "GameSounds", menuName = "Audio/GameSounds", order = 0)]
    public class GameSounds : ScriptableObject
    {
        private static GameSounds _instance;

        public static GameSounds Instance => _instance;

        public AudioClip puzzleCablesPlugIn;
        public AudioClip puzzleCablesPlugOut;
        
        void OnEnable()
        {
            if (_instance != null) return;
            _instance = this;
        }
    }
}