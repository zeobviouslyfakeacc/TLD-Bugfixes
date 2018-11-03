using System;
using Harmony;

namespace TLD_Bugfixes {

	/*
	 * The "Wildlife Respawn Frequency" does exactly the opposite of what it says it does.
	 * Selecting a  low   respawn frequency instead  reduces    the *time* between respawns, so it  increases  the respawn frequency.
	 * Selecting a  high  respawn frequency instead  increases  the *time* between respawns, so it  decreases  the respawn frequency.
	 *
	 * This definitely qualifies for a top spot in the "dumbest bug in the game" list.
	 * This has been reported so many times by different people, through various channels and literally takes a developer
	 * 2 minutes to fix. Yet, over 6 months later, this bug is still in the game. This really baffles my mind.
	 *
	 * See TLDP-5075.
	 */

	[HarmonyPatch(typeof(ExperienceModeManager), "GetCustomWildlifeRespawnTimeModifier", new Type[0])]
	internal static class WildlifeRespawnFrequencyFix {

		/*
		 * A method called GetCustomWildlifeRespawnTimeModifier should return a multiplier for the respawn time of a spawner.
		 * In other words, a low respawn frequency setting should result in a large respawn time modifier.
		 */

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
	}
}
