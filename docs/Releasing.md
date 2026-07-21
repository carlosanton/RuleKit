# Releasing RuleKit

RuleKit packages are built and published by GitHub Actions. Publishing uses NuGet Trusted Publishing, so no permanent NuGet API key is stored in GitHub.

## Prepare the release

1. Update `VersionPrefix` and `VersionSuffix` in `src/RuleKit/RuleKit.csproj`.
2. Move the relevant entries in `CHANGELOG.md` from `Unreleased` to the new version and add the release date.
3. Update version-specific installation examples when necessary.
4. Run the same checks used by CI:

```bash
dotnet restore RuleKit.slnx --locked-mode
dotnet format RuleKit.slnx --verify-no-changes --no-restore
dotnet build RuleKit.slnx --configuration Release --no-restore
dotnet test RuleKit.slnx --configuration Release --no-build
dotnet pack src/RuleKit/RuleKit.csproj --configuration Release --no-build --output artifacts
```

## Publish the release

1. Commit and push the prepared release to `master`.
2. Create and push a tag whose name is the package version prefixed with `v`, such as `v0.2.0-alpha.1`.
3. Create a GitHub Release from that tag. Mark it as a prerelease when the version contains a prerelease suffix.
4. Review the `Publish to NuGet` workflow run and approve its `nuget` environment when the source and version are correct.
5. Verify that both the package and its symbols are available on NuGet.org.

The publication workflow rejects tags that are not contained in `master`. The protected `nuget` environment only accepts tags beginning with `v` and requires approval from the repository owner.
