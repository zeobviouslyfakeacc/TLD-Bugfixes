using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using UnityEngine;

namespace TLD_Bugfixes {

	/*
	 * The current weapon sway is bad for many reasons:
	 * a) It's broken. No matter where the weapon points, the bullet / arrow always flies towards the center of the screen.
	 *    And if people try to skillfully counteract the sway, they actually end up aiming away from the target.
	 * b) It's always the same sway animation. If you can't make it random, at least randomize it from a small pool...
	 * c) Instead of rotating the weapon, the current implementation mostly just moves it around the player's face.
	 *    This isn't realistic and doesn't even make you miss your target, thus missing the entire point of weapon sway.
	 * d) It also discourages aiming for any amount of time. That makes sense with a bow as you can't keep the string
	 *    drawn forever, but with a rifle, that's just lazy game design.
	 * e) Also, the player should be able to keep the rifle's sights aligned for longer than 2 seconds.
	 *
	 * To fix this, we...
	 * - Add a new sway animation that targets the shoulder joint and uses perlin noise
	 *   for a smooth yet random weapon sway animation. This rotates the weapon (and
	 *   everything else the player is holding) instead of moving it around the camera.
	 * - Scale the weapon fatigue animation parameter down for the rifle so the player
	 *   actually keeps the sights (mostly) aligned.
	 * - Make bullets and arrows actually fly towards where the player aims the bow / rifle.
	 *   This is also includes compensating for the different weapon camera FOV vs world camera FOV
	 */

	internal static class WeaponSwayFixes {

		// Sway tuning constants
		private const float SWAY_SPEED = 0.8f;
		private const float SWAY_BOW_START = 1.5f;
		private const float SWAY_BOW_FATIGUE = 1.0f;
		private const float SWAY_RIFLE_START = 0.5f;
		private const float SWAY_RIFLE_FATIGUE = 1.0f;
		private const float SWAY_PISTOL_START = 1.0f;
		private const float SWAY_PISTOL_FATIGUE = 1.0f;
		private const float SWAY_MULTIPLIER_CROUCHED = 0.75f;

		[HarmonyPatch(typeof(PlayerAnimation), "Start", new Type[0])]
		private static class AddSwayOverrideComponent {
			private static void Prefix(PlayerAnimation __instance, bool ___m_StartHasBeenCalled) {
				if (___m_StartHasBeenCalled)
					return;

				__instance.gameObject.AddComponent<SwayOverride>();
			}
		}

		[HarmonyPatch(typeof(GunItem), "Update", new Type[0])]
		private static class LimitGunSway {

			/*
			 * A tiny bit of bobbing is okay. What you guys are doing is waaaay too much.
			 * For the most part, the rifle sights should stay lined up while aiming.
			 */
			private const float FATIGUE_MULTIPLIER = 0.07f;

			private static readonly MethodInfo from = AccessTools.Method(typeof(PlayerAnimation), "Update_WeaponFatigue", new Type[] { typeof(float) });
			private static readonly MethodInfo to = AccessTools.Method(typeof(LimitGunSway), "Target");

			private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
				return Transpilers.MethodReplacer(instructions, from, to);
			}

			private static void Target(PlayerAnimation playerAnimation, float fatigue) {
				playerAnimation.Update_WeaponFatigue(fatigue * GetGunSwayScale() * FATIGUE_MULTIPLIER);
			}
		}

		[HarmonyPatch(typeof(vp_FPSShooter), "Fire", new Type[0])]
		private static class FixBulletRotation {

			/*
			 * The bullet rotation is *so close* to being correct. The main problem is that the weapon camera
			 * uses a different FOV than the camera used to render the rest of the scene.
			 * So while it may look like two objects are lined up, they're not actually lined up in the game world.
			 *
			 * Essentially, before being multiplied by the player transform's rotation, the bullet emission locator's
			 * rotation needs to be scaled by the ratio of two different FOVs, which is implemented in CompensateForFOVChange.
			 */

			private static readonly MethodInfo from = AccessTools.Method(typeof(Quaternion), "op_Multiply", new Type[] { typeof(Quaternion), typeof(Quaternion) });
			private static readonly MethodInfo to = AccessTools.Method(typeof(FixBulletRotation), "Target");

			private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
				return Transpilers.MethodReplacer(instructions, from, to);
			}

