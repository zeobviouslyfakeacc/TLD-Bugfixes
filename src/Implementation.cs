using MelonLoader;
using UnityEngine;

namespace TLD_Bugfixes
{
    class Implementation : MelonMod
    {
        public override void OnApplicationStart()
        {
            Debug.Log($"[{Info.Name}] Version {Info.Version} loaded!");
        }
    }
}
