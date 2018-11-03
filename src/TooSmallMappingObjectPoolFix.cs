using System;
using System.Collections.Generic;
using Harmony;
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
			List<MapDetail> mapDetails = Traverse.Create(mapDetailManager).Field("m_MapDetailObjects").GetValue<List<MapDetail>>();
			bool startCalled = Traverse.Create(__instance).Field("m_StartHasBeenCalled").GetValue<bool>();

			if (startCalled) {
				// Grow detail pool if required
				Transform objectDetails = Traverse.Create(__instance).Field("m_DetailEntryPoolParent").GetValue<Transform>();
				int detailsRequired = mapDetails.Count - objectDetails.childCount;

				if (detailsRequired > 0) {
					Panel_Map.OBJECT_POOL_SIZE = 0;
					Panel_Map.DETAIL_POOL_SIZE = detailsRequired;
					AccessTools.Method(typeof(Panel_Map), "CreateObjectPools").Invoke(__instance, new object[0]);
				}
			} else {
				Panel_Map.DETAIL_POOL_SIZE = Math.Max(Panel_Map.DETAIL_POOL_SIZE, mapDetails.Count);
			}
		}
	}
}
