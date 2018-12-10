using Harmony;
using UnityEngine;

namespace TLD_Bugfixes
{
    /*
     * Some of the methods of InputSystemRewired used for acquiring user input first check if a Steam Controller is present.
     * 
     * This leads to thousands of invocations of IsSteamControllerActive() per second. 
     * Each invocation allocates a small array locally, which results in hundreds of kilobytes of garbage every second.
     * 
     * This fix introduces a throttle that executes the method at the most twice per second and re-uses the last result for every other call.
     */

    [HarmonyPatch(typeof(InputSystemRewired), "IsSteamControllerActive")]
    internal static class IsSteamControllerActiveFix
    {
        internal static bool lastResult;
        internal static float lastUpdate;

        private static void Postfix(bool __result)
        {
            lastResult = __result;
        }

        private static bool Prefix(ref bool __result)
        {
            if (Time.realtimeSinceStartup - lastUpdate < 0.5f)
            {
                __result = lastResult;
                return false;
            }

            lastUpdate = Time.realtimeSinceStartup;
            return true;
        }
    }
}