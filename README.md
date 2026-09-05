# SPT Ready

This mod changes the `Next` action on the Select Location screen for both PMC and Scav raids.
Cou can change the behaviour in F12 menu.
Button will skip to confirmation screen or start the raid outright.

## Building

The project targets `netstandard2.1`. Required EFT, SPT, Unity, Harmony, and BepInEx assemblies are referenced in place from a read-only SPT installation through the `SptPath` MSBuild property. Proprietary game assemblies are not copied into build output or release archives.

Set the path in the ignored `Directory.Build.props.user` file in the repository root:

```xml
<Project>
  <PropertyGroup>
    <SptPath>X:\Path\To\SPT</SptPath>
  </PropertyGroup>
</Project>
```

Build and package the plugin:

```powershell
pwsh -File .\scripts\build-release.ps1
```

After a successful restore, pass `-NoRestore` for an offline build. The archive is written to `dist/`; temporary package staging remains under ignored `artifacts/`. The script never deploys files into the referenced SPT installation.

# Changelog

4.1.0 - initial release for SPT 4.1.4

## License

SPT Ready is licensed under the MIT License.
