# CI/CD — PokemonSDK GitHub Actions

## Stratégie de branches

```
main       ← Production (merge manuel depuis staging uniquement)
staging    ← Recette (merge automatique depuis dev)
dev        ← Intégration (merge via PR depuis feature/bugfix)
feature/*  ← Nouvelles features    ex: feature/battle-nuzlocke
bugfix/*   ← Corrections de bugs   ex: bugfix/damage-formula-gen4
hotfix/*   ← Corrections prod      ex: hotfix/save-corruption
```

### Flux complet

```
feature/* ou bugfix/*
    │  PR → dev (CI vert obligatoire)
    ▼
   dev ──── CI + deploy auto ──── Environnement DEV
    │  merge auto dev → staging (après CI vert)
    ▼
staging ──── CI + deploy auto ──── Environnement RECETTE
    │  validation manuelle (smoke test + QA)
    │  PR manuelle staging → main
    ▼
  main ──── CI + deploy auto ──── Environnement PRODUCTION + NuGet (sur tag v*)
```

### Règles (NON NÉGOCIABLES)

- **Jamais de commit direct** sur `dev`, `staging`, `main`
- **Toute feature/bugfix** part de `dev` : `git checkout -b feature/nom dev`
- **PR feature → dev** : CI vert obligatoire avant merge
- **Merge staging → main** : manuel uniquement, après validation recette
- **Hotfix** : branche depuis `main`, merge dans `main` ET `dev`

### Nommage commits

```
feat(battle): add nuzlocke permanent death plugin
fix(data): correct type chart gen4 grass-fire effectiveness
refactor(scripting): extract lua sandbox into separate class
test(battle): add coverage for sleep status no-skip turns
docs(readme): add installation instructions
chore(ci): add sprite validator step to ci workflow
```

---

## .github/workflows/ci.yml

```yaml
name: CI

on:
  pull_request:
    branches: [dev]
  push:
    branches: [feature/**, bugfix/**]

jobs:
  build-and-test:
    strategy:
      matrix:
        os: [ubuntu-latest, windows-latest]
    runs-on: ${{ matrix.os }}

    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET 10
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Restore
        run: dotnet restore PokemonSDK.sln

      - name: Build
        run: dotnet build PokemonSDK.sln --no-restore --configuration Release

      - name: Test + Coverage
        run: |
          dotnet test tests/ \
            --no-build \
            --configuration Release \
            --collect:"XPlat Code Coverage" \
            --logger "console;verbosity=normal"

      - name: Validate SDK.Core dependencies (doit être vide)
        run: dotnet list src/SDK.Core/SDK.Core.csproj package

      - name: Validate sprites
        run: dotnet run --project src/SDK.Tools -- asset-validate
        continue-on-error: false  # exit code 1 = PR bloquée

      - name: Headless smoke test
        if: matrix.os == 'ubuntu-latest'
        run: dotnet run --project src/SDK.MonoGame -- --headless
```

---

## .github/workflows/deploy-dev.yml

```yaml
name: Deploy DEV

on:
  push:
    branches: [dev]

jobs:
  deploy:
    runs-on: ubuntu-latest
    environment: dev

    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET 10
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Build Release
        run: dotnet build PokemonSDK.sln --configuration Release

      - name: Run all tests (bloquant)
        run: dotnet test tests/ --configuration Release --no-build

      - name: Publish SDK
        run: |
          dotnet publish src/SDK.MonoGame/SDK.MonoGame.csproj \
            --configuration Release \
            --output ./publish/dev

      - name: Upload artifact DEV
        uses: actions/upload-artifact@v4
        with:
          name: pokemonsdk-dev-${{ github.sha }}
          path: ./publish/dev
          retention-days: 7
```

---

## .github/workflows/deploy-staging.yml

```yaml
name: Deploy STAGING

on:
  push:
    branches: [staging]

jobs:
  deploy:
    runs-on: ubuntu-latest
    environment: staging

    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET 10
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Build + Test
        run: |
          dotnet build PokemonSDK.sln --configuration Release
          dotnet test tests/ --configuration Release --no-build

      - name: Publish SDK STAGING
        run: |
          dotnet publish src/SDK.MonoGame/SDK.MonoGame.csproj \
            --configuration Release \
            --output ./publish/staging

      - name: Upload artifact STAGING
        uses: actions/upload-artifact@v4
        with:
          name: pokemonsdk-staging-${{ github.sha }}
          path: ./publish/staging
          retention-days: 14

      - name: Notify — validation recette requise
        run: |
          echo "✅ Build staging ${{ github.sha }} prêt"
          echo "⏳ Validation manuelle recette requise avant merge → main"
```

