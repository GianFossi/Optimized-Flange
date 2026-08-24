# Core Bootstrap Architecture

## Goal

Establish a stable F# foundation before implementing normative flange calculations.

## Module boundaries

### OptimizedFlange.Domain
Owns general engineering primitives and states. It has no dependency on WPF, persistence, JSON, filesystem access, or external repositories.

### OptimizedFlange.Configuration
Owns typed software/calculation configuration models and pure configuration operations. It distinguishes global defaults from project-owned technical configuration.

### OptimizedFlange.Persistence
Owns filesystem and JSON side effects. The calculation core must not depend on these implementation details.

## Dependency direction

```text
Domain
  ↑
Configuration
  ↑
Persistence
```

No reverse dependency is allowed.

## Next core steps

1. Define project/domain identifiers and project root model.
2. Define load-case domain types.
3. Define geometry states and flange-joint topology.
4. Define bolting domain.
5. Define gasket assembly/sealing-zone domain.
6. Define material snapshot contracts.
7. Only then start normative calculation engines.
