# Adding Custom `GenPass`
If you want to add your `GenPass` into subworlds of this mod, you need to write:
```c#
Mod.Call(0, [pass], [index 1], [index N]...);
```
### Where:
- [pass] - is your pass name
- [index n] - is a comma separated list of indices of worlds to which it will be applied

## Full list of subworld indices:
```
Pre-Moonlord Seeds
1 - "Normal" (clone of main world)
2 - "Purity" (clone of main world without "Corruption" GenPass)
3 - "Purity RNG" (same as "Purity", but random seed)
4 - "Corruption" (clone of main world with corruption world evil)
5 - "Corruption RNG" (same as "Corruption", but random seed)
6 - "Crimson" (clone of main world with crimson world evil)
7 - "Crimson RNG" (same as "Crimson", but random seed)

Secret Seeds (they all RNG)
8 - Zenith ("getfixedboy")
9 - Drunk World
10 - For The Worthy
11 - Not The Bees
12 - Celebration 10
13 - The Constant
14 - Don't Dig Up
15 - No Traps
```
## Different Situations:
1. Adding new GenPass (For example "Nuclear Crator" in vanilla worlds)
```c#
Mod.Call(0, "Nuclear Crator", 1,2,3,4,5,6,7);
```
```c#
public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
{
    // Adding "Nuclear Crator" GenTask 
}
```
2. You need to delete GenPass (For example "Clay" in purity worlds)
```c#
Mod.Call(0, "Clay", 2,3);
```
```c#
public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
{
    var clay = tasks.Find((t) => t.Name == "Clay");
    if (clay != null)
    {
        tasks.Remove(clay); 
        totalWeight -= clay.Weight;
    }
}
```
3. You need to return GenPass, even if it is disabled in the config (For example "Floating Islands" in 'clone' world)
```c#
Mod.Call(0, "Floating Islands", 1);
```