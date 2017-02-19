#pragma warning disable 3021
/*  This file is part of SevenZipSharp.

    SevenZipSharp is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    SevenZipSharp is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with SevenZipSharp.  If not, see <http://www.gnu.org/licenses/>.
*/

#define DOTNET20
#define UNMANAGED

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
#if !WINCE
using System.Runtime.Remoting.Messaging;
#endif
#if DOTNET20
using System.Threading;
#else
using System.Windows.Threading;
#endif
#if MONO
using SevenZip.Mono.COM;
#endif

namespace SevenZip
{
#if UNMANAGED

    /// <summary>
    /// The way of the event synchronization.
    /// </summary>
    public enum EventSynchronizationStrategy
    {
        /// <summary>
        /// Events are called synchronously if user can do some action; that is, cancel the execution process for example.
        /// </summary>
        Default,
        /// <summary>
        /// Always call events asynchronously.
        /// </summary>
        AlwaysAsynchronous,
        /// <summary>
        /// Always call events synchronously.
        /// </summary>
        AlwaysSynchronous
    }

    /// <summary>
    /// SevenZip Extractor/Compressor base class. Implements Password string, ReportErrors flag.
    /// </summary>
    public abstract class SevenZipBase : MarshalByRefObject
    {
        private readonly string _password;
        private readonly bool _reportErrors;
        private readonly int _uniqueID;
        private static readonly List<int> Identificators = new List<int>();
#if !WINCE
        internal static readonly AsyncCallback AsyncCallbackImplementation = AsyncCallbackMethod;

        /// <summary>
        /// True if the instance of the class needs to be recreated in new thread context; otherwise, false.
        /// </summary>
        protected internal bool NeedsToBeRecreated;

        /// <summary>
        /// AsyncCallback implementation used in asynchronous invocations.
        /// </summary>
        /// <param name="ar">IAsyncResult instance.</param>
        internal static void AsyncCallbackMethod(IAsyncResult ar)
        {
            var result = (AsyncResult)ar;
            result.AsyncDelegate.GetType().GetMethod("EndInvoke").Invoke(result.AsyncDelegate, new[] { ar });
            ((SevenZipBase)ar.AsyncState).ReleaseContext();
        }

        virtual internal void SaveContext(
#if !DOTNET20
DispatcherPriority priority = DispatcherPriority.Normal
#endif
)
        {
#if !DOTNET20
            Dispatcher = Dispatcher.CurrentDispatcher;
            Priority = priority;
#else
            Context = SynchronizationContext.Current;
#endif
            NeedsToBeRecreated = true;
        }

        virtual internal void ReleaseContext()
        {
#if !DOTNET20
            Dispatcher = null;
#else
            Context = null;
#endif
            NeedsToBeRecreated = true;
            GC.SuppressFinalize(this);
        }

        private delegate void EventHandlerDelegate<T>(EventHandler<T> handler, T e) where T : EventArgs;

        internal void OnEvent<T>(EventHandler<T> handler, T e, bool synchronous) where T : EventArgs
        {
            try
            {
                if (handler != null)
                {
                    switch (EventSynchronization)
                    {
                        case EventSynchronizationStrategy.AlwaysAsynchronous:
                            synchronous = false;
                            break;
                        case EventSynchronizationStrategy.AlwaysSynchronous:
                            synchronous = true;
                            break;
                    }
                    if (
#if !DOTNET20
Dispatcher == null
#else
                        Context == null
#endif
)
                    {
                        // Usual synchronous call
                        handler(this, e);
                    }
                    else
                    {
#if !DOTNET20
                        var eventHandlerDelegate = new EventHandlerDelegate<T>((h, ee) => h(this, ee));
                        if (synchronous)
                        {
                            // Could be just handler(this, e);
                            Dispatcher.Invoke(eventHandlerDelegate, Priority, handler, e);
                        }
                        else
                        {
                            Dispatcher.BeginInvoke(eventHandlerDelegate, Priority, handler, e);
                        }
#else
                    var callback = new SendOrPostCallback((obj) =>
                    {
                        var array = (object[])obj;
                        ((EventHandler<T>)array[0])(array[1], (T)array[2]);
                    });
                    if (synchronous)
                    {
                        // Could be just handler(this, e);
                        this.Context.Send(callback, new object[] { handler, this, e });
                    }
                    else
                    {
                        this.Context.Post(callback, new object[] { handler, this, e });
                    }
#endif
                    }
                }
            }
            catch (Exception ex)
            {
                AddException(ex);
            }
        }

#if !DOTNET20
        /// <summary>
        /// Gets or sets the Dispatcher object for this instance.
        /// It will be used to fire events in the user context.
        /// </summary>
        internal Dispatcher Dispatcher { get; set; }

