using UnityEngine;

namespace CatQuest_Randomizer.Extentions
{
    public class FrameUpdaterExtension : MonoBehaviour
    {
        public delegate void FrameUpdated();

        public static event FrameUpdated OnFrameUpdated;

        private void Update()
        {
            // Check if the delegate has any subscribers before invoking it
            OnFrameUpdated?.Invoke();
        }
    }
}
