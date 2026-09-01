#if IL2CPP
using Il2CppScheduleOne.Clothing;
#else
using ScheduleOne.Clothing;
#endif

namespace ThriftyThreadsExpanded;

internal sealed class ClothingVariant
{
    public string Name { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string PreferenceKey { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AssetPath { get; set; } = string.Empty;
    public float Price { get; set; }
    public EClothingSlot Slot { get; set; }
    public EClothingApplicationType ApplicationType { get; set; }
    public bool Colorable { get; set; }
    public EClothingColor DefaultColor { get; set; }
    public ClothingDefinition? Definition { get; set; }
}
