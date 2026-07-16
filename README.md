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

### NotDefault

Validates that a value type does not contain its default value. It can be used with built-in value types, enums and custom structs.

```csharp
using RuleKit;

public sealed class Request
{
    [NotDefault]
    public Guid? Identifier { get; set; }
}
```

`null` values are considered valid. Combine `NotDefaultAttribute` with the standard `RequiredAttribute` when a value must be present and different from its default value. For example, the default value is zero for numeric types and the zero-valued member for enums. Boolean values are not supported because requiring a value different from `false` would be more clearly expressed as a specific rule that requires `true`.

### DateString

Validates that a string represents a real calendar date written in an exact .NET date format. Validation uses invariant culture.

```csharp
using RuleKit;

public sealed class Request
{
    [DateString("yyyyMMdd")]
    public string? Date { get; set; }
}
```

Common format examples:

| Format | Valid value |
| --- | --- |
| `yyyyMMdd` | `20260715` |
| `yyyy-MM-dd` | `2026-07-15` |
| `ddMMyyyy` | `15072026` |
| `dd/MM/yyyy` | `15/07/2026` |

Any standard or custom date format supported by `DateOnly` can be supplied directly. `null` values are considered valid, while empty strings, white space, mismatched formats and nonexistent calendar dates are invalid.

### Property comparisons

Validates the order between two properties on the same object. The available attributes are `GreaterThan`, `GreaterThanOrEqualTo`, `LessThan` and `LessThanOrEqualTo`.

```csharp
using RuleKit;

public sealed class Request
{
    public DateOnly? StartDate { get; set; }

    [GreaterThanOrEqualTo(nameof(StartDate))]
    public DateOnly? EndDate { get; set; }
}
```

Declare each relationship on only one of its properties to avoid returning two errors for the same invalid pair. Both properties must use the same type, although `T` and `T?` are considered compatible. Strings and Boolean values are not supported.

If either value is `null`, validation succeeds. Use the standard `RequiredAttribute` independently when either property must contain a value. Missing properties, incompatible types and types that do not implement `IComparable` are treated as developer errors.

### RequiredIf

Makes a property required when another property contains a specified value.

```csharp
using RuleKit;

public sealed class Request
{
    public bool WantsInvoice { get; set; }

    [RequiredIf(nameof(WantsInvoice), true)]
    public string? TaxIdentifier { get; set; }
}
```

When `WantsInvoice` is `true`, `TaxIdentifier` follows the same rules as the standard `RequiredAttribute`. When the condition is not met, `TaxIdentifier` is not required.

String conditions use exact ordinal equality by default. Case and diacritical differences can be ignored explicitly:

```csharp
[RequiredIf(nameof(VehicleType), "camión", IgnoreCase = true, IgnoreDiacritics = true)]
public string? VehicleIdentifier { get; set; }
```

`IgnoreCase` and `IgnoreDiacritics` use invariant culture and never modify the received value. `AllowEmptyStrings` is also available and behaves like the property with the same name on `RequiredAttribute`.

The condition property and expected value must have the same type, although `T` and `T?` are considered compatible. The expected value must also be valid as a C# attribute argument. Primitive values accepted by C#, strings and enums are supported directly; values such as `decimal`, `Guid` and `DateTime` cannot be written directly as attribute arguments.

## License

MIT
