namespace DreadsMashedPatch.AssetForwarding;

public enum ForwardingRule
{
    Meshes,
    Textures,
    Appearance,
    Animations,
    Sounds,
    Music,
    Scripts,
    IconsAndImages,
    Trees
}

/// <summary>
/// Fixed forwarding policy while resource/appearance controls are unavailable.
/// Keep switches here for a future opt-in UI; these are not serialized settings.
/// Trees retain their separate user setting in ForwardingSettings.
/// </summary>
public static class FixedForwardingRules
{
    public const bool Meshes = true;
    public const bool Textures = true;
    public const bool Appearance = true;
    public const bool Animations = true;
    public const bool Sounds = true;
    public const bool Music = true;
    public const bool Scripts = true;
    public const bool IconsAndImages = true;

    public static bool Allows(ForwardingRule rule) => rule switch
    {
        ForwardingRule.Meshes => Meshes,
        ForwardingRule.Textures => Textures,
        ForwardingRule.Appearance => Appearance,
        ForwardingRule.Animations => Animations,
        ForwardingRule.Sounds => Sounds,
        ForwardingRule.Music => Music,
        ForwardingRule.Scripts => Scripts,
        ForwardingRule.IconsAndImages => IconsAndImages,
        _ => throw new ArgumentOutOfRangeException(nameof(rule))
    };
}
