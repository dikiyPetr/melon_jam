using UnityEngine;


public class GlobalConfigs : MonoBehaviour
{
    [Header("Configurations")] [SerializeField]
    private ExampleConfig exampleConfig;

    public ExampleConfig ExampleConfig => exampleConfig;
}