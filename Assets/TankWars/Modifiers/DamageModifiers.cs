public class DamageMultiplierModifier : IModifier
{
    private float damageMultiplier;

    public DamageMultiplierModifier(float damageMultiplier)
    {
        this.damageMultiplier = damageMultiplier;
    }

    public float Modify(float value)
    {
        return value * damageMultiplier;
    }
}

public class DamageReductionModifier : IModifier
{
    private float damageReduction;

    public DamageReductionModifier(float damageReduction)
    {
        this.damageReduction = damageReduction;
    }

    public float Modify(float value)
    {
        return value - damageReduction;
    }
}