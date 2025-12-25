using System.Collections.Generic;
using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    private GameMode _currentMode;
    [SerializeField] private GameMode _gameMode = GameMode.Playing;
    [SerializeField] private List<GameModeEntry> _modeSettings;


    private void Start()
    {
        SetMode(_gameMode);
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            SetMode(_gameMode);
        }
    }

    public void SetMode(GameMode newMode)
    {
        if (_currentMode != _gameMode)
        {
            _currentMode = newMode;
            ApplyModeSettings(newMode);
        }
    }

    private void ApplyModeSettings(GameMode newMode)
    {
        if (_modeSettings == null)
            return;

        var componentsToEnable = new HashSet<GameObject>();

        foreach (var entry in _modeSettings)
        {
            if (entry.Mode == newMode && entry.gameObjects != null)
            {
                foreach (var gameObject in entry.gameObjects)
                {
                    if (gameObject != null)
                    {
                        componentsToEnable.Add(gameObject);
                    }
                }
            }
        }

        foreach (var entry in _modeSettings)
        {
            foreach (var gameObject in entry.gameObjects)
            {
                if (gameObject != null)
                {
                    gameObject.SetActive(componentsToEnable.Contains(gameObject));
                }
            }
        }
    }
}