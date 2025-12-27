public class AIMotivationData
{
    public MotivationStat Aggression { get; private set; }
    public MotivationStat Wealth { get; private set; }
    public MotivationStat Adventure { get; private set; }

    public AIMotivationData()
    {
        Aggression = new MotivationStat("Жажда боя", 0f, 100f);
        Wealth = new MotivationStat("Финансы", 0f, 100f);
        Adventure = new MotivationStat("Азартность", 0f, 100f);
    }
}