			private static Quaternion Target(Quaternion playerRot, Quaternion bulletEmissionRot) {
				return playerRot * CompensateForFOVChange(bulletEmissionRot);
			}
		}

		[HarmonyPatch(typeof(BowItem), "ShootArrow", new Type[0])]
		private static class FixArrowRotation {

			/*
			 * Arrows, on the other hand, are just broken. They spawn at a different location than where they're shown
			 * in the weapon camera (why?), and they only use the rotation of that different spawn point to determine
			 * the direction of their flight path. With how the current sway animation just moves the bow around in front
			 * of the camera, this means that no matter where the arrow points, it'll always fly straight towards the
			 * center of the screen.
			 *
			 * Instead, let's spawn the arrow at the same position as the arrow shown in the weapons camera.
			 * For the rotation, try to make it easy for the player: Use the difference between the position of the
			 * weapon camera and the tip of the arrow in the weapon camera to determine the direction of the arrow.
			 * And even though the difference between the two FOVs is smaller in this case, we should still compensate
			 * for it. This way, the arrow always flies towards where its tip points in the game world.
			 */

			private static readonly MethodInfo target = AccessTools.Method(typeof(FixArrowRotation), "Target");

			private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
				foreach (var instruction in instructions) {
					if (instruction.operand is MethodBase method && method.Name == "Instantiate") {
						instruction.operand = target;
					}
					yield return instruction;
				}
			}

			// Hinterland: Find a better way to do this
			private const float ARROW_LENGTH = 1.12f;

			private static GameObject Target(GameObject arrow, Vector3 badPos, Quaternion badRot) {
				Vector3 tipPos = arrow.transform.TransformPoint(0, 0, ARROW_LENGTH);
				Vector3 cameraPos = GameManager.GetWeaponCamera().transform.position;
				Vector3 diff = tipPos - cameraPos;
				Quaternion arrowRot = Quaternion.FromToRotation(Vector3.forward, diff);

				Transform playerTransform = GameManager.GetPlayerTransform();
				Vector3 pos = playerTransform.TransformPoint(arrow.transform.position);
				Quaternion rot = playerTransform.rotation * CompensateForFOVChange(arrowRot);
				return UnityEngine.Object.Instantiate(arrow, pos, rot);
			}
		}

		private class SwayOverride : MonoBehaviour {

			/*
			 * Custom sway animation using perlin noise.
			 */

			private readonly vp_Perlin perlin = new vp_Perlin();

			private PlayerAnimation playerAnimation;
			private float time = 0f;

			private void Awake() {
				playerAnimation = GetComponent<PlayerAnimation>();
			}

			// LateUpdate to overwrite the rotation set by the player animation
			private void LateUpdate() {
				if (GameManager.m_IsPaused)
					return;

				if (playerAnimation.IsAiming()) {
					time += Time.deltaTime;

					float magnitude = GetSwayMagnitude();
					float pitch = magnitude * perlin.Noise(SWAY_SPEED * time);
					float yaw = magnitude * perlin.Noise(-SWAY_SPEED * time);
					Quaternion rot = Quaternion.Euler(pitch, yaw, 0);

					Transform target = playerAnimation.m_ShoulderJoint.transform;
					target.localRotation = rot;
				} else if (time > 1000f) {
					time = 0f; // Make sure we don't run into float precision issues
				}
			}

			private float GetSwayMagnitude() {
				float fatigue = GameManager.GetFatigueComponent().m_CurrentFatigue / GameManager.GetFatigueComponent().m_MaxFatigue;

				GearItem itemInHands = GameManager.GetPlayerManagerComponent().m_ItemInHands;
				if (itemInHands?.m_BowItem) {
					return GetBowSwayScale() * (SWAY_BOW_START + fatigue * SWAY_BOW_FATIGUE);
				} else if (itemInHands?.m_GunItem is GunItem gunItem) {
					if (gunItem.m_GunType == GunType.Rifle) {
						return GetGunSwayScale() * (SWAY_RIFLE_START + fatigue * SWAY_RIFLE_FATIGUE);
					} else {
						return GetGunSwayScale() * (SWAY_PISTOL_START + fatigue * SWAY_PISTOL_FATIGUE);
					}
				} else {
					return 0f;
				}
			}
		}

		private static Quaternion CompensateForFOVChange(Quaternion orig) {
			float fovFrom = GameManager.GetWeaponCamera().fieldOfView;
			float fovTo = GameManager.GetMainCamera().fieldOfView;
			float fovRatio = fovTo / fovFrom;

			Quaternion cameraRot = GameManager.GetWeaponCamera().transform.rotation;
			return Quaternion.SlerpUnclamped(cameraRot, orig, fovRatio);
		}

		private static float GetGunSwayScale() {
			int tier = GameManager.GetSkillRifle().GetCurrentTierNumber();
			int reduction = GameManager.GetSkillArchery().m_SwayReduction[tier];

			float swayScale = Mathf.Clamp01(1f - reduction / 100f);
			float crouchScale = GetCrouchSwayScale();
			return swayScale * crouchScale;
		}

		private static float GetBowSwayScale() {
			float swayScale = GameManager.GetSkillArchery().GetSwayScale();
			float crouchScale = GetCrouchSwayScale();
			return swayScale * crouchScale;
		}

		private static float GetCrouchSwayScale() {
			return GameManager.GetPlayerManagerComponent().PlayerIsCrouched() ? SWAY_MULTIPLIER_CROUCHED : 1f;
		}
	}
}
