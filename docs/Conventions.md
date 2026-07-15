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

---

## Compatibility

RuleKit should work naturally with:

- Validator.TryValidateObject(...)
- Validator.TryValidateProperty(...)
- ASP.NET Core model validation

No additional configuration should be required.

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