using UnityEngine;
using VContainer;

public class AIMotivationController : MonoBehaviour
{
    [SerializeField] private AIMotivationConfig config;

    [Inject] private AIMotivationData _motivationData;

    public AIMotivationConfig Config => config;

    private void Awake()
    {
        DI.Inject(this);
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (config == null || _motivationData == null) return;

        _motivationData.Aggression.Name = config.Aggression.Name;
        _motivationData.Aggression.MaxValue = config.Aggression.MaxValue;
        _motivationData.Aggression.LowValueNodeType = config.Aggression.LowValueNodeType;
        _motivationData.Aggression.HighValueNodeType = config.Aggression.HighValueNodeType;
        _motivationData.Aggression.SetValue(config.Aggression.GetRandomStartValue());

        _motivationData.Wealth.Name = config.Wealth.Name;
        _motivationData.Wealth.MaxValue = config.Wealth.MaxValue;
        _motivationData.Wealth.LowValueNodeType = config.Wealth.LowValueNodeType;
        _motivationData.Wealth.HighValueNodeType = config.Wealth.HighValueNodeType;
        _motivationData.Wealth.SetValue(config.Wealth.GetRandomStartValue());

        _motivationData.Adventure.Name = config.Adventure.Name;
        _motivationData.Adventure.MaxValue = config.Adventure.MaxValue;
        _motivationData.Adventure.LowValueNodeType = config.Adventure.LowValueNodeType;
        _motivationData.Adventure.HighValueNodeType = config.Adventure.HighValueNodeType;
        _motivationData.Adventure.SetValue(config.Adventure.GetRandomStartValue());
    }
}
