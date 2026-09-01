#if IL2CPP
using Il2CppScheduleOne.Clothing;
#else
using ScheduleOne.Clothing;
#endif

namespace ThriftyThreadsExpanded;

internal static class ClothingVariants
{
    public static readonly IReadOnlyList<ClothingVariant> All = new ClothingVariant[]
    {
        new() { Name = "Police Tactical Vest", Id = "tte_npc_police_vest", PreferenceKey = "CopyNpcPoliceVest", Description = "A wearable copy of the tactical vest used by police NPCs. Cosmetic only.", Price = 240f, AssetPath = "Avatar/Accessories/Chest/BulletproofVest/BulletproofVest_Police", Slot = EClothingSlot.Outerwear, ApplicationType = EClothingApplicationType.Accessory },
        new() { Name = "Police Cap", Id = "tte_npc_police_cap", PreferenceKey = "CopyNpcPoliceCap", Description = "A wearable copy of the navy cap used by police NPCs.", Price = 85f, AssetPath = "Avatar/Accessories/head/policecap/PoliceCap", Slot = EClothingSlot.Head, ApplicationType = EClothingApplicationType.Accessory },
        new() { Name = "Respirator", Id = "tte_npc_respirator", PreferenceKey = "CopyNpcRespirator", Description = "A wearable copy of the respirator used by NPCs. Cosmetic only.", Price = 120f, AssetPath = "Avatar/Accessories/head/respirator/Respirator", Slot = EClothingSlot.Head, ApplicationType = EClothingApplicationType.Accessory },
        new() { Name = "Hazmat Suit", Id = "tte_npc_hazmat_suit", PreferenceKey = "CopyNpcHazmatSuit", Description = "A wearable copy of the full-body hazmat suit used by NPCs.", Price = 300f, AssetPath = "Avatar/Accessories/waist/hazmatsuit/HazmatSuit", Slot = EClothingSlot.Outerwear, ApplicationType = EClothingApplicationType.Accessory, Colorable = true, DefaultColor = EClothingColor.Yellow },
        new() { Name = "Police Utility Belt", Id = "tte_npc_police_belt", PreferenceKey = "CopyNpcPoliceBelt", Description = "A wearable copy of the equipment belt used by police NPCs. Cosmetic only.", Price = 110f, AssetPath = "Avatar/Accessories/waist/policebelt/PoliceBelt", Slot = EClothingSlot.Waist, ApplicationType = EClothingApplicationType.Accessory },
        new() { Name = "Fast Food Staff Shirt", Id = "tte_npc_fast_food_shirt", PreferenceKey = "CopyNpcFastFoodShirt", Description = "A wearable copy of the uniform shirt used by fast-food NPCs.", Price = 65f, AssetPath = "Avatar/layers/top/FastFood T-Shirt", Slot = EClothingSlot.Top, ApplicationType = EClothingApplicationType.BodyLayer },
        new() { Name = "Fast Food Staff Cap", Id = "tte_npc_fast_food_cap", PreferenceKey = "CopyNpcFastFoodCap", Description = "A wearable copy of the cap used by fast-food NPCs.", Price = 55f, AssetPath = "Avatar/Accessories/Head/Cap/Cap_FastFood", Slot = EClothingSlot.Head, ApplicationType = EClothingApplicationType.Accessory },
        new() { Name = "Gas Station Staff Shirt", Id = "tte_npc_gas_station_shirt", PreferenceKey = "CopyNpcGasStationShirt", Description = "A wearable copy of the uniform shirt used by gas-station NPCs.", Price = 70f, AssetPath = "Avatar/layers/top/GasStation T-Shirt", Slot = EClothingSlot.Top, ApplicationType = EClothingApplicationType.BodyLayer },
        new() { Name = "Tucked T-Shirt", Id = "tte_npc_tucked_shirt", PreferenceKey = "CopyNpcTuckedShirt", Description = "A wearable copy of the neatly tucked shirt used by NPCs.", Price = 45f, AssetPath = "Avatar/layers/top/Tucked T-Shirt", Slot = EClothingSlot.Top, ApplicationType = EClothingApplicationType.BodyLayer, Colorable = true, DefaultColor = EClothingColor.White },
        new() { Name = "Formal Clergy Gown", Id = "tte_npc_formal_gown", PreferenceKey = "CopyNpcFormalGown", Description = "A wearable copy of the long formal gown used by a clergy NPC.", Price = 180f, AssetPath = "Avatar/Accessories/Waist/PriestGown/PriestGown", Slot = EClothingSlot.Outerwear, ApplicationType = EClothingApplicationType.Accessory }
    };
}
