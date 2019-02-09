using System;
using Harmony;

namespace TLD_Bugfixes {

	/*
	 * Items like flares, torches, or storm lanterns are supposed to provide a
	 * small amount of heat when turned on. This is accomplished using an attached
	 * HeatSource component. However, since the v1.41 update, the maximum heat that
	 * a HeatSource can emit is set to 0 when they are turned on, so these heat
	 * sources never actually provide any heat.
	 *
	 * The only HeatSources not affected by this are fires, since adding fuel sets
	 * the maximum temperature increase to a different value.
	 *
	 * To fix this bug properly, remove
	 * m_MaxTempIncrease = m_StartingTemp;
	 * from the HeatSource#TurnOn method.
	 */

	[HarmonyPatch(typeof(HeatSource), "TurnOn", new Type[0])]
	internal static class LightSourcesNotProvidingHeatFix {

		private static void Prefix(ref float ___m_MaxTempIncrease, ref float __state) {
			__state = ___m_MaxTempIncrease;
		}

		private static void Postfix(ref float ___m_MaxTempIncrease, ref float __state) {
			___m_MaxTempIncrease = __state;
		}
	}
}
