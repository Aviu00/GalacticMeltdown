using System;

namespace GalacticMeltdown.Actors;

public class LimitedNumber
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

    public LimitedNumber(int value, int maxValue)
    {
        _maxValue = maxValue;
        _value = Math.Min(value, _maxValue);
    }
}