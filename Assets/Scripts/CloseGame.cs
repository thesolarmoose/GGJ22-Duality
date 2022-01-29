using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CloseGame : MonoBehaviour
{
    [SerializeField] private InputAction closeButton;

    private void Start()
    {
        closeButton.Enable();
        closeButton.performed += context => Application.Quit();
    }

    private void OnEnable()
    {
        closeButton?.Enable();
    }

    private void OnDisable()
    {
        closeButton?.Disable();
    }
}