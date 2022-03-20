using System;
using Newtonsoft.Json;

namespace GalacticMeltdown.Utility;

public class LimitedNumber
{
    private int _value;
    private int? _maxValue;
    private int? _minValue;

    public int Value
    {
        get => _value;
        set
        {
            if(_maxValue != null)
                _value = Math.Min(value, _maxValue.Value);
            if(_minValue != null)
                _value = Math.Max(value, _minValue.Value);
        }
    }

    public int? MaxValue
    {
        get => _maxValue;
        set
        {
            _maxValue = value;
            if(value != null)
                _value = Math.Min(_value, value.Value);
        }
    }
    
    public int? MinValue
    {
        get => _minValue;
        set
        {
            _minValue = value;
            if(value != null)
                _value = Math.Max(_value, value.Value);
        }
    }

    [JsonConstructor]
    private LimitedNumber()
    {
    }
    public LimitedNumber(int value, int? maxValue = null, int? minValue = null)
    {
        _value = value;
        MaxValue = maxValue;
        MinValue = minValue;
    }
}