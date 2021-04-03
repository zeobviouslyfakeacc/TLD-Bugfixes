using Harmony;
using UnityEngine;

namespace TLD_Bugfixes {

	/*
	 * In the hunting lodge, there are areas that are a few degrees colder
	 * than the rest of the building. This is because the InteriorTemperatureTrigger
	 * was copied & pasted from a Coastal Highway house and the size of its
	 * collider was not adjusted to the larger hunting lodge scene.
	 */

	[HarmonyPatch(typeof(GameManager), "Start")]
	internal static class HuntingLodgeTemperatureTriggerFix {

		private const string TRIGGER_NAME = "/Root/Design/Scripting/TRIGGER_InteriorTemperatureCoastalHouse";

		private static void Postfix(GameManager __instance) {
			if (__instance.gameObject.scene.name != "HuntingLodgeA")
				return;

			GameObject interiorTrigger = GameObject.Find(TRIGGER_NAME);
			BoxCollider collider = interiorTrigger?.GetComponent<BoxCollider>();

			if (!collider) {
				MelonLoader.MelonLogger.LogWarning("Could not fix the interior temperature in the Hunting Lodge");
				return;
			}

			collider.size = new Vector3(100, 100, 100);
		}
	}
}
