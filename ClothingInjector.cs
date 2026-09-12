#if IL2CPP
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppScheduleOne;
using Il2CppScheduleOne.Clothing;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.ItemFramework;
#else
using ScheduleOne;
using ScheduleOne.Clothing;
using ScheduleOne.DevUtilities;
using ScheduleOne.ItemFramework;
#endif
using ThriftyThreadsExpanded.Utils;
using S1API.Items;
using ApiClothingApplicationType = S1API.Items.Clothing.ClothingApplicationType;
using ApiClothingColor = S1API.Items.Clothing.ClothingColor;
using ApiClothingItemCreator = S1API.Items.Clothing.ClothingItemCreator;
using ApiClothingSlot = S1API.Items.Clothing.ClothingSlot;
using UnityEngine;

namespace ThriftyThreadsExpanded;

internal static class ClothingInjector
{
    private static readonly object RegistrationGate = new();
    private static bool _registered;
    private static bool _registrationInProgress;
    private static bool _warnedAboutBaseDefinition;

    public static void RegisterAll()
    {
        lock (RegistrationGate)
        {
            if (_registered || _registrationInProgress)
            {
                return;
            }

            _registrationInProgress = true;
        }

        try
        {
            RegisterAllCore();
        }
        finally
        {
            lock (RegistrationGate)
            {
                _registrationInProgress = false;
            }
        }
    }

    private static void RegisterAllCore()
    {
        var registry = Singleton<Registry>.Instance;
        if (registry?.ItemRegistry is null)
        {
            Log.Warning("Registry is not ready; clothing registration will be retried.");
            return;
        }

        var baseDefinition = FindBaseDefinition(registry);
        if (!baseDefinition && !_warnedAboutBaseDefinition)
        {
            Log.Warning($"Could not find base item '{Constants.BaseDefinitionName}'. New items will use reduced defaults.");
            _warnedAboutBaseDefinition = true;
        }

        var enabledCount = 0;
        var registeredCount = 0;
        foreach (var variant in ClothingVariants.All)
        {
            variant.Definition = null;
            if (!ModPreferences.IsEnabled(variant))
            {
                continue;
            }

            enabledCount++;
            if (TryFindExisting(registry, variant.Id, out var existing))
            {
                variant.Definition = existing;
                registeredCount++;
                continue;
            }

            var definition = BuildDefinition(variant, baseDefinition, registry);
            if (!definition)
            {
                Log.Error($"Failed to create ClothingDefinition for '{variant.Id}'.");
                continue;
            }

            variant.Definition = definition;
            registeredCount++;
            Log.Message($"Registered clothing item: {variant.Name} ({variant.Id}).");
        }

        _registered = registeredCount == enabledCount;
        if (!_registered)
        {
            Log.Warning($"Registered {registeredCount}/{enabledCount} enabled clothing items; missing items will be retried.");
        }
        else
        {
            Log.Message($"Registered {registeredCount} enabled NPC clothing copies.");
        }
    }

    private static ClothingDefinition? FindBaseDefinition(Registry registry)
    {
        foreach (var entry in registry.ItemRegistry)
        {
            var definition = entry?.Definition;
            if (definition && definition!.name == Constants.BaseDefinitionName)
            {
#if IL2CPP
                return ((Il2CppObjectBase)definition).TryCast<ClothingDefinition>();
#else
                return definition as ClothingDefinition;
#endif
            }
        }

        return null;
    }

    private static bool TryFindExisting(Registry registry, string id, out ClothingDefinition? result)
    {
        foreach (var entry in registry.ItemRegistry)
        {
            var definition = entry?.Definition;
            if (!definition || !string.Equals(definition!.ID, id, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

#if IL2CPP
            result = ((Il2CppObjectBase)definition).TryCast<ClothingDefinition>();
#else
            result = definition as ClothingDefinition;
#endif
            return result;
        }

        result = null;
        return false;
    }

    private static ClothingDefinition? BuildDefinition(
        ClothingVariant variant,
        ClothingDefinition? baseDefinition,
        Registry registry)
    {
        try
        {
            var builder = baseDefinition
                ? ApiClothingItemCreator.CloneFrom(baseDefinition!.ID)
                : ApiClothingItemCreator.CreateBuilder();
            if (builder is null)
            {
                return null;
            }

            builder
                .WithBasicInfo(variant.Id, variant.Name, variant.Description, ItemCategory.Clothing)
                .WithPricing(variant.Price)
                .WithStackLimit(1)
                .WithSlot((ApiClothingSlot)(int)variant.Slot)
                .WithApplicationType((ApiClothingApplicationType)(int)variant.ApplicationType)
                .WithClothingAsset(variant.AssetPath)
                .WithColorable(variant.Colorable)
                .WithDefaultColor((ApiClothingColor)(int)variant.DefaultColor)
                .WithBlockedSlots();

            var icon = IconLoader.Load(variant.Id);
            if (icon)
            {
                builder.WithIcon(icon!);
            }

            builder.Build();
            return TryFindExisting(registry, variant.Id, out var definition) ? definition : null;
        }
        catch (Exception exception)
        {
            // S1API registers the native definition before completing Build().
            // If a later step fails, the next registration attempt must adopt
            // that definition instead of inserting the same ID again.
            if (TryFindExisting(registry, variant.Id, out var partiallyRegistered))
            {
                Log.Warning($"Recovered partially registered clothing item '{variant.Id}' after build failure: {exception.GetBaseException().Message}");
                return partiallyRegistered;
            }

            Log.Error($"S1API failed to build clothing item '{variant.Id}': {exception}");
            return null;
        }
    }
}
