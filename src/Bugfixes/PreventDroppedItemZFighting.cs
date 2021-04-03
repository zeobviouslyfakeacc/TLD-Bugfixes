using Harmony;
using UnityEngine;

namespace TLD_Bugfixes {

	/*
	 * The game exhibits some severe z-fighting if large numbers
	 * of flat items are dropped at the same position.
	 *
	 * Steps to reproduce:
	 * - Give yourself 20 books (add booka 20)
	 * - Drop all books at the same position
	 * - Move around the pile of books
	 *
	 * To fix this, when sticking an item onto a surface, we'll nudge each item
	 * by a small distance along the normal vector of the surface.
	 *
	 * Ideally, you'd fix this in GearItem#StickToGroundAndOrientOnSlope(Vector3).
	 * I just used UpdateOrientationForCorpse as it offered easy access to the normal vector.
	 */

	[HarmonyPatch(typeof(GearItem), "UpdateOrientationForCorpse")]
	internal static class PreventDroppedItemZFighting {

		private const float MAX_OFFSET = 0.005f;

		private static void Postfix(GearItem __instance, Vector3 __result) {
			float offset = Random.Range(-MAX_OFFSET, MAX_OFFSET);
			__instance.transform.localPosition += offset * __result;
		}
	}
}
