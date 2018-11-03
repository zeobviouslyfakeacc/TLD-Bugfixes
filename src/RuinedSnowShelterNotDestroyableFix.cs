using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;

namespace TLD_Bugfixes {

	/*
	 * Ruined snow shelters obviously shouldn't be accessible to the player, but there's
	 * no reason why players shouldn't be able to deconstruct these ruined snow shelters.
	 *
	 * This is so out of place that it has to be considered a bug.
	 *
	 * See TLDP-5443.
	 */

	internal static class RuinedSnowShelterNotDestroyableFix {

		private const int STICKS_FROM_RUINED_DISMANTLE = 4;
		private const int CLOTH_FROM_RUINED_DISMANTLE = 2;

		[HarmonyPatch(typeof(SnowShelter), "ProcessInteraction", new Type[0])]
		private static class AllowOpeningInterface {
			private static void Postfix(ref bool __result, SnowShelter __instance) {
				if (!__result && __instance.IsRuined() && !GameManager.GetSnowShelterManager().PlayerEnteringShelter()) {
					InterfaceManager.m_Panel_SnowShelterInteract.SetSnowShelterInstance(__instance);
					InterfaceManager.m_Panel_SnowShelterInteract.Enable(true);
					__result = true;
				}
			}
		}

		[HarmonyPatch(typeof(Panel_SnowShelterInteract), "Update", new Type[0])]
		private static class KeepInterfaceOpenWhenRuined {

			private static readonly MethodInfo originalMethod = AccessTools.Method(typeof(SnowShelter), "IsRuined");
			private static readonly MethodInfo returnsFalseMethod = AccessTools.Method(typeof(KeepInterfaceOpenWhenRuined), "IsRuined");

			private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
				foreach (CodeInstruction instruction in instructions) {
					if (instruction.opcode == OpCodes.Callvirt && instruction.operand == originalMethod) {
						instruction.opcode = OpCodes.Call;
						instruction.operand = returnsFalseMethod;
					}
					yield return instruction;
				}
			}

			private static bool IsRuined(SnowShelter snowShelter /* ignored, clear from operand stack */) {
				return false;
			}
		}

		[HarmonyPatch(typeof(Panel_SnowShelterInteract), "UpdateButtonLegend", new Type[0])]
		private static class DisableUseInButtonLegendWhenRuined {
			private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
				List<CodeInstruction> ops = new List<CodeInstruction>(instructions);
				int i = 0;

				// Find m_ButtonLegendContainer.UpdateButton("Continue", "GAMEPLAY_Use", true, 1, true);
				for (; i < ops.Count; ++i) {
					CodeInstruction op = ops[i];
					yield return op;

					if (op.opcode == OpCodes.Ldstr && (string) op.operand == "GAMEPLAY_Use")
						break;
				}

				// Skip constant load of true and call ShouldEnableUseButton instead
				i += 2;
				MethodInfo targetMethod = AccessTools.Method(typeof(DisableUseInButtonLegendWhenRuined), "ShouldEnableUseButton");
				yield return new CodeInstruction(OpCodes.Call, targetMethod);

				// Emit the rest of the instructions
				for (; i < ops.Count; ++i) {
					yield return ops[i];
				}
			}

			private static bool ShouldEnableUseButton() {
				return !IsSnowShelterRuined(InterfaceManager.m_Panel_SnowShelterInteract);
			}
		}

		[HarmonyPatch(typeof(Panel_SnowShelterInteract), "UpdateRepairButton", new Type[0])]
		private static class ShowUseButtonAsDisabledWhenRuined {
			private static void Postfix(Panel_SnowShelterInteract __instance) {
				Panel_Inventory_Examine_MenuItem menuItem = __instance.m_Button_Use.gameObject.GetComponent<Panel_Inventory_Examine_MenuItem>();
				menuItem.SetDisabled(IsSnowShelterRuined(__instance));
			}
		}

		[HarmonyPatch(typeof(Panel_SnowShelterInteract), "UpdatePanelDisplays", new Type[0])]
		private static class DisableBottomRightUseButtonWhenRuined {
			private static void Postfix(Panel_SnowShelterInteract __instance) {
				if (IsSnowShelterRuined(__instance)) {
					Utils.SetActive(__instance.m_BottomRightActionButton, false);
				}
			}
		}

		[HarmonyPatch(typeof(Panel_SnowShelterInteract), "NeedsRepair", new Type[0])]
		private static class DisableRepairButtonWhenRuined {
			private static bool Prefix(ref bool __result, Panel_SnowShelterInteract __instance) {
				if (IsSnowShelterRuined(__instance)) {
					__result = false; // Don't let it be repaired
					return false; // Don't run the original method
				}
				return true;
			}
		}

		[HarmonyPatch(typeof(Panel_SnowShelterInteract), "RefreshRepairPanel", new Type[0])]
		private static class HideRepairErrorMessageWhenRuined {

			/*
			 * There would be an error message saying that the snow shelter is in perfect condition.
			 * We don't have translations for all languages, so let's just disable it instead.
			 */

			private static void Postfix(Panel_SnowShelterInteract __instance) {
				if (IsSnowShelterRuined(__instance)) {
					__instance.m_ErrorLabel.gameObject.SetActive(false);
				}
			}
		}

		[HarmonyPatch(typeof(Panel_SnowShelterInteract), "Start", new Type[0])]
		private static class SetUseButtonDisabledColors {
			private static void Postfix(Panel_SnowShelterInteract __instance) {
				Panel_Inventory_Examine_MenuItem repairItem = __instance.m_Button_Repair.GetComponent<Panel_Inventory_Examine_MenuItem>();
				Panel_Inventory_Examine_MenuItem useItem = __instance.m_Button_Use.gameObject.GetComponent<Panel_Inventory_Examine_MenuItem>();

				// Copy disabled colors from repair button
				useItem.m_TextColor_Disabled = repairItem.m_TextColor_Disabled;
				useItem.m_TextColor_DisabledHover = repairItem.m_TextColor_DisabledHover;
				useItem.m_TextColor_DisabledSelected = repairItem.m_TextColor_DisabledSelected;
			}
		}

		[HarmonyPatch(typeof(Panel_SnowShelterInteract), "RefreshDismantlePanel", new Type[0])]
		private static class AdjustMaterialsFromRuinedDismantle {
			private static void Prefix(Panel_SnowShelterInteract __instance) {
				SnowShelter snowShelter = Traverse.Create(__instance).Field("m_SnowShelter").GetValue<SnowShelter>();
				if (snowShelter && snowShelter.IsRuined()) {
					snowShelter.m_NumSticksFromDismantle = STICKS_FROM_RUINED_DISMANTLE;
					snowShelter.m_NumClothFromDismantle = CLOTH_FROM_RUINED_DISMANTLE;
				}
			}
		}

		private static bool IsSnowShelterRuined(Panel_SnowShelterInteract panel) {
			SnowShelter snowShelter = Traverse.Create(panel).Field("m_SnowShelter").GetValue<SnowShelter>();
			return snowShelter && snowShelter.IsRuined();
		}
	}
}