        /// <summary>
        /// Gets or sets the Dispatcher priority of calling user events.
        /// </summary>
        internal DispatcherPriority Priority { get; set; }
#else
        internal SynchronizationContext Context { get; set; }
#endif
        /// <summary>
        /// Gets or sets the event synchronization strategy.
        /// </summary>
        public EventSynchronizationStrategy EventSynchronization { get; set; }
#else // WINCE
        internal void OnEvent<T>(EventHandler<T> handler, T e, bool synchronous) where T : System.EventArgs
        {
            try
            {
                handler(this, e);
            }
            catch (Exception ex)
            {
                AddException(ex);
            }
        }
#endif
        /// <summary>
        /// Gets the unique identificator of this SevenZipBase instance.
        /// </summary>
        public int UniqueID
        {
            get
            {
                return _uniqueID;
            }
        }

        /// <summary>
        /// User exceptions thrown during the requested operations, for example, in events.
        /// </summary>
        private readonly List<Exception> _exceptions = new List<Exception>();

        private static int GetUniqueID()
        {
            lock(Identificators)
            {
                
            int id;
            var rnd = new Random(DateTime.Now.Millisecond);
            do
            {
                id = rnd.Next(Int32.MaxValue);
            }
            while (Identificators.Contains(id));
            Identificators.Add(id);
            return id;
            }
        }

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the SevenZipBase class.
        /// </summary>
        protected SevenZipBase()
        {
            _password = "";
            _reportErrors = true;
            _uniqueID = GetUniqueID();
        }

        /// <summary>
        /// Initializes a new instance of the SevenZipBase class
        /// </summary>
        /// <param name="password">The archive password.</param>
        protected SevenZipBase(string password)
        {
            if (String.IsNullOrEmpty(password))
            {
                throw new SevenZipException("Empty password was specified.");
            }
            _password = password;
            _reportErrors = true;
            _uniqueID = GetUniqueID();
        }
        #endregion

        /// <summary>
        /// Removes the UniqueID from the list.
        /// </summary>
        ~SevenZipBase()
        {
            // This lock probably isn't necessary but just in case...
            lock (Identificators)
            {
                Identificators.Remove(_uniqueID);
            }
        }

        /// <summary>
        /// Gets or sets the archive password
        /// </summary>
        public string Password
        {
            get
            {
                return _password;
            }
        }

        /// <summary>
        /// Gets or sets throw exceptions on archive errors flag
        /// </summary>
        internal bool ReportErrors
        {
            get
            {
                return _reportErrors;
            }
        }

        /// <summary>
        /// Gets the user exceptions thrown during the requested operations, for example, in events.
        /// </summary>
        internal ReadOnlyCollection<Exception> Exceptions
        {
            get
            {
                return new ReadOnlyCollection<Exception>(_exceptions);
            }
        }

        internal void AddException(Exception e)
        {
            _exceptions.Add(e);
        }

        internal void ClearExceptions()
        {
            _exceptions.Clear();
        }

        internal bool HasExceptions
        {
            get
            {
                return _exceptions.Count > 0;
            }
        }

        /// <summary>
        /// Throws the specified exception when is able to.
        /// </summary>
        /// <param name="e">The exception to throw.</param>
        /// <param name="handler">The handler responsible for the exception.</param>
        internal bool ThrowException(CallbackBase handler, params Exception[] e)
        {
            if (_reportErrors && (handler == null || !handler.Canceled))
            {
                throw e[0];
            }
            return false;
        }

        internal void ThrowUserException()
        {
            if (HasExceptions)
            {
                throw new SevenZipException(SevenZipException.USER_EXCEPTION_MESSAGE);
            }
        }

