# RuleKit

A lightweight .NET library that extends DataAnnotations with reusable validation rules.

> 🚧 This project is under development.

## Goals

- Extend .NET DataAnnotations with reusable validation rules.
- Keep a clean and intuitive API.
- Be fully unit tested.
- Be easy to integrate into any .NET project.

## Available attributes

### MaxDigits

Validates the maximum number of digits in an integer. The sign is not counted and `null` values are considered valid.

```csharp
using RuleKit;

public sealed class Request
{
    [MaxDigits(5)]
    public long? Code { get; set; }
}
```

Use the standard `RequiredAttribute` when a value must not be `null`. Error messages can be replaced through the properties inherited from `ValidationAttribute`.

## License

MIT
