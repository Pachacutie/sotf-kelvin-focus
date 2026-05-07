using RedLoader;
using SUI;

namespace SOTF_KELVIN_FOCUS;

public static class Config
{
    public static ConfigCategory Category { get; private set; }

    //public static ConfigEntry<bool> SomeEntry { get; private set; }

    public static void Init()
    {
        Category = ConfigSystem.CreateFileCategory("SOTF_KELVIN_FOCUS", "SOTF_KELVIN_FOCUS", "SOTF_KELVIN_FOCUS.cfg");

        // SomeEntry = Category.CreateEntry(
        //     "some_entry",
        //     true,
        //     "Some entry",
        //     "Some entry that does some stuff.");
    }

    // Same as the callback in "CreateSettings". Called when the settings ui is closed.
    public static void OnSettingsUiClosed()
    {
    }
}