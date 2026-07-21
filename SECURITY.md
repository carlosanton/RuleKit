# Security Policy

## Supported versions

Security fixes are applied to the latest published version of RuleKit. Users should update to the newest available version before reporting a vulnerability that may already have been resolved.

## Reporting a vulnerability

Please report suspected vulnerabilities through [GitHub private vulnerability reporting](https://github.com/carlosanton/RuleKit/security/advisories/new).

Do not open a public issue for an unpatched vulnerability. Include the affected RuleKit version, a clear description, reproduction steps and the potential impact when possible.

The report will be reviewed privately. If the issue is confirmed, a fix and a coordinated disclosure will be prepared before its details are made public.

## Package integrity

Dependencies are locked and audited during every build. GitHub also performs CodeQL analysis, dependency monitoring, malware alerts and secret scanning.

NuGet packages are built from a version tag by GitHub Actions and published through NuGet Trusted Publishing. Publication uses a temporary credential and requires manual approval; no permanent NuGet API key is stored in the repository.

## Validation scope

RuleKit validates whether values follow declared application rules. Validation attributes do not replace authentication, authorization, output encoding, parameterized database access or the other security controls required by an application.
