using ChainedPuzzles;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MTFO.Custom.CCP.Components
{
    internal sealed class ClusterPuzzleData : MonoBehaviour
    {
        public Il2CppValueField<uint> PersistentID;

        public CP_Bioscan_Hud[] ChildHuds = Array.Empty<CP_Bioscan_Hud>();
    }
}
