using System.Collections.Generic;
using System.Reflection;
using Harmony;

namespace TLD_Bugfixes {

	/*
	 * When "Survivor Monologue" is turned off in a custom mode game, drinking is
	 * completely silent, whereas eating still creates some sound.
	 *
	 * This might just be because the order of arguments has been mixed up when calling Launch,
	 * or maybe this was a deliberate (albeit weird) design decision. Neither the eating
	 * nor the drinking noises constitute "Survivor Monologue" in my opinion.
	 *
	 * See TLDP-6468.
	 */

	[HarmonyPatch(typeof(PlayerManager), "DrinkFromWaterSupply")]
	internal static class PlayDrinkingSoundWithoutMonologue {
		private static readonly MethodBase target = AccessTools.Method(typeof(Panel_GenericProgressBar), "Launch");
		private static readonly MethodBase replacement = AccessTools.Method(typeof(PlayDrinkingSoundWithoutMonologue), "FixArgsOrder");

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			return Transpilers.MethodReplacer(instructions, target, replacement);
		}

		private static void FixArgsOrder(Panel_GenericProgressBar progressBarPanel,
		                                 string name, float seconds, float minutes, float randomFailureThreshold,
		                                 string audioNameShouldBeVoiceName, string voiceNameShouldBeAudioName,
		                                 bool supressHeavyBreathing, bool skipRestoreInHands, OnExitDelegate del) {

			progressBarPanel.Launch(name, seconds, minutes, randomFailureThreshold,
			                        voiceNameShouldBeAudioName, audioNameShouldBeVoiceName,
			                        supressHeavyBreathing, skipRestoreInHands, del);
		}
	}
}
