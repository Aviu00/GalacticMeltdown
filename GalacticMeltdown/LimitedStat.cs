using System;

namespace GalacticMeltdown;

public class LimitedStat
{
    private int _value;
    private int _maxValue;

    public int Value
    {
        get => _value;
        set => _value = Math.Min(value, _maxValue);
    }

    public int MaxValue
    {
        get => _maxValue;
        set
        {
            _maxValue = value;
            _value = Math.Min(_value, _maxValue);
        }
    }

    public LimitedStat(int value, int maxValue)
    {
        _maxValue = maxValue;
        _value = Math.Min(value, _maxValue);
    }
}