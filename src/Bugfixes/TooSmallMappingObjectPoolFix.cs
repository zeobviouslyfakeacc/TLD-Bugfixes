using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace TLD_Bugfixes {

	/*
	 * Panel_Map uses an object pool for the icons it shows on the map.
	 * However, with only 300 objects for detail icons, this pool is way too small.
	 * Regions like Forlorn Muskeg can contain up to 3 times that amount of detail icons.
	 * If the object pool is exhausted, newly discovered details will no longer appear on the map.
	 *
	 * Ideally, this bug would be fixed *not* by making that object pool bigger, but by starting
	 * with an (almost) empty object pool and growing it when more objects are required.
	 *
	 * Thanks to WulfMarius for tracking down this bug!
	 *
	 * See TLDP-5999, TLDP-6275, and probably a few more.
	 */

	[HarmonyPatch(typeof(Panel_Map), "Start", new Type[0])]
	internal static class TooSmallMappingObjectPoolFix {

		private static void Prefix(Panel_Map __instance) {
			MapDetailManager mapDetailManager = GameManager.GetMapDetailManager();
			var mapDetails = mapDetailManager.m_MapDetailObjects;
			bool startCalled = __instance.m_StartHasBeenCalled;

			if (startCalled) {
				// Grow detail pool if required
				Transform objectDetails = __instance.m_DetailEntryPoolParent;
				int detailsRequired = mapDetails.Count - objectDetails.childCount;

				if (detailsRequired > 0) {
					Panel_Map.OBJECT_POOL_SIZE = 0;
					Panel_Map.DETAIL_POOL_SIZE = detailsRequired;
					__instance.CreateObjectPools();
				}
			} else {
				Panel_Map.DETAIL_POOL_SIZE = Math.Max(Panel_Map.DETAIL_POOL_SIZE, mapDetails.Count);
			}
		}
	}
}
