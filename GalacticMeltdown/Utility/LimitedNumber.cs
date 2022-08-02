using System;
using Newtonsoft.Json;

namespace GalacticMeltdown.Utility;

public class LimitedNumber
{
    [JsonProperty] private int? _maxValue;
    [JsonProperty] private int? _minValue;
    [JsonProperty] private int _value;

    [JsonIgnore]
    public int? MaxValue
    {
        get => _maxValue;
        set
        {
            _maxValue = value;
            if (value is not null)
                _value = Math.Min(_value, value.Value);
        }
    }
    
    [JsonIgnore]
    public int? MinValue
    {
        get => _minValue;
        set
        {
            _minValue = value;
            if(value is not null)
                _value = Math.Max(_value, value.Value);
        }
    }
    
    [JsonIgnore]
    public int Value
    {
        get => _value;
        set
        {
            _value = value;
            if (_maxValue is not null)
                _value = Math.Min(_value, _maxValue.Value);
            if (_minValue is not null)
                _value = Math.Max(_value, _minValue.Value);
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