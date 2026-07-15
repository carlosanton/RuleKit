# RuleKit Design

## Purpose

RuleKit extends .NET DataAnnotations with reusable validation rules.

The goal is not to replace DataAnnotations, but to provide validation attributes that are missing from the standard framework.

## Principles

- Keep the public API simple.
- Follow the conventions of .NET.
- Avoid duplicating existing DataAnnotations.
- Prefer readability over unnecessary abstractions.
- Every validation rule should solve one problem only.
- Keep the core package synchronous and independent from ASP.NET Core, dependency injection and external services.
- Treat documentation and tests as part of each public feature.
