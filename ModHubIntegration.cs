using System.Reflection;
using MelonLoader;
using ThriftyThreadsExpanded.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace ThriftyThreadsExpanded;

internal static class ModHubIntegration
{
    private const string ModHubAssemblyName = "ModHub.Core";
    private const string ModHubCoreTypeName = "ModHub.Core.ModHubCore";
    private const string ModHubUiTypeName = "ModHub.Core.ModHubUI";

    private static Type? _modHubUiType;
    private static bool _registered;

    public static void TryRegister()
    {
        if (_registered)
        {
            return;
        }

        try
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(candidate => string.Equals(
                    candidate.GetName().Name,
                    ModHubAssemblyName,
                    StringComparison.OrdinalIgnoreCase));
            if (assembly is null)
            {
                Log.Message("ModHub was not detected; MelonPreferences remain available through the standard config file.");
                return;
            }

            var coreType = assembly.GetType(ModHubCoreTypeName, false);
            _modHubUiType = assembly.GetType(ModHubUiTypeName, false);
            var rendererType = assembly.GetType("ModHub.Core.CustomContentRenderer", false);
            var registerMethod = coreType?.GetMethod(
                "RegisterCustomContent",
                BindingFlags.Public | BindingFlags.Static);
            var renderMethod = typeof(ModHubIntegration).GetMethod(
                nameof(RenderSettings),
                BindingFlags.NonPublic | BindingFlags.Static);

            if (_modHubUiType is null || rendererType is null || registerMethod is null || renderMethod is null)
            {
                Log.Warning("ModHub was detected, but its custom settings API is unavailable. Falling back to automatic MelonPreferences discovery.");
                return;
            }

            var renderer = Delegate.CreateDelegate(rendererType, renderMethod);
            registerMethod.Invoke(null, new object[] { Constants.ModName, renderer });
            _registered = true;
            Log.Message("Registered the settings panel with ModHub.");
        }
        catch (Exception exception)
        {
            Log.Warning($"Could not register the optional ModHub settings panel: {exception.GetBaseException().Message}");
        }
    }

    private static void RenderSettings(Transform contentParent, Font font)
    {
        if (_modHubUiType is null)
        {
            return;
        }

        try
        {
            InvokeUi("CreateSectionHeader", "NPC CLOTHING COPIES", contentParent, font, 13);
            CreateInformationCard(contentParent, font);

            foreach (var entry in ModPreferences.GetEditableEntries())
            {
                CreateToggleCard(contentParent, font, entry);
            }
        }
        catch (Exception exception)
        {
            Log.Error($"ModHub failed to render Thrifty Threads Expanded settings: {exception.GetBaseException()}");
        }
    }

    private static void CreateInformationCard(Transform parent, Font font)
    {
        var card = (GameObject?)InvokeUi("CreateCard", "NpcClothingNotice", parent, 72f, null);
        if (!card)
        {
            return;
        }

        var text = (Text?)InvokeUi(
            "CreateText",
            "These options create purchasable copies of game-owned clothing used by NPCs. No game assets are bundled. Restart the game after changing a setting.",
            card!.transform,
            font,
            11,
            FontStyle.Normal);
        if (!text)
        {
            return;
        }

        text!.alignment = TextAnchor.MiddleLeft;
        var rect = text.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(14f, 6f);
        rect.offsetMax = new Vector2(-14f, -6f);
    }

    private static void CreateToggleCard(
        Transform parent,
        Font font,
        MelonPreferences_Entry<bool> entry)
    {
        var card = (GameObject?)InvokeUi("CreateCard", $"Preference_{entry.Identifier}", parent, 52f, null);
        if (!card)
        {
            return;
        }

        var label = (Text?)InvokeUi(
            "CreateText",
            entry.DisplayName ?? entry.Identifier,
            card!.transform,
            font,
            13,
            FontStyle.Normal);
        if (label)
        {
            label!.alignment = TextAnchor.MiddleLeft;
            var labelRect = label.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0f, 0f);
            labelRect.anchorMax = new Vector2(0.78f, 1f);
            labelRect.offsetMin = new Vector2(14f, 0f);
            labelRect.offsetMax = Vector2.zero;
        }

        Action<bool> onChanged = value =>
        {
            entry.Value = value;
            MelonPreferences.Save();
        };
        var toggleResult = InvokeUi("CreateToggle", card.transform, font, entry.Value, 0.22f, onChanged);
        var toggleObject = toggleResult?.GetType().GetField("Item1")?.GetValue(toggleResult) as GameObject;
        if (toggleObject)
        {
            toggleObject!.GetComponent<RectTransform>().anchoredPosition = new Vector2(-14f, 0f);
        }
    }

    private static object? InvokeUi(string methodName, params object?[] arguments)
    {
        var methods = _modHubUiType!.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(method => method.Name == methodName && method.GetParameters().Length == arguments.Length)
            .ToArray();
        if (methods.Length != 1)
        {
            throw new MissingMethodException(ModHubUiTypeName, methodName);
        }

        return methods[0].Invoke(null, arguments);
    }
}