        /// <summary>
        /// Throws exception if HRESULT != 0.
        /// </summary>
        /// <param name="hresult">Result code to check.</param>
        /// <param name="message">Exception message.</param>
        /// <param name="handler">The class responsible for the callback.</param>
        internal void CheckedExecute(int hresult, string message, CallbackBase handler)
        {
            if (hresult != (int)OperationResult.Ok || handler.HasExceptions)
            {
                if (!handler.HasExceptions)
                {
                    if (hresult < -2000000000)
                    {
                        ThrowException(handler,
                                       new SevenZipException(
                                           "The execution has failed due to the bug in the SevenZipSharp.\n" +
                                           "Please report about it to http://sevenzipsharp.codeplex.com/WorkItem/List.aspx, post the release number and attach the archive."));
                    }
                    else
                    {
                        ThrowException(handler,
                                       new SevenZipException(message + hresult.ToString(CultureInfo.InvariantCulture) +
                                                             '.'));
                    }
                }
                else
                {
                    ThrowException(handler, handler.Exceptions[0]);
                }
            }
        }

#if !WINCE && !MONO
        /// <summary>
        /// Changes the path to the 7-zip native library.
        /// </summary>
        /// <param name="libraryPath">The path to the 7-zip native library.</param>
        public static void SetLibraryPath(string libraryPath)
        {
            SevenZipLibraryManager.SetLibraryPath(libraryPath);
        }
#endif
        /// <summary>
        /// Gets the current library features.
        /// </summary>
        [CLSCompliant(false)]
        public static LibraryFeature CurrentLibraryFeatures
        {
            get
            {
                return SevenZipLibraryManager.CurrentLibraryFeatures;
            }
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current SevenZipBase.
        /// </summary>
        /// <param name="obj">The System.Object to compare with the current SevenZipBase.</param>
        /// <returns>true if the specified System.Object is equal to the current SevenZipBase; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            var inst = obj as SevenZipBase;
            if (inst == null)
            {
                return false;
            }
            return _uniqueID == inst._uniqueID;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns> A hash code for the current SevenZipBase.</returns>
        public override int GetHashCode()
        {
            return _uniqueID;
        }

        /// <summary>
        /// Returns a System.String that represents the current SevenZipBase.
        /// </summary>
        /// <returns>A System.String that represents the current SevenZipBase.</returns>
        public override string ToString()
        {
            var type = "SevenZipBase";
            if (this is SevenZipExtractor)
            {
                type = "SevenZipExtractor";
            }
            /*
            if (this is SevenZipCompressor)
            {
                type = "SevenZipCompressor";
            }
             */
            return string.Format("{0} [{1}]", type, _uniqueID);
        }
    }

    internal class CallbackBase : MarshalByRefObject
    {
        private readonly string _password;
        private readonly bool _reportErrors;
        /// <summary>
        /// User exceptions thrown during the requested operations, for example, in events.
        /// </summary>
        private readonly List<Exception> _exceptions = new List<Exception>();

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the CallbackBase class.
        /// </summary>
        protected CallbackBase()
        {
            _password = "";
            _reportErrors = true;
        }

        /// <summary>
        /// Initializes a new instance of the CallbackBase class.
        /// </summary>
        /// <param name="password">The archive password.</param>
        protected CallbackBase(string password)
        {
            if (String.IsNullOrEmpty(password))
            {
                throw new SevenZipException("Empty password was specified.");
            }
            _password = password;
            _reportErrors = true;
        }
        #endregion

        /// <summary>
        /// Gets or sets the archive password
        /// </summary>
        public string Password
        {
            get
            {
                return _password;
            }
        }

        /// <summary>
        /// Gets or sets the value indicating whether the current procedure was cancelled.
        /// </summary>
        public bool Canceled { get; set; }

        /// <summary>
        /// Gets or sets throw exceptions on archive errors flag
        /// </summary>
        public bool ReportErrors
        {
            get
            {
                return _reportErrors;
            }
        }

        /// <summary>
        /// Gets the user exceptions thrown during the requested operations, for example, in events.
        /// </summary>
        public ReadOnlyCollection<Exception> Exceptions
        {
            get
            {
                return new ReadOnlyCollection<Exception>(_exceptions);
            }
        }

        public void AddException(Exception e)
        {
            _exceptions.Add(e);
        }

        public void ClearExceptions()
        {
            _exceptions.Clear();
        }

        public bool HasExceptions
        {
            get
            {
                return _exceptions.Count > 0;
            }
        }

