public class HealingMultiplierModifier : IModifier
{
    private float healingMultiplier;

    public HealingMultiplierModifier(float healingMultiplier)
    {
        this.healingMultiplier = healingMultiplier;
    }

    public float Modify(float value)
    {
        return value * healingMultiplier;
    }
}

public class HealingReductionModifier : IModifier
{
    private float healingReduction;

    public HealingReductionModifier(float healingReduction)
    {
        this.healingReduction = healingReduction;
    }

    public float Modify(float value)
    {
        return value - healingReduction;
    }
}