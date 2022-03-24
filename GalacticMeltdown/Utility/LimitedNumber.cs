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
            if(value != null)
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
            if(value != null)
                _value = Math.Max(_value, value.Value);
        }
    }
    
    [JsonIgnore]
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