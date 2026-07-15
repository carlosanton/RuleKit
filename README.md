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

Validates the maximum number of digits in an integer. The sign is not counted and `null` values are considered valid. It supports the built-in integer types, native-sized integers and `BigInteger`.

```csharp
using RuleKit;

public sealed class Request
{
    [MaxDigits(5)]
    public long? Code { get; set; }
}
```

Use the standard `RequiredAttribute` when a value must not be `null`. Error messages can be replaced through the properties inherited from `ValidationAttribute`.

### DecimalDigits

Validates the maximum number of integer and decimal digits in a `decimal`. The sign is not counted, while trailing decimal zeros are counted.

```csharp
using RuleKit;

public sealed class Request
{
    [DecimalDigits(11, 2)]
    public decimal? Amount { get; set; }
}
```

In this example, the value may contain up to 11 integer digits and 2 decimal digits. A value such as `1.2300` is invalid because it contains 4 decimal digits.

## License

MIT
