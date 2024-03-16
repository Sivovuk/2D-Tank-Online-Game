using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameSelector : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameField;
    [SerializeField] private Button _connectBtn;
    [SerializeField] private int _minNameLenght = 1;
    [SerializeField] private int _maxNameLenght = 12;

    private const string PlayerNameKey = "PlayerName";
    
    private void Start()
    {
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
            return;
        }

        _nameField.text = PlayerPrefs.GetString(PlayerNameKey, String.Empty);
        HandleNameChange();
    }

    public void HandleNameChange()
    {
        _connectBtn.interactable = _nameField.text.Length >= _minNameLenght && _nameField.text.Length <= _maxNameLenght;
    }

    public void Connect()
    {
        PlayerPrefs.SetString(PlayerNameKey, _nameField.text);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
}
