using System;
using UnityEngine;
using VContainer;

public class ExampleComponent : MonoBehaviour
{
    [Inject] private GlobalConfigs _globalConfigs;

    private void Awake()
    {
        DI.Inject(this);
        Debug.Log(_globalConfigs.ExampleConfig.title);
    }
}