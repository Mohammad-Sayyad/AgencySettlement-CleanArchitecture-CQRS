# Dependency direction

```text
Domain
  ↑
Application
  ↑
Infrastructure
  ↑
API
```

Infrastructure also implements Application abstractions for persistence and external integration.
No Application reference to Infrastructure exists.
No Domain reference to Application/Infrastructure/API exists.
