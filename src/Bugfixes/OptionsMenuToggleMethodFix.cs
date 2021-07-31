using HarmonyLib;

namespace TLD_Bugfixes {

	/*
	 * There's a bug where opening and closing the options menu would change the camera settings.
	 * The most notable difference is that the contrast changes, which turns the game darker
	 * than it's supposed to be at night. The reason for this is a bug in the implementation
	 * of the Panel_OptionsMenu#ToggleGameCameraForBrightness(bool) method:
	 *
	 * ToggleGameCameraForBrightness(true) saves some camera settings (like the culling mask,
	 * or the contrast and vignetting settings) to fields and then changes these settings.
	 * ToggleGameCameraForBrightness(false) later restores the settings from the saved values.
	 *
	 * However, this only works if the method is never called with the same "toggle" argument twice.
	 * If it is called with true twice, it ends up overwriting the saved camera settings.
	 * And if it's called with false twice, you may end up reverting the settings to stale values.
	 *
	 * To fix this, we simply only call ToggleGameCameraForBrightness when toggle != lastToggle.
	 * Hinterland should investigate whether it is even necessary to call
	 * ToggleGameCameraForBrightness(false) from OnCancel() when the main tab was active.
	 *
	 * See TLDP-6574.
	 */

	[HarmonyPatch(typeof(Panel_OptionsMenu), "ToggleGameCameraForBrightness")]
	internal static class OptionsMenuToggleMethodFix {

		private static bool lastToggle = false;

		private static bool Prefix(bool toggle) {
			if (toggle == lastToggle)
				return false; // Don't run original method

			lastToggle = toggle;
			return true;
		}
	}
}
