# Contributing

Thanks for contributing to `PsychologyApp`.

## Commit Message Standard

To keep history professional and easy to scan, use this format:

`type(scope): short summary`

Examples:
- `feat(journal): add factor summary line in timeline notes`
- `fix(startup): fail closed on clinical gate exceptions`
- `refactor(practice): remove thin dashboard presenter layer`
- `test(presentation): add enricher fallback unit coverage`
- `docs(readme): update stack markers and quick links`

## Recommended Types

- `feat` - new user-facing behavior
- `fix` - bug fix or regression fix
- `refactor` - internal structure change without behavior change
- `test` - test-only changes
- `docs` - documentation-only changes
- `chore` - maintenance tasks

## Scope Recommendations

Use clear scopes based on folders/features, for example:
- `journal`
- `profile-settings`
- `practice`
- `clinical`
- `navigation`
- `bootstrap`
- `infrastructure`
- `docs`

## Pull Request Checklist

- Keep PR focused on one logical change.
- Add or update tests for behavior changes.
- Use `AppStrings` for all user-facing copy.
- Avoid force-push to `main`.
- Update `CHANGELOG.md` when shipping notable behavior changes.

