using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using UnityEngine;

namespace TLD_Bugfixes {

	/*
	 * When "Survivor Monologue" is turned off in a custom mode game, the player
	 * still monologues about a treatment not working, e.g. when drinking medicinal
	 * tea without having any afflictions.
	 *
	 * See TLDP-6339.
	 */

	[HarmonyPatch(typeof(PlayerManager), "OnFirstAidComplete")]
	internal static class MuteTreatmentFailedWithoutMonologue {
		private static readonly MethodBase target = AccessTools.Method(typeof(GameAudioManager), "PlaySound",
				new Type[] { typeof(string), typeof(GameObject) });
		private static readonly MethodBase replacement = AccessTools.Method(typeof(MuteTreatmentFailedWithoutMonologue), "PlayVoice");

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			return Transpilers.MethodReplacer(instructions, target, replacement);
		}

		private static uint PlayVoice(string soundID, GameObject goIgnored) {
			return GameManager.GetPlayerVoiceComponent().PlayCritical(soundID);
		}
	}
}
