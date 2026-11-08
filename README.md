<p align="center">
    <img src="Branding/plusstrapper-horizontal-dark.svg#gh-dark-mode-only" width="380" alt="PlusStrapper">
    <img src="Branding/plusstrapper-horizontal.svg#gh-light-mode-only" width="380" alt="PlusStrapper">
</p>

<p align="center"><b>Make Roblox look the way you want, without having to tweak settings by hand.</b></p>

PlusStrapper is a Roblox launcher for Windows. Pick a font, cursor, emoji style or classic sounds from a simple menu, press **Save**, and PlusStrapper applies them every time you play.

It is built on [Bloxstrap](https://github.com/bloxstraplabs/bloxstrap) (MIT), but it puts the things you can *see* first and hides the advanced Fast Flags.

## Install

1. Go to the [**latest release**](https://github.com/randonaughtica-arch/plusstrapper/releases/latest) and download `PlusStrapper.exe`.
2. Double-click `PlusStrapper.exe`. If Windows SmartScreen shows *"Windows protected your PC"*, click **More info**, then **Run anyway**. This happens because the app isn't code-signed.
3. Pick a language, then click **Install**. It installs to `%LocalAppData%\PlusStrapper` and puts a **PlusStrapper** shortcut on your Desktop. No admin rights are needed.
4. Open **PlusStrapper** from your Desktop, change what you like, and press **Save**.
5. Play any game from roblox.com as usual. PlusStrapper now launches Roblox for you.

You don't need to install .NET. The exe includes everything it needs.

### Uninstall

Go to **Settings > Apps > Installed apps**, find **PlusStrapper**, and click **Uninstall**. Roblox will go back to its normal launcher.

## What you can change

**Look & Feel** (changes Roblox itself)
- **Game font**: Roblox default, Nimbus Sans, Nimbus Sans Bold, or any `.ttf` / `.otf` file of your own
- **Emoji style**: Twemoji, Catmoji, Windows 11, Windows 10, Windows 8
- **Mouse cursor**: default, 2006, 2013, or any PNG image of your own
- **Old character sounds** and **old avatar editor background**
- **Performance level**: Off, Light, Balanced or Max. This turns down anti-aliasing, textures, grass and model detail for more FPS. It only uses fast flags on [Roblox's allowlist](https://devforum.roblox.com/t/allowlist-for-local-client-configuration-via-fast-flags/3966569), so every setting is one Roblox still accepts. Roblox doesn't allow changing the FPS cap this way.

**Strapper Settings** (changes PlusStrapper's own windows)
- **Colour theme**: light, dark, or follow Windows
- **Accent colour**: PlusStrapper Teal, Green, Blue, Purple, Pink, Orange, Red, or your Windows accent
- **App font**: Nimbus Sans or Segoe UI

**Appearance**: how the window that shows up while Roblox is loading looks (style, icon, title, custom themes).

Bloxstrap's other features are still there: Discord Rich Presence, server location, multi-instance, launch shortcuts and more.

## Is it safe? Can I get banned?

PlusStrapper only swaps out Roblox's own files (fonts, images, sounds) before the game starts. It doesn't inject code into Roblox. Bloxstrap has worked this way for years. As with any third-party tool, use it at your own risk.

## Build from source

You need the [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) (the version is pinned in `global.json`).

```
dotnet publish Bloxstrap/Bloxstrap.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -o dist
```

The output is `dist/PlusStrapper.exe`. The code project is still called `Bloxstrap` to keep it close to upstream.

## Credits & licences

- [Bloxstrap](https://github.com/bloxstraplabs/bloxstrap) by pizzaboxer and contributors, MIT. See [LICENSE](LICENSE).
- [WPF UI](https://github.com/lepoco/wpfui), MIT, included in `wpfui/`.
- Nimbus Sans L by URW++, GPL with a font exception. See [`Bloxstrap/Resources/Fonts/Nimbus/COPYING`](Bloxstrap/Resources/Fonts/Nimbus/COPYING).

PlusStrapper is not affiliated with Roblox Corporation or the Bloxstrap team.
