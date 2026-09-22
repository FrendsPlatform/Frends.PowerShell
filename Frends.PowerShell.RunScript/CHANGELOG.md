# Changelog

## [1.3.0] - 2025-01-01
### Changed
- Updated target framework to .NET 8.0 and Microsoft.PowerShell.SDK to 7.4.20 (from .NET 6.0 / 7.2.23).
  This fixes failures (e.g. "The getter method should be public, not void, static, and have one parameter
  of the type PSObject") when importing Windows-PowerShell-only modules, such as ActiveDirectory, via the
  Windows PowerShell Compatibility feature. The root cause was a PowerShell engine bug
  (PowerShell/PowerShell#13157) where, in hosted/published .NET applications, the PowerShell `Modules`
  folder used by the compatibility feature to resolve its own engine modules ends up separated from
  `System.Management.Automation.dll`, breaking the implicit proxy/type generation for compatibility-loaded
  modules. This was fixed upstream in PowerShell 7.3.0 and later.

## [1.2.0] - 2024-10-09
### Changed
- Added explicit reference to Microsoft.Management.Infrastructure

## [1.1.0] - 2024-10-08
### Changed
- Updated PowerShell version to 7.2.23

 [1.0.2] - 2022-12-21
### Changed
- Memory leak fix.

# [1.0.1] - 2022-03-04
### Changed
- Targeting changed to only include .NET 6.0 (removed .NET Standard 2.0)

## [1.0.0] - 2022-02-25
### Added
- Initial implementation
