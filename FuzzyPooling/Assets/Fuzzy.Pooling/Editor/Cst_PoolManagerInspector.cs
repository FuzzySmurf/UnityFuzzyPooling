using Fuzzy.Helper;
using Fuzzy.Pooling;
using UnityEditor;

namespace Fuzzy.Pooling
{

    [CustomEditor(typeof(PoolManager))]
    public class Cst_PoolManagerInspector : Editor {
        public void OnEnable() {
            //Add the tags to the project when the PoolManager is first used.
            TagHelper.AddTag(PoolManagerSettings.PoolManagerTagName);
            TagHelper.AddTag(PoolManagerSettings.SpawnHandlerTagName);
        }
    }
}