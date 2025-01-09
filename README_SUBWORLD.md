# Adding Custom `Subword`

If you want to add your `Subworld` into subworld select UI of this mod, you need to write:
```c#
Mod.Call(1, [subworld], [icon], [tooltip], [condition]);
```

### Where:
- [subworld] - is your subworld path (for ```SubworldSystem.Enter```)
- [icon] - is a `Asset<Texture2D>` or `null`
- [tooltip] - is a `LocalizedText` or `null`
- [condition] - is a `System.Func<bool>` or `null`
- 
## Different Situations:
1. Only add subworld
```c#
Mod.Call(1, "MyMod/MySubworld", null, null, null);
```
2. Add subworld with custom icon
```c#
Mod.Call(1, "MyMod/MySubworld", Main.Assets.Request<Texture2D>("Images/UI/IconCorruption"), null, null);
```
3. Add subworld with custom tooltip
```c#
Mod.Call(1, "MyMod/MySubworld", null, Language.GetText("Mods.MyMod.Anything"), null);
```
4. Add subworld with custom condition (For example defeated moonlord)
```c#
Mod.Call(1, "MyMod/MySubworld", null, null, () => { return NPC.downedMoonlord; });
```