# MCP for Unity — 2020.3 LTS Backport

A fork of [CoplayDev/unity-mcp](https://github.com/CoplayDev/unity-mcp) backported to **Unity 2020.3 LTS**. The upstream package requires Unity 2021.3+; this fork compiles and runs on 2020.3 (tested on **2020.3.49f1**), so older projects can give AI assistants — Claude Code, Cursor, VS Code, and any other [MCP](https://modelcontextprotocol.io/introduction) client — direct control of the Unity Editor: scene and GameObject editing, script management, play mode, tests, screenshots, and more.

Forked from upstream `v9.7.2-beta.10`. All credit for the architecture and tools goes to the [Coplay](https://www.coplay.dev/?ref=unity-mcp) team — see the [upstream docs](https://coplaydev.github.io/unity-mcp/) for the full tool reference; almost everything there applies to this fork unchanged.

---

## Install

### 1. The Unity package

**Package Manager → + → Add package from git URL:**

```text
https://github.com/kylemaestro/unity-2020-mcp.git?path=/MCPForUnity
```

Or clone this repo and reference it locally in your project's `Packages/manifest.json` (useful if you want to hack on the package):

```json
"com.coplaydev.unity-mcp": "file:../../unity-2020-mcp/MCPForUnity"
```

### 2. The MCP server (Python)

The server lives in `Server/` and requires Python 3.10+ and [uv](https://docs.astral.sh/uv/):

```powershell
cd unity-2020-mcp/Server
uv sync
```

### 3. Connect your MCP client

The in-editor setup wizard (Window → MCP for Unity) can auto-configure detected clients, but the manual route is simple. For **Claude Code**, drop a `.mcp.json` in your Unity project root:

```json
{
  "mcpServers": {
    "UnityMCP": {
      "command": "<path-to>/unity-2020-mcp/Server/.venv/Scripts/mcp-for-unity.exe",
      "args": []
    }
  }
}
```

(On macOS/Linux the venv script is `Server/.venv/bin/mcp-for-unity`.)

### 4. Use stdio transport

This fork is tested with the **stdio** transport: the editor hosts a local TCP bridge (port 6400) and your client launches the Python server, which connects to it. In the MCP for Unity window, make sure the transport is set to **stdio**, not HTTP. If the bridge doesn't come up when the editor loads (no `unity-mcp-status-*.json` heartbeat in `~/.unity-mcp/`), check that the `MCPForUnity.UseHttpTransport` editor pref is off — HTTP mode depends on `uvx` being on PATH and is not covered by this backport's testing.

With the editor open, your MCP client should now list ~42 `manage_*` tools. Try: *"Take a screenshot of the game view."*

---

## What the backport changes

Unity 2020.3 compiles C# 8 against a .NET Standard 2.0 profile, and several editor APIs the upstream package uses didn't exist yet. The changes fall into three buckets, all inside `MCPForUnity/`:

**C# 9 → C# 8 syntax** — `is not` patterns, target-typed `new()`, and `and`/`or` pattern combinators rewritten throughout (~100 sites).

**Missing BCL APIs** — shimmed in `Editor/Helpers/Compat/`:
- `NetStandard20Compat` — `string.Contains(value, comparison)`, `string.Split(char, options)`, `string.Replace(…, comparison)`, `Dictionary.Remove(key, out value)`, and friends, as extension methods
- `MathCompat.Clamp`, `PathCompat.GetRelativePath`
- Range/index expressions (`s[..^4]`) rewritten to `Substring`

**Newer Unity APIs** — gated behind `#if UNITY_2021_2_OR_NEWER` (or `2021_1`) with 2020.3 fallbacks:

| Upstream API | 2020.3 fallback |
|---|---|
| `UnityEditor.SceneManagement.PrefabStage(Utility)` | alias to the `Experimental` namespace; `OpenPrefab` → `AssetDatabase.OpenAsset` |
| `NamedBuildTarget` | `BuildTargetGroup` overloads |
| `Client.AddAndRemove` (UPM batch) | sequential `Client.Add`/`Client.Remove` |
| `PackageInfo.GetAllRegisteredPackages` | reflection over the internal `GetAll()` |
| `DropdownField` (UI Toolkit) | custom `PopupField<string>` subclass with a UXML factory (`Editor/Helpers/Compat/DropdownFieldCompat.cs`) |
| `StandaloneBuildSubtarget`, `BuildOptions.CleanBuildCache`, `LightmapCompression`, `ShaderPropertyType.Int`, `MaterialPropertyBlock.HasColor`, `ProfilerCategory.FileIO`, `ArrayPool` in the WebSocket client | gated out or replaced with equivalents |

The Python server is unchanged — it's version-agnostic.

## Known limitations on 2020.3

- **`manage_ui` is stubbed.** It drives runtime UI Toolkit (`UIDocument`/`PanelSettings`), which shipped in Unity 2021.2. The tool returns a friendly error pointing you at the uGUI workflow (`manage_gameobject`/`manage_components`), which works fully.
- **No standalone server (headless) build subtarget** — the concept doesn't exist before 2021.2; builds always target the player.
- **Lightmap compression settings** can't be read or written (API is 2021.1+).
- Properties/APIs in the table above silently degrade to their fallbacks; behavior is otherwise identical to upstream.

## Compatibility

| Component | Version |
|---|---|
| Unity | 2020.3 LTS (tested 2020.3.49f1) — newer versions still work via the `#if` gates |
| Upstream base | v9.7.2-beta.10 |
| Python | 3.10+ |
| `com.unity.nuget.newtonsoft-json` | 3.0.2 (pulled in automatically) |

## License

MIT, same as upstream — see [LICENSE](LICENSE).
