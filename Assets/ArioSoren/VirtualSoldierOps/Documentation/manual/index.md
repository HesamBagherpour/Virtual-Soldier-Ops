
# ArioSoren Virtual Soldier Ops (Refactor Task)

## Note:

- Strategies for using our package registry:
  -[x] Our packages
  -[ ] Our packages | Third-party packages
  -[ ] Our packages | Third-party packages | Legacy third-party packages

> **Packages:** Group of assets that already implemented package structure (package.json, Assembly Definition, work fine as package) that can be used in UPM.

> **Legacy Packages:** Group of assets packaged as .unityPackage file and not compatible with UPM or making them compatible.

- Rules for using source of packages:

## Folder Rules
- xTemp in Assets: should be used for any temporary assets excluding scripts.
- xTemp on * -> Scripts -> Runtime: should be used just for temporary scripts.

- All scripts should be a subset of an assembly definition.
- Editor scripts should be a subset of an assembly definition with just editor's platform configuration.

### Assembly Definition Conventions:
- Name of assembly definition file:
  `[CompanyName.ProjectOrPackageName]` e.g., `ArioSoren.VirtualSoldierOps`
  
  for editor:

  `[CompanyName.ProjectOrPackageName.Editor]` e.g., `ArioSoren.VirtualSoldierOps.Editor`

  for tests:
  
  `[CompanyName.ProjectOrPackageName.Test]` e.g., `ArioSoren.VirtualSoldierOps.Test`

- Use the name of assembly definition file for these properties:
  - Name
  - Root Namespace
  
- Unchecked `autoReferenced` property for manually checking package dependencies (to avoid missing added dependencies in the package's dependencies property list).


