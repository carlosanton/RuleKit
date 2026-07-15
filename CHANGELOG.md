# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]

### Added

- Add `DecimalDigitsAttribute` to validate the number of integer and decimal digits in decimal values.
- Add `NotDefaultAttribute` to reject default values of value types.
- Add `DateStringAttribute` to validate date strings using an exact .NET date format.

### Changed

- Define the supported integer types and error behavior of `MaxDigitsAttribute`.
- Support standard `ValidationAttribute` error message formatting and localization in `MaxDigitsAttribute`.
- Set the initial package version to `0.1.0-alpha.1`.
- Include the project README in the NuGet package.

### Documentation

- Document public validation, error message and package scope conventions.
