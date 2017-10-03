using System;
using System.Runtime.InteropServices;
using MonoLibUsb.Transfer;

namespace MonoLibUsb
{
    /// <summary>
    /// Asynchronous transfer callback delegate
    /// </summary>
    /// <param name="transfer">The transfer previously allocated with <see cref="MonoUsbApi.AllocTransfer"/>.</param>
    [UnmanagedFunctionPointer(MonoUsbApi.CC)]
    public delegate void MonoUsbTransferDelegate(MonoUsbTransfer transfer);

    /// <summary>
    /// Callback delegate, invoked when a new file descriptor should be added to the set of file descriptors monitored for events. 
    /// </summary>
    /// <param name="fd">The new file descriptor.</param>
    /// <param name="events">Events to monitor for, see PollfdItem for a description.</param>
    /// <param name="user_data">User data pointer specified in <see cref="MonoUsbApi.SetPollfdNotifiers"/> call.</param>
    [UnmanagedFunctionPointer(MonoUsbApi.CC)]
    public delegate void PollfdAddedDelegate(int fd, short events, IntPtr user_data);

    /// <summary>
    /// Callback delegate, invoked when a file descriptor should be removed from the set of file descriptors being monitored for events.
    /// </summary>
    /// <remarks>After returning from this callback, do not use that file descriptor again. </remarks>
    /// <param name="fd">The file descriptor to stop monitoring.</param>
    /// <param name="user_data">User data pointer specified in <see cref="MonoUsbApi.SetPollfdNotifiers"/> call.</param>
    [UnmanagedFunctionPointer(MonoUsbApi.CC)]
    public delegate void PollfdRemovedDelegate(int fd, IntPtr user_data);
}