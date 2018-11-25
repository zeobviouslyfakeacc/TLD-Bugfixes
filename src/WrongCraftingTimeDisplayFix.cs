using Harmony;

namespace TLD_Bugfixes {

	/*
	 * There's a bug where selecting a crafting recipe would display the wrong
	 * crafting time if the player had previously selected another crafting
	 * recipe where their tools provided a crafting time bonus.
	 *
	 * The reason this happens is because when the crafting page is refreshed from
	 * Panel_Log#RefreshCraftingVisuals and Panel_Crafting#GetModifiedCraftingDuration
	 * is called, the selected blueprint item has not yet been set.
	 * This causes Panel_Crafting#SelectedToolCanCraftSelectedItem to return an
	 * incorrect result, which causes the wrong crafting time to be shown.
	 *
	 * To fix this, either set the selected blueprint item earlier, or create a
	 * version of SelectedToolCanCraftSelectedItem that takes the blueprint item
	 * as an argument. Or honestly, just have one set of UI elements to display
	 * the crafting time, materials, etc. instead of having one per crafting recipe.
	 *
	 * See TLDP-6271.
	 */

	[HarmonyPatch(typeof(Panel_Log), "RefreshCraftingVisuals")]
	internal static class WrongCraftingTimeDisplayFix {

		private static void Prefix(Panel_Log __instance) {
			AccessTools.Method(typeof(Panel_Log), "SetOverrideBPI").Invoke(__instance, new object[0]);
		}
	}
}
