using System;
using Harmony;
using UnityEngine;

namespace TLD_Bugfixes {

	/*
	 * Before TLD v1.33, the game tried to prevent the player from eating ruined food items, but there were still ways
	 * around these restrictions. For example, one could drop a ruined food item on the ground and eat it by pressing the
	 * space key. Similarly, one could cook ruined meat to get 50% cooked meat items with only a low food poisoning chance.
	 *
	 * Then, in v1.33, with the introduction of the new cooking mechanics, all of these restrictions for ruined food
	 * suddenly vanished. Meat can now be stored forever at 0% condition. And when eaten after aquiring the level 5
	 * cooking skill, these items don't even carry a risk of food poisoning.
	 *
	 * This is most likely an unintended regression, so let's fix it.
	 */

	internal static class PreventEatingRuinedFood {

		[HarmonyPatch(typeof(Panel_Inventory), "AllowUseAtZeroHP", new Type[] { typeof(GearItem) })]
		private static class DontAllowUseAtZeroHP {

			private static bool Prefix(ref bool __result, GearItem gi) {
				if (gi.m_FoodItem) {
					__result = false;
					return false;
				}
				return true;
			}
		}

		[HarmonyPatch(typeof(PlayerManager), "CanUseFoodInventoryItem", new Type[] { typeof(GearItem) })]
		private static class PreventnUseFoodInventoryItem {

			private static bool Prefix(ref bool __result, GearItem gi) {
				if (gi.IsWornOut()) {
					GameAudioManager.PlayGUIError();
					__result = false;
					return false;
				}
				return true;
			}
		}

		[HarmonyPatch(typeof(ItemDescriptionPage), "GetEquipButtonLocalizationId", new Type[] { typeof(GearItem) })]
		private static class DontShowEatButtonForRuinedFood {

			private static bool Prefix(ref string __result, GearItem gi) {
				if (gi && gi.m_FoodItem && gi.IsWornOut()) {
					__result = string.Empty;
					return false;
				}
				return true;
			}
		}

		[HarmonyPatch(typeof(CookingPotItem), "SetCookedGearProperties", new Type[] { typeof(GearItem), typeof(GearItem) })]
		private static class RuinedFoodRemainsRuinedWhenCooked {

			private static void Postfix(GearItem rawItem, GearItem cookedItem) {
				if (!rawItem || !cookedItem)
					return;

				if (rawItem.IsWornOut()) {
					cookedItem.ForceWornOut();
					cookedItem.UpdateDamageShader();
				}
			}
		}

		[HarmonyPatch(typeof(Cookable), "MaybeStartWarmingUpDueToNearbyFire", new Type[0])]
		private static class PreventWarmingUpRuinedFood {

			private static bool Prefix(Cookable __instance) {
				GearItem gearItem = __instance.GetComponent<GearItem>();
				return !gearItem.IsWornOut(); // Do not run original method when item is ruined
			}
		}

		/*
		 * Allow harvesting the cans of canned food items. The idea is that while
		 * the food inside the can may be ruined, the can itself should still be usable.
		 */

		[HarmonyPatch(typeof(FoodItem), "Awake", new Type[0])]
		private static class AllowBreakingDownFoodForContainer {

			private static void Postfix(FoodItem __instance) {
				GameObject item = __instance.gameObject;

				if (!item.GetComponent<Harvest>() && __instance.m_GearPrefabHarvestAfterFinishEatingNormal) {
					GearItem resultItem = __instance.m_GearPrefabHarvestAfterFinishEatingNormal.GetComponent<GearItem>();

					Harvest harvest = item.AddComponent<Harvest>();
					harvest.m_YieldGear = new GearItem[] { resultItem };
					harvest.m_YieldGearUnits = new int[] { 1 };
					harvest.m_DurationMinutes = 5;
					harvest.m_Audio = "Play_OpenCan";

					// Retroactively cache Harvest in GearItem
					GearItem baseGear = __instance.GetComponent<GearItem>();
					baseGear.m_Harvest = harvest;
				}
			}
		}
	}
}
