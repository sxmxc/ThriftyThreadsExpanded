# Thrifty Threads Expanded

Spiritual successor to the MoreClothing Schedule I mod that adds purchasable copies of ten NPC-only outfits and accessories to Thrifty Threads.

## Requirements

- MelonLoader 0.7.1 or compatible
- S1API 3.1.4 or compatible
- `ThriftyThreadsExpanded_Il2cpp.dll` for the default/`beta` IL2CPP branch, or `ThriftyThreadsExpanded_Mono.dll` for the `alternate`/`alternate-beta` Mono branch

Install only the DLL for the runtime you use.

## Configuration and ModHub

The mod registers standard MelonPreferences under `ThriftyThreadsExpanded`. When ModHub 1.8 or newer is installed, an optional integration renders those same preferences in a dedicated ModHub panel. ModHub is not required for the mod itself. There is a master switch and a separate switch for every clothing item.

These options create purchasable copies of clothing assets already owned by the game and used by NPCs. The game assets are referenced at runtime; they are not included in this project or the release package. Restart the game after changing an option.

## Included clothing

- Police Tactical Vest
- Police Cap
- Respirator
- Hazmat Suit
- Police Utility Belt
- Fast Food Staff Shirt
- Fast Food Staff Cap
- Gas Station Staff Shirt
- Tucked T-Shirt
- Formal Clergy Gown

Each item has an original store icon embedded in the mod DLL.

## Build

Run `setup.ps1` once to create the ignored `local.build.props`, then build each runtime:

```powershell
dotnet build .\ThriftyThreadsExpanded.sln -c Mono -p:AutomateLocalDeployment=false
dotnet build .\ThriftyThreadsExpanded.sln -c Il2cpp -p:AutomateLocalDeployment=false
```

Outputs are written beneath `bin\Mono` and `bin\Il2cpp`.

The project does not include or redistribute game assemblies, decompiled source, generated IL2CPP wrappers, or game artwork/assets.
