using System;
using System.Collections.Generic;
using Harmony;

namespace TLD_Bugfixes {

	/*
	 * Fixes various bad item weights where crafting or harvesting magically creates mass.
	 *
	 * Still need to fix improvised tool recipes... somehow
	 */

	internal static class ItemWeightFixes {

		private static readonly IDictionary<string, float> oldWeights = new Dictionary<string, float>();

		[HarmonyPatch(typeof(GearItem), "Awake", new Type[0])]
		private static class FixNewGearItems {
			private static void Postfix(GearItem __instance) {
				FixItemWeight(__instance);
			}
		}

		[HarmonyPatch(typeof(GearItem), "Deserialize", new Type[] { typeof(string), typeof(bool) })]
		private static class FixDeserializedGearItems {
			private static void Postfix(GearItem __instance, string text) {
				if (text != null) {
					FixItemWeight(__instance);
				}
			}
		}

		[HarmonyPatch(typeof(GearItem), "Serialize", new Type[0])]
		private static class FixSerialize {
			private static void Prefix(GearItem __instance, ref float __state) {
				__state = __instance.m_WeightKG;

				string localizationId = __instance.m_LocalizedDisplayName.m_LocalizationID;
				if (oldWeights.TryGetValue(localizationId, out float oldWeight)) {
					__instance.m_WeightKG = oldWeight;
				}
			}

			private static void Postfix(GearItem __instance, ref float __state) {
				__instance.m_WeightKG = __state;
			}
		}

		private static void FixItemWeight(GearItem gearItem) {
			string localizationId = gearItem.m_LocalizedDisplayName.m_LocalizationID;
			float targetWeight = GetWeight(localizationId);

			if (targetWeight > 0f) {
				if (!oldWeights.ContainsKey(localizationId)) {
					oldWeights.Add(localizationId, gearItem.m_WeightKG); // Memorize old weight for Serialize
				}

				gearItem.m_WeightKG = targetWeight;
			}
		}

		private static float GetWeight(string name) {
			switch (name) {
				case "GAMEPLAY_NewsprintRoll":
					// Can be harvested to 4 Tinder, i.e. 4 * 0.05 kg = 0.2 kg
					return 0.2f;
				case "GAMEPLAY_CoffeeCup":
				case "GAMEPLAY_GreenTeaCup":
				case "GAMEPLAY_ReishiTea":
				case "GAMEPLAY_RoseHipTea":
					// Use 0.25 L water and restore the same amount of hydration as 0.25 L water
					// In other words, they should weigh at least as much as 0.25 L water
					return 0.25f;
				case "GAMEPLAY_DeerHide":
				case "GAMEPLAY_DeerHideDried":
					// Needs to be heavier to be able to produce the 2 kg deerskin boots and pants (both 2x hide)
					// As deer are bigger than wolves, these hides being heavier makes sense, too
					return 1.0f;
				case "GAMEPLAY_BearHide":
				case "GAMEPLAY_BearHideDried":
					// Needs to be heavier to be able to produce the 5 kg bearskin coat (2x hide)
					// And really, if a huge bear has just 0.25 kg more hide than a wolf, something's wrong
					return 2.5f;
				case "GAMEPLAY_RabbitSkinMitts":
					// For a ~2 kg rabbit, 0.1 kg skin feel about right, so the weight of the gloves needs to be changed, instead
					return 0.5f;
				case "GAMEPLAY_Bandage":
					// 1 piece of cloth (0.1 kg) breaks down into 2 bandages. The bandages thus need to weigh less
					return 0.05f;
				case "GAMEPLAY_OldMansBeardDressing":
					// 3 * 0.01 kg old mans beard lichen = 0.03 kg old mans beard dressing
					// 0.1 kg used to make sense back when the recipe still required a bandage
					return 0.03f;
				default:
					// No change
					return 0f;
			}
		}
	}
}