---

## .github/workflows/deploy-prod.yml

```yaml
name: Deploy PROD

on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    environment: production  # GitHub env protégé avec required reviewers

    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET 10
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Build + Test (filet de sécurité final)
        run: |
          dotnet build PokemonSDK.sln --configuration Release
          dotnet test tests/ --configuration Release --no-build

      - name: Publish SDK PROD
        run: |
          dotnet publish src/SDK.MonoGame/SDK.MonoGame.csproj \
            --configuration Release \
            --output ./publish/prod

      - name: Create GitHub Release
        uses: actions/create-release@v1
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
        with:
          tag_name: ${{ github.ref_name }}
          release_name: PokemonSDK ${{ github.ref_name }}
          draft: false
          prerelease: false

      - name: Upload release artifact
        uses: actions/upload-artifact@v4
        with:
          name: pokemonsdk-prod-${{ github.sha }}
          path: ./publish/prod
```

---

## .github/workflows/publish-nuget.yml

```yaml
name: Publish NuGet

on:
  push:
    tags: ['v*']   # déclenché sur tag vX.Y.Z uniquement

jobs:
  publish:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET 10
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Build Release
        run: dotnet build PokemonSDK.sln --configuration Release

      - name: Run all tests
        run: dotnet test tests/ --configuration Release --no-build

      - name: Pack tous les packages
        run: |
          dotnet pack src/SDK.Core           --configuration Release -o ./nupkg
          dotnet pack src/SDK.Data           --configuration Release -o ./nupkg
          dotnet pack src/SDK.Battle         --configuration Release -o ./nupkg
          dotnet pack src/SDK.Scripting      --configuration Release -o ./nupkg
          dotnet pack src/SDK.Plugins.Nuzlocke   --configuration Release -o ./nupkg
          dotnet pack src/SDK.Plugins.Randomizer --configuration Release -o ./nupkg
          dotnet pack src/SDK.Plugins.Turbo      --configuration Release -o ./nupkg

      - name: Push vers NuGet.org
        run: |
          dotnet nuget push ./nupkg/*.nupkg \
            --api-key ${{ secrets.NUGET_API_KEY }} \
            --source https://api.nuget.org/v3/index.json \
            --skip-duplicate

      - name: Upload nupkg artifacts
        uses: actions/upload-artifact@v4
        with:
          name: nuget-packages-${{ github.ref_name }}
          path: ./nupkg
```

---

## Commandes Git — workflow quotidien

```bash
# Créer une branche feature depuis dev
git checkout dev && git pull origin dev
git checkout -b feature/battle-nuzlocke

# Créer une branche bugfix
git checkout -b bugfix/damage-formula-gen4

# Merger dev → staging (après CI vert sur dev)
git checkout staging && git pull origin staging
git merge dev
git push origin staging

# Merger staging → main (validation manuelle recette uniquement)
git checkout main && git pull origin main
git merge staging
git push origin main

# Créer un tag de release (déclenche publish-nuget.yml)
git tag v1.0.0 -m "Release v1.0.0 — PokéForge v1.0 Release"
git push origin v1.0.0

# Hotfix en prod
git checkout main && git pull
git checkout -b hotfix/save-corruption
# ... fix ...
git checkout main && git merge hotfix/save-corruption
git checkout dev  && git merge hotfix/save-corruption
git push origin main dev
```

---

## Règles Claude Code pour le CI/CD

- Toujours travailler sur `feature/*` ou `bugfix/*` — jamais commit direct sur `dev`
- `dotnet test` vert localement avant de proposer une PR
- Ne jamais proposer de merger `staging → main` — décision humaine uniquement
- Les tags NuGet suivent SemVer strict : `v1.0.0`, `v1.1.0`, `v2.0.0`
- Breaking change = bump majeur obligatoire (D-18)
