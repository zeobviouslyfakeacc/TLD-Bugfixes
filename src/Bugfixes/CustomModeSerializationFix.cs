using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace TLD_Bugfixes {

	/*
	 * There's a bug in both CustomExperienceModeManager#CreateStringFromCustomMode and CustomExperienceModeManager#CreateCustomModeFromString
	 * that prevents some settings (m_StartWeather, m_WolfSpawnDistance, m_WildlifeDetectionRange) from being (de-)serialized correctly.
	 *
	 * Ths bug can be traced back directly to Hinterland's over-use of copy & paste programming.
	 *
	 * See TLDP-6210.
	 */

	//internal static class CustomModeSerializationFix {

		//[HarmonyPatch(typeof(CustomExperienceModeManager), "CreateStringFromCustomMode")]
		//private static class FixSerialize {

			/*
			 * Is:
			 * num = (int) customMode.m_StartWeather * 7 + (int) customMode.m_WolfSpawnDistance * 3 + (int) customMode.m_WildlifeDetectionRange;
			 * Should be:
			 * num = (int) customMode.m_StartWeather * 9 + (int) customMode.m_WolfSpawnDistance * 3 + (int) customMode.m_WildlifeDetectionRange;
			 */

			/*private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
				return Replace7With9(instructions);
			}
		}

		[HarmonyPatch(typeof(CustomExperienceModeManager), "CreateCustomModeFromString")]
		private static class FixDeserialize {*/

			/*
			 * Is:
			 * num -= (customMode.m_StartWeather = (CustomTunableWeather) (num / 7)) * 7;
			 * Should be:
			 * num -= (customMode.m_StartWeather = (CustomTunableWeather) (num / 9)) * 9;
			 */

			/*private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
				return Replace7With9(instructions);
			}
		}

		private static IEnumerable<CodeInstruction> Replace7With9(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> ops = new List<CodeInstruction>(instructions);

			for (int i = 0; i < ops.Count; ++i) {
				CodeInstruction cur = ops[i];

				// Find a constant load of 7 followed by a mul or div instruction, replace it with a constant load of 9
				if (cur.opcode == OpCodes.Ldc_I4_7 && i < ops.Count - 1
						&& (ops[i + 1].opcode == OpCodes.Mul || ops[i + 1].opcode == OpCodes.Div)) {
					cur.opcode = OpCodes.Ldc_I4_S;
					cur.operand = (sbyte) 9;
				}

				yield return cur;
			}
		}
	}*/
}
