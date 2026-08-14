# Changelog

All notable changes to this project are documented in this file.

This project follows a Keep a Changelog style and Semantic Versioning principles for release notes.

## [Unreleased]

### Changed
- Standardized release documentation and contribution guidelines for cleaner, more professional history going forward.

## [2.002] - 2026-08-14

### Added
- Added `PracticeClinicalDashboardEnricher` and dedicated tests for clinical dashboard enrichment paths.
- Added auto-save behavior in settings with debounce while keeping explicit manual save.
- Added centralized secondary-page tab bar hiding through navigation flow.

### Changed
- Moved and simplified practice dashboard orchestration to reduce thin pass-through layers.
- Improved journal factor chip presentation and note formatting for better readability.
- Updated startup and shell behaviors for safer clinical gate handling and better theme/tab synchronization.
- Aligned bootstrap/service registration so integration/bootstrap paths resolve technique catalog services.
- Updated long-form README with current stack markers where needed.

### Security
- Hardened local SQLite file handling with best-effort OS-level file protection and stricter app data path usage.

### Fixed
- Fixed fail-open behavior in clinical startup gate by switching to explicit fallback behavior on gate failures.
- Fixed silent clinical UI degradation by showing fallback risk status when enrichment fails.
- Fixed mismatch between root-tab and pushed-page tab visibility behavior.

## [2.001] - 2026-08-13

### Changed
- Deepened journal ritual flows and polished crisis/user safety paths.

### Fixed
- Improved note autosave reliability and clarified journal hub check-in behavior.

## [2.000] - 2026-08-12

### Changed
- Refined architecture boundaries by moving clinical and scoring rules further into Domain.