        /// <summary>
        /// Throws the specified exception when is able to.
        /// </summary>
        /// <param name="e">The exception to throw.</param>
        /// <param name="handler">The handler responsible for the exception.</param>
        public bool ThrowException(CallbackBase handler, params Exception[] e)
        {
            if (_reportErrors && (handler == null || !handler.Canceled))
            {
                throw e[0];
            }
            return false;
        }

        /// <summary>
        /// Throws the first exception in the list if any exists.
        /// </summary>
        /// <returns>True means no exceptions.</returns>
        public bool ThrowException()
        {
            if (HasExceptions && _reportErrors)
            {
                throw _exceptions[0];
            }
            return true;
        }

        public void ThrowUserException()
        {
            if (HasExceptions)
            {
                throw new SevenZipException(SevenZipException.USER_EXCEPTION_MESSAGE);
            }
        }
    }

    /// <summary>
    /// Struct for storing information about files in the 7-zip archive.
    /// </summary>
    public struct ArchiveFileInfo
    {
        /// <summary>
        /// Gets or sets index of the file in the archive file table.
        /// </summary>
        [CLSCompliant(false)]
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets file name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the file last write time.
        /// </summary>
        public DateTime LastWriteTime { get; set; }

        /// <summary>
        /// Gets or sets the file creation time.
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the file creation time.
        /// </summary>
        public DateTime LastAccessTime { get; set; }

        /// <summary>
        /// Gets or sets size of the file (unpacked).
        /// </summary>
        [CLSCompliant(false)]
        public ulong Size { get; set; }

        /// <summary>
        /// Gets or sets CRC checksum of the file.
        /// </summary>
        [CLSCompliant(false)]
        public uint Crc { get; set; }

        /// <summary>
        /// Gets or sets file attributes.
        /// </summary>
        [CLSCompliant(false)]
        public uint Attributes { get; set; }

        /// <summary>
        /// Gets or sets being a directory.
        /// </summary>
        public bool IsDirectory { get; set; }

        /// <summary>
        /// Gets or sets being encrypted.
        /// </summary>
        public bool Encrypted { get; set; }

        /// <summary>
        /// Gets or sets comment for the file.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current ArchiveFileInfo.
        /// </summary>
        /// <param name="obj">The System.Object to compare with the current ArchiveFileInfo.</param>
        /// <returns>true if the specified System.Object is equal to the current ArchiveFileInfo; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return (obj is ArchiveFileInfo) ? Equals((ArchiveFileInfo)obj) : false;
        }

        /// <summary>
        /// Determines whether the specified ArchiveFileInfo is equal to the current ArchiveFileInfo.
        /// </summary>
        /// <param name="afi">The ArchiveFileInfo to compare with the current ArchiveFileInfo.</param>
        /// <returns>true if the specified ArchiveFileInfo is equal to the current ArchiveFileInfo; otherwise, false.</returns>
        public bool Equals(ArchiveFileInfo afi)
        {
            return afi.Index == Index && afi.FileName == FileName;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns> A hash code for the current ArchiveFileInfo.</returns>
        public override int GetHashCode()
        {
            return FileName.GetHashCode() ^ Index;
        }

        /// <summary>
        /// Returns a System.String that represents the current ArchiveFileInfo.
        /// </summary>
        /// <returns>A System.String that represents the current ArchiveFileInfo.</returns>
        public override string ToString()
        {
            return "[" + Index.ToString(CultureInfo.CurrentCulture) + "] " + FileName;
        }

        /// <summary>
        /// Determines whether the specified ArchiveFileInfo instances are considered equal.
        /// </summary>
        /// <param name="afi1">The first ArchiveFileInfo to compare.</param>
        /// <param name="afi2">The second ArchiveFileInfo to compare.</param>
        /// <returns>true if the specified ArchiveFileInfo instances are considered equal; otherwise, false.</returns>
        public static bool operator ==(ArchiveFileInfo afi1, ArchiveFileInfo afi2)
        {
            return afi1.Equals(afi2);
        }

        /// <summary>
        /// Determines whether the specified ArchiveFileInfo instances are not considered equal.
        /// </summary>
        /// <param name="afi1">The first ArchiveFileInfo to compare.</param>
        /// <param name="afi2">The second ArchiveFileInfo to compare.</param>
        /// <returns>true if the specified ArchiveFileInfo instances are not considered equal; otherwise, false.</returns>
        public static bool operator !=(ArchiveFileInfo afi1, ArchiveFileInfo afi2)
        {
            return !afi1.Equals(afi2);
        }
    }

