using System.Collections.Generic;
using Harmony;
using UnityEngine;

namespace TLD_Bugfixes {

	/*
	 * The math used to determine how many scroll bar steps there should be in
	 * the list of crafting recipes is wrong. The following code
	 * -
	 * Mathf.CeilToInt((m_FilteredBlueprintItemList.Count - 12) / 2f);
	 * -
	 * is only correct for 13 and 14 crafting recipes, for which 2 scroll bar
	 * steps is the correct amount. But for 15 and 16 recipes, which would
	 * still fit into 4 rows (-> 2 scroll bar steps), that code returns 3.
	 * In general, the original code sets a too large amount of slider steps,
	 * which leads to overscrolling.
	 */

	/*[HarmonyPatch(typeof(Panel_Log), "RefreshBlueprintsSlider")]
	internal static class BlueprintListScrollBarFix {

		private static bool Prefix(Panel_Log __instance, List<BlueprintItem> ___m_FilteredBlueprintItemList) {
			GameObject scrollBar = __instance.m_ScrollbarBlueprints;
			int numRows = (___m_FilteredBlueprintItemList.Count + 3) / 4;

			if (numRows > 3) {
				scrollBar.SetActive(true);

				UISlider uiSlider = scrollBar.GetComponentInChildren<UISlider>(true);
				if (uiSlider) {
					uiSlider.numberOfSteps = 1 + (numRows - 3);
				}
			} else {
				scrollBar.SetActive(false);
			}

			return false; // Don't run the original method
		}
	}*/
}
