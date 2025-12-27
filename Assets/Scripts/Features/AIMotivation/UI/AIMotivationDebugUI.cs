using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VContainer;

public class AIMotivationDebugUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _aggressionText;
    [SerializeField] private Slider _aggressionSlider;

    [SerializeField] private TextMeshProUGUI _wealthText;
    [SerializeField] private Slider _wealthSlider;

    [SerializeField] private TextMeshProUGUI _adventureText;
    [SerializeField] private Slider _adventureSlider;

    [Inject] private AIMotivationData _motivationData;

    private void Awake()
    {
        DI.Inject(this);
    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (_motivationData == null) return;

        UpdateStat(_aggressionText, _aggressionSlider, _motivationData.Aggression);
        UpdateStat(_wealthText, _wealthSlider, _motivationData.Wealth);
        UpdateStat(_adventureText, _adventureSlider, _motivationData.Adventure);
    }

    private void UpdateStat(TextMeshProUGUI text, Slider slider, MotivationStat stat)
    {
        if (stat == null) return;

        if (text != null)
        {
            text.text = $"{stat.CurrentValue:F0}/{stat.MaxValue:F0}";
        }

        if (slider != null)
        {
            slider.value = stat.Percentage;
        }
    }
}
