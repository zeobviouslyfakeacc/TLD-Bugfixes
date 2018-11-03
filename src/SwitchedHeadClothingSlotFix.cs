using System;
using System.Reflection;
using Harmony;
using UnityEngine;

namespace TLD_Bugfixes {

	/*
	 * The inner and outer hat slot are switched.
	 * The inner slot should be closer to the paper doll, the outer one further away.
	 *
	 * The hat slot used to be on the right side of the clothing panel, but was then
	 * moved to the left side without switching the clothing slots.
	 *
	 * See TLDP-5100.
	 */

	[HarmonyPatch(typeof(Panel_Clothing), "Start", new Type[0])]
	internal class SwitchedHeadClothingSlotFix {

		private const string CONTAINER_PATH = "NonPaperDoll/ClothingSlotsUI";
		private const string INNER_NAME = "ClothingSlotHead1";
		private const string OUTER_NAME = "ClothingSlotHead2";
		private static readonly FieldInfo COLUMN_FIELD = AccessTools.Field(typeof(ClothingSlot), "m_LayoutColumnIndex");

		private static void Prefix(Panel_Clothing __instance) {
			Transform container = __instance.transform.Find(CONTAINER_PATH);
			Transform innerTransform = container?.Find(INNER_NAME);
			Transform outerTransform = container?.Find(OUTER_NAME);
			if (!innerTransform || !outerTransform || innerTransform.localPosition.x > outerTransform.localPosition.x) {
				Debug.LogWarning("[TLD-Bugfixes] Warning: Could not apply head clothing slot fix");
			}

			// Switch positions
			Vector3 innerPos = innerTransform.localPosition;
			Vector3 outerPos = outerTransform.localPosition;
			innerTransform.localPosition = outerPos;
			outerTransform.localPosition = innerPos;

			// Fix column indices
			ClothingSlot innerSlot = innerTransform.GetComponentInChildren<ClothingSlot>();
			ClothingSlot outerSlot = outerTransform.GetComponentInChildren<ClothingSlot>();
			COLUMN_FIELD.SetValue(innerSlot, 1);
			COLUMN_FIELD.SetValue(outerSlot, 0);
		}
	}
}
