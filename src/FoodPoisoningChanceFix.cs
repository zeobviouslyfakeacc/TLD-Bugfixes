using Harmony;
using UnityEngine;

namespace TLD_Bugfixes {

	/*
	 * For some reason, food items have two cutoff values for food poisoning:
	 *         x > 0.745: No   food poisoning chance
	 * 0.745 > x > 0.200: Low  food poisoning chance
	 * 0.200 > x        : High food poisoning chance
	 *
	 * When knowing how this system works, it ruins the randomness of the food poisoning system.
	 * It's always just "check if below 20%, if yes, throw away".
	 * A 74% condition food item should not have the same food poisoning chance as a 21% condition one.
	 * So let's fix this:
	 */

	[HarmonyPatch(typeof(GearItem), "RollForFoodPoisoning", new System.Type[] { typeof(float) })]
	internal static class FoodPoisoningChanceFix {
		private static bool Prefix(GearItem __instance, ref bool __result, float startingCalories) {
			FoodItem foodItem = __instance.m_FoodItem;
			float condition = __instance.GetNormalizedCondition();

			if (!foodItem || startingCalories < 5f) {
				__result = false;
				return false;
			}
			if (foodItem.m_IsRawMeat) {
				return true; // Let the original method handle this. Raw meat is alright there
			}
			if (condition > 0.745f) {
				__result = false;
				return false;
			}

			float lowChance = foodItem.m_ChanceFoodPoisoning;
			float highChance = foodItem.m_ChanceFoodPoisoningLowCondition;

			if (lowChance > highChance) {
				return true; // No idea what's going on here, bail
			}

			float chanceFoodPoisioning = GetFoodPoisoningChance(lowChance, highChance, condition);
			__result = Utils.RollChance(chanceFoodPoisioning);

			return false; // Don't run the original method
		}

		private static float GetFoodPoisoningChance(float lowChance, float highChance, float condition) {
			float chanceAt45Percent = lowChance;
			float chanceAt25Percent = Mathf.Min(2 * lowChance, highChance);
			float chanceAt15Percent = highChance;

			if (condition > 0.45f) {
				return Mathf.Lerp(0f, chanceAt45Percent, Mathf.InverseLerp(0.745f, 0.45f, condition));
			} else if (condition > 0.25f) {
				return Mathf.Lerp(chanceAt45Percent, chanceAt25Percent, Mathf.InverseLerp(0.45f, 0.25f, condition));
			} else if (condition > 0.15f) {
				return Mathf.Lerp(chanceAt25Percent, chanceAt15Percent, Mathf.InverseLerp(0.25f, 0.15f, condition));
			} else {
				return chanceAt15Percent;
			}
		}
	}
}
