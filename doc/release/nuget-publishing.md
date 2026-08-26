# NuGet Publishing

## Purpose

This document records the manual NuGet publishing path for OptimizedFlange packages.

Publishing is intentionally a release activity, not part of normal Debug development. Do not publish packages that contain unqualified normative engineering calculations unless the matching validation and qualification evidence has been completed.

## Prerequisites

- Install the .NET 10 SDK used by the repository.
- Create or sign in to a NuGet.org account.
- Create a NuGet.org API key with package push permissions.
- Keep the API key out of source control.

Microsoft documents the current CLI flow as:

- create packages with `dotnet pack`;
- publish existing `.nupkg` files with `dotnet nuget push`;
- use the NuGet.org source `https://api.nuget.org/v3/index.json`.

References:

- https://learn.microsoft.com/en-us/nuget/quickstart/create-and-publish-a-package-using-the-dotnet-cli
- https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-nuget-push
- https://learn.microsoft.com/en-us/nuget/nuget-org/publish-a-package

## API Key and Source

### NuGet.org

Get the API key from NuGet.org:

1. Sign in at `https://www.nuget.org/`.
2. Open the user menu in the upper-right corner.
3. Select `API Keys`.
4. Select `Create`.
5. Use a clear key name, for example `OptimizedFlange release publishing`.
6. Select a push scope for package publishing.
7. Limit the package glob pattern when package IDs are stable, or use `*` during early setup.
8. Copy the generated key immediately and store it in a password manager or a local secret store.

The NuGet.org v3 source is:

```text
https://api.nuget.org/v3/index.json
```

For this repository, the default script source is already NuGet.org:

```powershell
$env:NUGET_API_KEY = "replace-with-your-nuget-api-key"
.\scripts\publish-nuget.ps1 -Publish
```

### Custom or Private Feed

For a private feed, get the source URL from the feed provider's connection instructions. Then pass it explicitly:

```powershell
$env:NUGET_API_KEY = "replace-with-feed-token-or-api-key"
.\scripts\publish-nuget.ps1 -Source "replace-with-feed-v3-index-url" -Publish
```

Azure Artifacts, GitHub Packages, and other private feeds may use a personal access token or provider-specific credential instead of a NuGet.org API key. Follow that provider's feed connection page and keep credentials outside source control.

## Trusted Publishing

NuGet.org Trusted Publishing is the preferred GitHub Actions path because it avoids storing a long-lived NuGet API key in the repository.

This repository includes:

```text
.github/workflows/publish.yml
```

Configure the NuGet.org Trusted Publishing policy with:

```text
Package owner: Ganfoss
Publisher: GitHubActions
Repository Owner: GianFossi
Repository: OptimizedFlange
Workflow File: publish.yml
Environment: leave empty
Glob Patterns and Packages: OptimizedFlange.*
```

The workflow uses `NuGet/login@v1` with the NuGet.org profile name:

```text
Ganfoss
```

After the policy is created, run the GitHub workflow manually:

1. Open the GitHub repository.
2. Select `Actions`.
3. Select `Publish NuGet`.
4. Select `Run workflow`.

NuGet.org marks a new policy as provisional until it is used successfully. If the page says `Use within 7 day(s) to keep it permanently active`, run the workflow within that window. A successful trusted publish activates the policy permanently.

## Package Metadata

Common package metadata is centralized in `Directory.Build.props`.

Packages include:

- repository `README.md`;
- repository `LICENSE`;
- repository URL;
- PolyForm Noncommercial License 1.0.0 license file.

## Dry Run

From the repository root:

```powershell
.\scripts\publish-nuget.ps1
```

The default run restores, builds, tests, packs, lists the discovered `.nupkg` files, and stops before publishing.

## Publish to NuGet.org

Set the API key for the current PowerShell session:

```powershell
$env:NUGET_API_KEY = "replace-with-your-nuget-api-key"
```

Then publish:

```powershell
.\scripts\publish-nuget.ps1 -Publish
```

The script pushes all Release `.nupkg` files produced under the repository and passes `--skip-duplicate` to avoid failing when the same version already exists on the target feed.

## Useful Options

```powershell
.\scripts\publish-nuget.ps1 -IncludeSymbols
.\scripts\publish-nuget.ps1 -SkipTests
.\scripts\publish-nuget.ps1 -SkipPack -Publish
.\scripts\publish-nuget.ps1 -Source "https://api.nuget.org/v3/index.json" -Publish
.\scripts\publish-nuget.ps1 -ApiKey "replace-with-key" -Publish
```

Prefer `NUGET_API_KEY` over `-ApiKey` so secrets are less likely to appear in terminal history.

## Safety Checklist

Before publishing:

1. Confirm package IDs and version numbers.
2. Confirm the package license is intended for public distribution.
3. Run the full test suite.
4. Run `dotnet pack OptimizedFlange.sln --configuration Release`.
5. Inspect generated packages if the metadata changed.
6. Publish only reviewed packages.

NuGet.org packages are publicly visible after publishing unless unlisted from NuGet.org.