    /// <summary>
    /// Archive property struct.
    /// </summary>
    public struct ArchiveProperty
    {
        /// <summary>
        /// Gets the name of the archive property.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the value of the archive property.
        /// </summary>
        public object Value { get; internal set; }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current ArchiveProperty.
        /// </summary>
        /// <param name="obj">The System.Object to compare with the current ArchiveProperty.</param>
        /// <returns>true if the specified System.Object is equal to the current ArchiveProperty; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return (obj is ArchiveProperty) ? Equals((ArchiveProperty)obj) : false;
        }

        /// <summary>
        /// Determines whether the specified ArchiveProperty is equal to the current ArchiveProperty.
        /// </summary>
        /// <param name="afi">The ArchiveProperty to compare with the current ArchiveProperty.</param>
        /// <returns>true if the specified ArchiveProperty is equal to the current ArchiveProperty; otherwise, false.</returns>
        public bool Equals(ArchiveProperty afi)
        {
            return afi.Name == Name && afi.Value == Value;
        }

        /// <summary>
        ///  Serves as a hash function for a particular type.
        /// </summary>
        /// <returns> A hash code for the current ArchiveProperty.</returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Value.GetHashCode();
        }

        /// <summary>
        /// Returns a System.String that represents the current ArchiveProperty.
        /// </summary>
        /// <returns>A System.String that represents the current ArchiveProperty.</returns>
        public override string ToString()
        {
            return Name + " = " + Value;
        }

        /// <summary>
        /// Determines whether the specified ArchiveProperty instances are considered equal.
        /// </summary>
        /// <param name="afi1">The first ArchiveProperty to compare.</param>
        /// <param name="afi2">The second ArchiveProperty to compare.</param>
        /// <returns>true if the specified ArchiveProperty instances are considered equal; otherwise, false.</returns>
        public static bool operator ==(ArchiveProperty afi1, ArchiveProperty afi2)
        {
            return afi1.Equals(afi2);
        }

        /// <summary>
        /// Determines whether the specified ArchiveProperty instances are not considered equal.
        /// </summary>
        /// <param name="afi1">The first ArchiveProperty to compare.</param>
        /// <param name="afi2">The second ArchiveProperty to compare.</param>
        /// <returns>true if the specified ArchiveProperty instances are not considered equal; otherwise, false.</returns>
        public static bool operator !=(ArchiveProperty afi1, ArchiveProperty afi2)
        {
            return !afi1.Equals(afi2);
        }
    }

#if COMPRESS

    /// <summary>
    /// Archive compression mode.
    /// </summary>
    public enum CompressionMode
    {
        /// <summary>
        /// Create a new archive; overwrite the existing one.
        /// </summary>
        Create,
        /// <summary>
        /// Add data to the archive.
        /// </summary>
        Append,
    }

    internal enum InternalCompressionMode
    {
        /// <summary>
        /// Create a new archive; overwrite the existing one.
        /// </summary>
        Create,
        /// <summary>
        /// Add data to the archive.
        /// </summary>
        Append,
        /// <summary>
        /// Modify archive data.
        /// </summary>
        Modify
    }

    /// <summary>
    /// Zip encryption method enum.
    /// </summary>
    public enum ZipEncryptionMethod
    {
        /// <summary>
        /// ZipCrypto encryption method.
        /// </summary>
        ZipCrypto,
        /// <summary>
        /// AES 128 bit encryption method.
        /// </summary>
        Aes128,
        /// <summary>
        /// AES 192 bit encryption method.
        /// </summary>
        Aes192,
        /// <summary>
        /// AES 256 bit encryption method.
        /// </summary>
        Aes256
    }

    /// <summary>
    /// Archive update data for UpdateCallback.
    /// </summary>
    internal struct UpdateData
    {
        public uint FilesCount;
        public InternalCompressionMode Mode;

        public Dictionary<int, string> FileNamesToModify { get; set; }

        public List<ArchiveFileInfo> ArchiveFileData { get; set; }
    }
#endif
#endif
}