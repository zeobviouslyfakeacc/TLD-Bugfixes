using System.Collections.Generic;
using System.Reflection;
using Harmony;

namespace TLD_Bugfixes {

	/*
	 * If the player opens the inventory panel (or any other panel with a hotkey) from
	 * the tool selection menu when crafting, they'll trigger the "eternal crafting" bug.
	 *
	 * Reproduction: See TLDP-6482.
	 *
	 * Part of this issue stems from the fact that when the tool selection panel is enabled,
	 * the log panel is still enabled, too. That makes CanExecutePanelActivateAction return
	 * true, which allows opening other panels like the inventory panel.
	 * So to fix this, we don't just use InterfaceManager.m_Panel_Log.IsEnabled(), but rather
	 * (InterfaceManager.m_Panel_Log.IsEnabled() && !InterfaceManager.m_Panel_GearSelect.IsEnabled()).
	 *
	 * See TLDP-6482, TLDP-6261, TLDP-6255, and probably others.
	 */

	[HarmonyPatch(typeof(InputManager), "CanExecutePanelActivateAction")]
	internal static class EternalCraftingFix {

		private static readonly MethodBase target = AccessTools.Method(typeof(Panel_Log), "IsEnabled");
		private static readonly MethodBase replacement = AccessTools.Method(typeof(EternalCraftingFix), "FixedLogEnabled");

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			return Transpilers.MethodReplacer(instructions, target, replacement);
		}

		private static bool FixedLogEnabled(Panel_Log panel) {
			return panel.IsEnabled() && !InterfaceManager.m_Panel_GearSelect.IsEnabled();
		}
	}
}
