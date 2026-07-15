# RuleKit Conventions

This document describes the design conventions followed by RuleKit.

These conventions ensure that every validation attribute behaves consistently and provides a predictable developer experience.

---

## General principles

- RuleKit extends the standard .NET DataAnnotations library.
- RuleKit complements DataAnnotations instead of replacing them.
- Every validation rule should solve a specific problem.
- Simplicity is preferred over unnecessary abstractions.

---

## Code formatting

Method and constructor calls should remain on a single line whenever reasonably possible. If a call becomes difficult to read, prefer extracting arguments into clearly named local variables instead of placing each argument on a separate line.

Conditional expressions may span multiple lines when this makes their branches easier to distinguish.

```csharp
return value >= 0
    ? value
    : -value;
```

---

## Namespace

All public types must belong to the same namespace.

```csharp
using RuleKit;
```

Consumers should never need to reference multiple namespaces depending on the validation rule.

The internal folder structure does not affect the public API.

---

## Validation attributes

Every validation attribute must:

- inherit from `ValidationAttribute`;
- be declared as `sealed`;
- declare its supported attribute targets explicitly;
- validate a single concept;
- never modify the validated object;
- provide XML documentation;
- include unit tests.

Example:

```csharp
[MaxDigits(5)]
public int? Code { get; set; }
```

---

## Null values

Unless explicitly stated otherwise, validation attributes must consider `null` values as valid.

Nullability should be validated using the standard `RequiredAttribute`.

Example:

```csharp
[Required]
[MaxDigits(5)]
public int? Code { get; set; }
```

---

## Error messages

Every attribute must provide an English default error message.

Consumers may override the message using the inherited `ErrorMessage` property.

Example:

```csharp
[MaxDigits(
    5,
    ErrorMessage = "The code cannot contain more than five digits.")]
public int? Code { get; set; }
```

RuleKit must not introduce custom constructor parameters for error messages.

---

## Constructor arguments

Invalid attribute configuration represents a developer error.

Constructors must throw meaningful exceptions when invalid arguments are supplied.

Examples:

- ArgumentException
- ArgumentOutOfRangeException

Applying an attribute to an unsupported data type is also a developer error. Validation must throw an `InvalidOperationException` with a message that identifies the attribute and unsupported type.

Constructor arguments that form part of the public contract must be exposed through read-only properties.

---

## Error message formatting and localization

Attributes must use the error message infrastructure inherited from `ValidationAttribute`.

This includes support for:

- `ErrorMessage`;
- `ErrorMessageResourceName`;
- `ErrorMessageResourceType`;
- placeholders formatted by `FormatErrorMessage(...)`.

Default messages and their placeholders must be documented by the attribute. Consumers must be able to replace them without changing validation behavior.

---

## Compatibility

RuleKit should work naturally with:

- Validator.TryValidateObject(...)
- Validator.TryValidateProperty(...)
- ASP.NET Core model validation

No additional configuration should be required.

---

## Versioning

RuleKit follows Semantic Versioning using the `Major.Minor.Patch[-Prerelease]` format.

- Versions below `1.0.0` represent the initial development phase.
- Prerelease versions use the `alpha`, `beta` and `rc` identifiers followed by a numeric component.
- Patch versions contain backwards-compatible bug fixes.
- Minor versions contain backwards-compatible functionality.
- Major versions may contain breaking public API or behavior changes.

The project declares `VersionPrefix` and `VersionSuffix` separately so prerelease identifiers can be overridden by the packaging workflow.

---

## Tests

Every public validation attribute must include unit tests.

At minimum, tests should cover:

- valid values;
- invalid values;
- null values;
- boundary values;
- invalid constructor arguments;
- custom error messages;
- unsupported data types when applicable.

Tests should also verify the default error message, its placeholders and the member name returned in the validation result.
