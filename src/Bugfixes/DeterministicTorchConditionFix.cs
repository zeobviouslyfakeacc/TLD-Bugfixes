using System;
using HarmonyLib;

namespace TLD_Bugfixes {

	/*
	 * The condition of a torch taken from a fire is not random.
	 * Instead, it is deterministically initialized, like all other gear condition.
	 * Unfortunately, this makes the torch condition exploitable. If a player doesn't move and continues to take and then
	 * extinguish torches, the torches will all have the same condition (as their name, position, and scene are equal).
	 */

	//[HarmonyPatch(typeof(GearItem), "OverrideGearCondition")]
	//internal static class DeterministicTorchConditionFix {

		/*
		 * This fix isn't pretty. It relies on quite a few assumptions.
		 * E.g. nothing but Panel_FeedFire#OnTakeTorch using GearItem#OverrideGearCondition.
		 *
		 * Ideally, you'd fix this bug by splitting RollGearCondition into RollRandomGearCondition and RollDeterministicGearCondition
		 * RollDeterministicGearCondition would set the random seed, call RollRandomGearCondition, and reset the random seed.
		 *
		 * Even more ideally, you'd stop using *one single random object* for all of your randomness.
		 */

		/*private static readonly Random INDEPENDENT_RANDOM = new Random();

		private static void Prefix(GearItem __instance, ref string __state) {
			// Save old name
			__state = __instance.gameObject.name;
			// Randomly salt the gameObject name
			__instance.gameObject.name += "__TEMP__" + INDEPENDENT_RANDOM.Next();
		}

		private static void Postfix(GearItem __instance, ref string __state) {
			// Reset gameObject name
			if (__state != null) {
				__instance.gameObject.name = __state;
			}
		}
	}*/
}
