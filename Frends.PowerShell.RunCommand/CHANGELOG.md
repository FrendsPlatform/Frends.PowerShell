# Changelog

## [2.0.0] - 2026-10-15
### Changed
- The Task's parameter and result types were renamed to follow the Frends Task standard (`Input`, `Options` and `Result`). Existing process definitions may need to be re-saved.
- The `Result` property of the Task result was renamed to `Results`. Update any references such as `#result.Result` to `#result.Results`.
- Added `ThrowErrorOnFailure` and `ErrorMessageOnFailure` options. By default the Task still throws on failure; when `ThrowErrorOnFailure` is disabled the Task returns a result with `Success` set to false and the failure details in `Error`.
- Added `Success` and `Error` properties to the Task result.
- Added missing documentation to the Task.

## [1.3.0] - 2026-09-29
### Added
- Option to execute commands with a custom PowerShell executable. Custom PowerShell executions cannot use a shared session and can set an execution policy.
### Changed
- Updated PowerShell to 7.4.20.
- Updated target framework to .NET 8.0.

## [1.2.0] - 2024-10-09
### Changed
- Added explicit reference to Microsoft.Management.Infrastructure
- 
## [1.1.0] - 2024-10-08
### Changed
- Updated PowerShell version to 7.2.23

## [1.0.2] - 2022-12-29
### Fixed
- Memory leak fix by unloading assembly context after Task execution.

## [1.0.1] - 2022-03-07
### Changed
- Targeting changed to only include .NET 6.0 (removed .NET Standard 2.0)

## [1.0.0] - 2022-03-04
### Added
- Initial implementation
