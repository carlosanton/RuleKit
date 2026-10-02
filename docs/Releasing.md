# Releasing RuleKit

RuleKit packages are built and published by GitHub Actions. Publishing uses NuGet Trusted Publishing, so no permanent NuGet API key is stored in GitHub.

## Prepare the release

1. Update `VersionPrefix` in `src/RuleKit/RuleKit.csproj`. Set `VersionSuffix` only for prereleases; omit it for stable versions.
2. Set `PackageValidationBaselineVersion` to the latest published package version.
3. Move the relevant entries in `CHANGELOG.md` from `Unreleased` to the new version and add the release date.
4. Update version-specific installation examples when necessary.
5. Run the same checks used by CI:

```bash
dotnet restore RuleKit.slnx --locked-mode
dotnet format RuleKit.slnx --verify-no-changes --no-restore
dotnet build RuleKit.slnx --configuration Release --no-restore
dotnet test RuleKit.slnx --configuration Release --no-build
dotnet pack src/RuleKit/RuleKit.csproj --configuration Release --no-build --output artifacts
```

## Publish the release

1. Commit and push the prepared release to a separate branch, open a pull request into `master`, and merge it after the required `build` and `CodeQL` checks pass. The branch must be up to date; administrator bypass is disabled. A second person's approval is not required.
2. Create and push a tag whose name is the package version prefixed with `v`, such as `v0.2.0-alpha.1`.
3. Create a GitHub Release from that tag. Mark it as a prerelease when the version contains a prerelease suffix.
4. Review the `Publish to NuGet` workflow run and approve its `nuget` environment when the source and version are correct.
5. Verify that both the package and its symbols are available on NuGet.org.

The publication workflow rejects tags that are not contained in `master`. The protected `nuget` environment only accepts tags beginning with `v` and requires approval from the repository owner.

The tag version must exactly match the project version. The workflow checks this before building so the assembly and package versions stay consistent.

## Verify build provenance

Download `rulekit-release-packages` from the successful publication workflow run and extract the archive. The original packages are retained for 90 days. Verify each package with GitHub CLI, for example:

```bash
gh attestation verify RuleKit.1.0.0.nupkg --repo carlosanton/RuleKit --signer-workflow carlosanton/RuleKit/.github/workflows/publish.yml
```

This verifies the original build artifact. NuGet.org adds a repository signature when publishing, which changes the package bytes; the downloaded NuGet.org package therefore has a different digest from the original attested artifact. Use `dotnet nuget verify --all` to verify the NuGet.org package signature. Neither check is a guarantee that the code has no vulnerabilities.
