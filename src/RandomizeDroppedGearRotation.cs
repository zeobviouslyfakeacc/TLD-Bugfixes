using Harmony;
using UnityEngine;

namespace TLD_Bugfixes {

	/*
	 * When an item is dropped, it always faces the same direction.
	 * This doesn't make much sense, and players have started to abuse this behavior.
	 * Instead, let's just randomize it.
	 */

	[HarmonyPatch(typeof(Utils), "GetOrientationOnSlope")]
	internal static class RandomizeDroppedGearRotation {

		private static void Postfix(ref Quaternion __result, Transform t, Vector3 groundNormal) {
			Quaternion oldRotation = __result;
			Quaternion rotateAroundNormal = Quaternion.AngleAxis(Random.Range(0f, 360f), groundNormal);
			__result = rotateAroundNormal * oldRotation;
		}
	}
}
