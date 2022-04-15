namespace Template.AspNet6.Domain.Common;

public sealed class ModelState
{
    private readonly IDictionary<string, IList<string>> _errorMessages = new Dictionary<string, IList<string>>();

    public IDictionary<string, string[]> ErrorMessages 
        => _errorMessages.ToDictionary(item => item.Key, item => item.Value.ToArray());

    public bool IsValid => _errorMessages.Count == 0;

    public bool IsInvalid => !IsValid;
    
    public void Add(string key, string message)
    {
        if (!_errorMessages.ContainsKey(key)) 
            _errorMessages[key] = new List<string>();

        _errorMessages[key].Add(message);
    }
    
    public void Add(string subPropertyName, ModelState modelState)
    {
        foreach (var (key, value) in modelState.ErrorMessages)
            foreach (var message in value)
                Add($"{subPropertyName}.{key}", message);
    }
    
    public void Add(string subPropertyName, int index, ModelState modelState)
    {
        foreach (var (key, value) in modelState.ErrorMessages)
            foreach (var message in value)
                Add($"{subPropertyName}[{index}].{key}", message);
    }
}
