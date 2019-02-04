using Harmony;
using UnityEngine;

namespace TLD_Bugfixes
{
    /*
     * When using a BreakDown item all objects on top of that item must be repositioned, or they will levitate.
     *
     * Unfortunately BreakDown selects far too many objects and ends up moving some containers that clear should be static.
     *
     * This fix prevents all Containers with the flag m_CanStickToGroundAfterBreakdown set to false from being moved either directly or via their parents.
     */

    [HarmonyPatch(typeof(BreakDown), "CanStickToGround")]
    internal class BreakDownMovingContainersFix
    {
        internal static void Postfix(GameObject go, ref bool __result)
        {
            if (!__result)
            {
                return;
            }

            Container[] containers = go.GetComponentsInChildren<Container>();
            if (containers == null)
            {
                return;
            }

            foreach (Container eachContainer in containers)
            {
                if (!eachContainer.m_CanStickToGroundAfterBreakdown)
                {
                    __result = false;
                    break;
                }
            }
        }
    }
}
