using Interactables.Interobjects.DoorUtils;
using Qurre.API;
using UnityEngine;

namespace Qurre.Internal.Misc
{
    internal class DoorsUpdater : MonoBehaviour
    {
        readonly float _interval = 0.1f;
        float _nextCycle = 0f;

        internal DoorVariant Door;

        private void Start()
        {
            _nextCycle = Time.time;
        }
        internal void Update()
        {
            if (Door is null)
                return;

            if (Time.time < _nextCycle)
                return;

            _nextCycle += _interval;

            try { Door.netIdentity.UpdateData(); } catch { }
        }
    }
}