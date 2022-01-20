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
        
        [Space]
        
        public AudioClip puzzlePing0;
        public AudioClip puzzlePing1;
        
        void OnEnable()
        {
            if (_instance != null) return;
            _instance = this;
        }
    }
}