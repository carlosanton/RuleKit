# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]

## [0.1.0-alpha.1] - 2026-07-18

### Added

- Add `DecimalDigitsAttribute` to validate the number of integer and decimal digits in decimal values.
- Add `NotDefaultAttribute` to reject default values of value types.
- Add `DateStringAttribute` to validate date strings using an exact .NET date format.
- Add property comparison attributes for greater-than and less-than relationships, including their inclusive variants.
- Add `RequiredIfAttribute` with optional case-insensitive and diacritic-insensitive string conditions.
- Add `EmailDomainAttribute` to restrict email addresses to one or more domains, with optional subdomain support.

### Changed

- Define the supported integer types and error behavior of `MaxDigitsAttribute`.
- Support standard `ValidationAttribute` error message formatting and localization in `MaxDigitsAttribute`.
- Set the initial package version to `0.1.0-alpha.1`.
- Include the project README in the NuGet package.

### Documentation

- Document public validation, error message and package scope conventions.
