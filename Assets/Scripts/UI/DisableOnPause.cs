﻿using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace UI
{
    public class DisableOnPause : MonoBehaviour
    {
        [SerializeField] private BoolVariable _pausedVariable;

        private bool _activeBeforeChanged;
        
        private void Start()
        {
            _pausedVariable.Changed.Register(OnPauseChange);
        }

        private void OnPauseChange(bool paused)
        {
            try  // try because game object could be destroyed
            {
                if (paused)
                {
                    _activeBeforeChanged = gameObject.activeSelf;
                    gameObject.SetActive(false);
                }
                else
                {
                    if (_activeBeforeChanged)
                    {
                        gameObject.SetActive(true);
                    }
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}