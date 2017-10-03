using System;

namespace MonoLibUsb.Profile
{
    /// <summary>
    /// Describes a device arrival/removal notification event
    /// </summary>
    public class AddRemoveEventArgs : EventArgs
    {
        private readonly AddRemoveType mAddRemoveType;
        private readonly MonoUsbProfile mMonoUSBProfile;

        internal AddRemoveEventArgs(MonoUsbProfile monoUSBProfile, AddRemoveType addRemoveType)
        {
            mMonoUSBProfile = monoUSBProfile;
            mAddRemoveType = addRemoveType;
        }
        /// <summary>
        /// The <see cref ="MonoUsbProfile"/> that was added or removed.
        /// </summary>
        public MonoUsbProfile MonoUSBProfile
        {
            get { return mMonoUSBProfile; }
        }

        /// <summary>
        /// The type of event that occured.
        /// </summary>
        public AddRemoveType EventType
        {
            get { return mAddRemoveType; }
        }
    }
}