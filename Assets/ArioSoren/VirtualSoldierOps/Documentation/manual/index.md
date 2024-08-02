

## Folder Structure
---

### Projects Asset Folder
- Each Unity project can include one or more sub-projects, all of which should be located separately in the `Assets/ArioSoren` folder.

> [!NOTE]
> No asset should be outside of its respective project folder unless it is a package.

![projects-path](images/projects-path.png)

### xTemp(s)
- `xTemp` in Assets: Should be used for any temporary assets excluding scripts.
- `xTemp` in * -> Scripts -> Runtime: Should be used only for temporary scripts.

![xTemp](images/x-temp-folders.png)

### Legacy Packages
Packages that are added to the project outside of Unity's Package Manager should, if possible, be placed in the following folders:

![xTemp](images/legacy-packages.png)

- `Assets/Plugins`: Packages that need to be in the `Plugins` folder.

> [!NOTE]
> Some packages may only work in their own root folder. These packages are exceptions.

### Configs/Settings/AssetDB
These types of assets should preferably be placed in the `Settings` folder unless they need to be in a specific folder. 
In the image, sample folders `Resources`, `XR`, and `XRI` are exceptions.

![xTemp](images/configs.png)

## Assembly Definition
---

- All scripts should be a subset of an assembly definition.
- Editor scripts should be a subset of an assembly definition with just the editor's platform configuration.

#### Assembly Definition Conventions:
- Name of assembly definition file:
  `[CompanyName.ProjectOrPackageName]` e.g., `ArioSoren.VirtualSoldierOps`
  
  For editor:

  `[CompanyName.ProjectOrPackageName.Editor]` e.g., `ArioSoren.VirtualSoldierOps.Editor`

  For tests:
  
  `[CompanyName.ProjectOrPackageName.Test]` e.g., `ArioSoren.VirtualSoldierOps.Test`

- Use the name of the assembly definition file for these properties:
  - Name
  - Root Namespace
  
- Uncheck the `autoReferenced` property for manually checking package dependencies (to avoid missing added dependencies in the package's dependencies property list).

## Glossary
---

> **Packages:** Group of assets that already implemented package structure (package.json, Assembly Definition, work fine as package) that can be used in UPM.

> **Legacy Packages:** Group of assets packaged as .unityPackage file and not compatible with UPM or making them compatible.

## C# Style Guide & Unity3D Coding Standards
---

- [Naming and code style tips for C# scripting in Unity](https://unity.com/how-to/naming-and-code-style-tips-c-scripting-unity)

- [Writing Cleaner Code That Scales-EBook-v6](https://cdn.bfldr.com/S5BC9Y64/at/5v473vn8kz7p85275gw8vnph/2022_WritingCleanerCodeThatScales_EBook-v6-Final.pdf?)

- [Unity3D Coding Standards](https://leotgo.github.io/unity-coding-standards/)

- [Microsoft Framework Design Guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/)

- [Unity-Code-Style-Guide StyleExample](https://github.com/thomasjacobsen-unity/Unity-Code-Style-Guide/blob/master/StyleExample.cs)

- Managers
Managers indicate that only one instance of that object exists in the project at any given time.

> [!NOTE]
> Managers may or may not use `DontDestroyOnLoad` depending on the code architecture.