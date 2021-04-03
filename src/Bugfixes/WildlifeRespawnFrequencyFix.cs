using System;
using Harmony;

namespace TLD_Bugfixes {

	/*
	 * The "Wildlife Respawn Frequency" does exactly the opposite of what it says it does.
	 * Selecting a  low   respawn frequency  decreases  the *time* between respawns, so it  increases  the respawn frequency.
	 * Selecting a  high  respawn frequency  increases  the *time* between respawns, so it  decreases  the respawn frequency.
	 *
	 * I'm amazed how this bug has still not been fixed:
	 * - The bug has been reported multiple times through various channels.
	 * - It's simple to reproduce, easy to understand, and takes basically no time to fix.
	 * - The bug has even been marked as fixed in the v1.41 update, but neither the code nor the asset files were changed,
	 *   so after over a year, the bug is still in the game.
	 *
	 * See TLDP-5075.
	 */

	/*[HarmonyPatch(typeof(ExperienceModeManager), "GetCustomWildlifeRespawnTimeModifier", new Type[0])] //inlined
	internal static class WildlifeRespawnFrequencyFix {

		//
		// A method called GetCustomWildlifeRespawnTimeModifier should return a multiplier for the respawn time of a spawner.
		// In other words, a low respawn frequency setting should result in a large respawn time modifier.
		//

		private static readonly float[] RESPAWN_MODIFIERS = new[] { 2.0f, 1.0f, 0.7f, 0.5f };

		private static bool Prefix(ExperienceModeManager __instance, ref float __result) {
			int wildlifeSpawnFrequency = (int) __instance.GetCustomMode().m_WildlifeSpawnFrequency;
			if (wildlifeSpawnFrequency >= 0 && wildlifeSpawnFrequency < RESPAWN_MODIFIERS.Length) {
				__result = RESPAWN_MODIFIERS[wildlifeSpawnFrequency];
			} else {
				__result = 1f;
			}

			return false; // Don't run original method
		}
	}*/
}
