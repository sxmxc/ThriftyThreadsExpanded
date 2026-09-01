using HarmonyLib;

#if IL2CPP
using Il2CppScheduleOne;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.UI.Shop;
#else
using ScheduleOne;
using ScheduleOne.DevUtilities;
using ScheduleOne.ItemFramework;
using ScheduleOne.UI.Shop;
#endif

namespace ThriftyThreadsExpanded;

[HarmonyPatch(typeof(Registry), nameof(Registry.Awake))]
internal static class RegistryAwakePatch
{
    [HarmonyPostfix]
    private static void Postfix() => ClothingInjector.RegisterAll();
}

[HarmonyPatch(typeof(ClothingShopInterface), "Start")]
internal static class ClothingShopStartPatch
{
    private static readonly HashSet<int> InjectedShopInstances = new();

    [HarmonyPostfix]
    private static void Postfix(ClothingShopInterface __instance)
    {
        ClothingInjector.RegisterAll();

        if (__instance?.Listings is null)
        {
            Log.Warning("Clothing shop was not ready for listing injection.");
            return;
        }

        if (!InjectedShopInstances.Add(__instance.GetInstanceID()))
        {
            return;
        }

        var added = 0;
        foreach (var variant in ClothingVariants.All)
        {
            if (!ModPreferences.IsEnabled(variant))
            {
                continue;
            }

            if (!variant.Definition)
            {
                Log.Warning($"Skipped shop listing '{variant.Id}' because its definition is unavailable.");
                continue;
            }

            if (ContainsListing(__instance, variant.Id))
            {
                continue;
            }

            var listing = new ClothingShopListing
            {
                Item = variant.Definition,
                name = variant.Name,
                OverridePrice = true,
                OverriddenPrice = variant.Price,
                Color = variant.DefaultColor
            };
            __instance.Listings.Add(listing);
            listing.Initialize(__instance);
            __instance.CreateListingUI(listing);
            added++;
        }

        Log.Message($"Added {added} Thrifty Threads Expanded listings to the clothing shop.");
    }

    private static bool ContainsListing(ClothingShopInterface shop, string itemId)
    {
        foreach (var listing in shop.Listings)
        {
            if (listing?.Item is not null && string.Equals(listing.Item.ID, itemId, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}

[HarmonyPatch(typeof(ClothingShopInterface), nameof(ClothingShopInterface.HandoverItems))]
internal static class ClothingShopHandoverPatch
{
    [HarmonyPrefix]
    private static void Prefix(ClothingShopInterface __instance, ref bool __state)
    {
        __state = false;
        foreach (var listing in __instance.Listings)
        {
            if (listing?.Item is null || listing.QuantityInCart <= 0 || !IsCustomItem(listing.Item.ID))
            {
                continue;
            }

            __state = true;
            Log.Message($"Checkout handover started: {listing.Item.ID} x{listing.QuantityInCart}.");
        }
    }

    [HarmonyPostfix]
    private static void Postfix(bool __state, bool __result)
    {
        if (__state)
        {
            Log.Message($"Checkout handover returned: {__result}.");
        }
    }

    private static bool IsCustomItem(string itemId)
    {
        foreach (var variant in ClothingVariants.All)
        {
            if (string.Equals(variant.Id, itemId, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
