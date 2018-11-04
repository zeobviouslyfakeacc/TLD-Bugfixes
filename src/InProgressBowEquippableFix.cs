using System;
using Harmony;

namespace TLD_Bugfixes {

	/*
	 * The player can craft a bow for 0.5 of 5 hours, which will not consume any raw materials and creates a new in-progress bow.
	 * The player can then drop that bow onto the floor, inspect it, and press space to equip it.
	 * Provided that the player has at least one arrow in their inventory, the bow will be equipped and working like a fully crafted bow.
	 *
	 * See TLDP-5339.
	 */

	[HarmonyPatch(typeof(PlayerManager), "ItemShouldEquipOnPickup", new Type[] { typeof(GearItem) })]
	internal static class InProgressBowEquippableFix {
		private static bool Prefix(ref bool __result, GearItem gi) {
			if (gi && gi.m_BowItem && gi.m_BowItem.CanEquipWithArrow()) {
				bool craftingInProgress = gi.m_InProgressCraftItem && !gi.m_InProgressCraftItem.IsProgressComplete();
				__result = !gi.IsWornOut() && !craftingInProgress;
				return false;
			}
			return true;
		}
	}
}
