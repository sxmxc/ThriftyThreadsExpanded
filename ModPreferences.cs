using MelonLoader;
using ThriftyThreadsExpanded.Utils;

namespace ThriftyThreadsExpanded;

internal static class ModPreferences
{
    private static readonly Dictionary<string, MelonPreferences_Entry<bool>> ItemEntries =
        new(StringComparer.OrdinalIgnoreCase);
    private static readonly List<MelonPreferences_Entry<bool>> EditableEntries = new();

    private static MelonPreferences_Entry<bool>? _enableNpcClothingCopies;

    public static void Initialize()
    {
        var category = MelonPreferences.CreateCategory(Constants.PreferencesCategory, Constants.ModName);

        _enableNpcClothingCopies = category.CreateEntry(
            "EnableNpcClothingCopies",
            true,
            "Enable NPC clothing copies",
            "Master switch for purchasable copies of game-owned NPC clothing at Thrifty Threads. Restart the game after changing this setting.");

        ItemEntries.Clear();
        EditableEntries.Clear();
        EditableEntries.Add(_enableNpcClothingCopies);
        foreach (var variant in ClothingVariants.All)
        {
            var entry = category.CreateEntry(
                variant.PreferenceKey,
                true,
                $"Copy NPC clothing: {variant.Name}",
                $"Creates a purchasable copy of the game-owned {variant.Name} asset used by NPCs. Restart the game after changing this setting.");
            ItemEntries[variant.PreferenceKey] = entry;
            EditableEntries.Add(entry);
        }

        category.SaveToFile(false);
    }

    public static IReadOnlyList<MelonPreferences_Entry<bool>> GetEditableEntries() => EditableEntries;

    public static bool IsEnabled(ClothingVariant variant)
    {
        if (!(_enableNpcClothingCopies?.Value ?? true))
        {
            return false;
        }

        return !ItemEntries.TryGetValue(variant.PreferenceKey, out var entry) || entry.Value;
    }
}
