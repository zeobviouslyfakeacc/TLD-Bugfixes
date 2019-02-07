using Harmony;
using UnityEngine;

namespace TLD_Bugfixes {

	/*
	 * The sprint bar used to drain counter-clockwise, but now it drains clockwise.
	 * However, the fill direction of the red "sprint reduction bar" wasn't adjusted after that change,
	 * so now that red bar is drawn at the wrong location, almost always behind the white stamina bar.
	 *
	 * Fixed by setting "invert" to false on the sprint reduction's UISprite.
	 *
	 * See TLDP-6600.
	 */

	[HarmonyPatch(typeof(Panel_HUD), "Start")]
	internal static class StaminaBarReductionInvertedFix {

		private const string REDUCTION_BAR_NAME = "SprintReductionBar";

		private static void Postfix(Panel_HUD __instance) {
			Transform parent = __instance.m_SprintBar.transform;
			Transform child = parent.Find(REDUCTION_BAR_NAME);
			if (!child) {
				Debug.LogWarning("[TLD-Bugfixes] Warning: Could not fix the flipped sprint reduction bar");
				return;
			}

			UISprite redBarSprite = child.GetComponent<UISprite>();
			redBarSprite.invert = false;
		}
	}
}
