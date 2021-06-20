using System;
using HarmonyLib;
using UnityEngine;

namespace TLD_Bugfixes {

	/*
	 * The MapDetail of a prepper cache is supposed to be added to the map after it
	 * has been discovered by a player (i.e. the player climbed down the hatch).
	 *
	 * However, this doesn't happen because PlayerManager#InteractiveObjectsProcessInteraction
	 * only looks for MapDetails in the children of m_InteractiveObjectUnderCrosshair.
	 *
	 * In the case of caches, the game object holding the MapDetail component is a parent
	 * of the game object with the collider (assigned to m_InteractiveObjectUnderCrosshair).
	 *
	 * Actually, PlayerManager#InteractiveObjectsProcessInteraction never even looks for MapDetails
	 * in the prepper cache objects at all because it immediately returns false when encountering
	 * a game object with a LoadScene component.
	 *
	 * Both of these bugs would need to be fixed for caches to appear on players' maps again.
	 */

	[HarmonyPatch(typeof(PlayerManager), "InteractiveObjectsProcessInteraction", new Type[0])]
	internal static class CachesNeverAddedToMapFix {

		private static void Prefix(PlayerManager __instance) {
			GameObject gameObject = __instance.m_InteractiveObjectUnderCrosshair;
			if (!gameObject)
				return;

			MapDetail detail = gameObject.GetComponentInParent<MapDetail>();
			if (detail && detail.enabled && !detail.m_IsUnlocked && detail.m_RequiresInteraction) {
				detail.Unlock();
			}
		}
	}
}
