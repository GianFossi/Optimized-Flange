# Using OptimizedFlange with Codex in VS Code

## Objective

Codex should work from the repository itself rather than from a copied chat transcript. The persistent context is stored in repository files.

## Files Codex should use

The root `AGENTS.md` is the entry point. It directs Codex to read:

1. `AI.md` — current project memory and implementation status.
2. `README.md` — project purpose and user/developer overview.
3. `doc/AI_ENGINEERING_PROJECT_STANDARD.md` — reusable engineering-software standard.
4. `doc/AI_STARTER_INSTRUCTIONS.md` — compact working rules.
5. `doc/architecture/` — current architectural documentation.
6. `registry/` — machine-readable registries and policies.

Do not paste all previous ChatGPT messages into every Codex prompt. Important durable decisions belong in these files.

## Recommended local workflow

1. Clone or extract the repository into a normal local folder.
2. Open the **repository root folder** in VS Code, not a nested `src` folder.
3. Install the OpenAI Codex IDE extension and sign in with the same ChatGPT account when appropriate.
4. Install the recommended VS Code extensions shown by the workspace.
5. Install the .NET 10 SDK.
6. In the Codex panel, start with a repository-orientation task such as:

   `Read AGENTS.md, AI.md, README.md and doc/architecture. Do not modify files yet. Summarize the current architecture, implementation status and the next approved step.`

7. Then give a focused implementation task, for example:

   `Implement only Core Step 3 according to AGENTS.md. Keep normative equations out until source clauses and tests are available. Update README.md, doc/ and AI.md.`

8. Review the diff before accepting/committing changes.
9. Run the VS Code build/test tasks after the .NET SDK is installed.

## Git workflow

Prefer a feature branch before asking Codex to modify files:

```text
git switch -c feature/core-step3
```

After Codex finishes:

```text
dotnet restore
dotnet build OptimizedFlange.sln
git status
git diff
```

Commit only after review and successful applicable checks.

## Why AGENTS.md matters

Codex reads project instructions from `AGENTS.md` files in the project hierarchy. More specific files can be added in deeper folders later if a module needs stricter local instructions.

For this project the root file intentionally fixes the order:

```text
Core → Testing/Validation → WPF/MVVM UI
```
