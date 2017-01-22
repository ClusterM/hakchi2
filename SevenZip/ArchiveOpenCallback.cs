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

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
#if MONO
using SevenZip.Mono;
using SevenZip.Mono.COM;
#endif

namespace SevenZip
{
    #if UNMANAGED
    /// <summary>
    /// Callback to handle the archive opening
    /// </summary>
    internal sealed class ArchiveOpenCallback : CallbackBase, IArchiveOpenCallback, IArchiveOpenVolumeCallback,
                                                ICryptoGetTextPassword, IDisposable
    {
        private FileInfo _fileInfo;
        private Dictionary<string, InStreamWrapper> _wrappers = 
            new Dictionary<string, InStreamWrapper>();
        private readonly List<string> _volumeFileNames = new List<string>();

        /// <summary>
        /// Gets the list of volume file names.
        /// </summary>
        public IList<string> VolumeFileNames
        {
            get
            {
                return _volumeFileNames;
            }
        }

        /// <summary>
        /// Performs the common initialization.
        /// </summary>
        /// <param name="fileName">Volume file name.</param>
        private void Init(string fileName)
        {
            if (!String.IsNullOrEmpty(fileName))
            {
                _fileInfo = new FileInfo(fileName);
                _volumeFileNames.Add(fileName);
                if (fileName.EndsWith("001"))
                {
                    int index = 2;
                    var baseName = fileName.Substring(0, fileName.Length - 3);
                    var volName = baseName + (index > 99 ? index.ToString() : 
                        index > 9 ? "0" + index : "00" + index);
                    while (File.Exists(volName))
                    {
                        _volumeFileNames.Add(volName);
                        index++;
                        volName = baseName + (index > 99 ? index.ToString() :
                        index > 9 ? "0" + index : "00" + index);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the ArchiveOpenCallback class.
        /// </summary>
        /// <param name="fileName">The archive file name.</param>
        public ArchiveOpenCallback(string fileName)
        {
            Init(fileName);
        }

        /// <summary>
        /// Initializes a new instance of the ArchiveOpenCallback class.
        /// </summary>
        /// <param name="fileName">The archive file name.</param>
        /// <param name="password">Password for the archive.</param>
        public ArchiveOpenCallback(string fileName, string password) : base(password)
        {
            Init(fileName);
        }

        #region IArchiveOpenCallback Members

        public void SetTotal(IntPtr files, IntPtr bytes) {}

        public void SetCompleted(IntPtr files, IntPtr bytes) {}

        #endregion

        #region IArchiveOpenVolumeCallback Members

        public int GetProperty(ItemPropId propId, ref PropVariant value)
        {
            switch (propId)
            {
                case ItemPropId.Name:
                    value.VarType = VarEnum.VT_BSTR;
                    value.Value = Marshal.StringToBSTR(_fileInfo.FullName);
                    break;
                case ItemPropId.IsDirectory:
                    value.VarType = VarEnum.VT_BOOL;
                    value.UInt64Value = (byte) (_fileInfo.Attributes & FileAttributes.Directory);
                    break;
                case ItemPropId.Size:
                    value.VarType = VarEnum.VT_UI8;
                    value.UInt64Value = (UInt64) _fileInfo.Length;
                    break;
                case ItemPropId.Attributes:
                    value.VarType = VarEnum.VT_UI4;
                    value.UInt32Value = (uint) _fileInfo.Attributes;
                    break;
                case ItemPropId.CreationTime:
                    value.VarType = VarEnum.VT_FILETIME;
                    value.Int64Value = _fileInfo.CreationTime.ToFileTime();
                    break;
                case ItemPropId.LastAccessTime:
                    value.VarType = VarEnum.VT_FILETIME;
                    value.Int64Value = _fileInfo.LastAccessTime.ToFileTime();
                    break;
                case ItemPropId.LastWriteTime:
                    value.VarType = VarEnum.VT_FILETIME;
                    value.Int64Value = _fileInfo.LastWriteTime.ToFileTime();
                    break;
            }
            return 0;
        }

        public int GetStream(string name, out IInStream inStream)
        {
            if (!File.Exists(name))
            {
                name = Path.Combine(Path.GetDirectoryName(_fileInfo.FullName), name);
                if (!File.Exists(name))
                {
                    inStream = null;
                    AddException(new FileNotFoundException("The volume \"" + name + "\" was not found. Extraction can be impossible."));
                    return 1;
                }
            }
            _volumeFileNames.Add(name);
            if (_wrappers.ContainsKey(name))
            {
                inStream = _wrappers[name];
            }
            else
            {
                try
                {
                    var wrapper = new InStreamWrapper(
                        new FileStream(name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), true);
                    _wrappers.Add(name, wrapper);
                    inStream = wrapper;                    
                }
                catch (Exception)
                {
                    AddException(new FileNotFoundException("Failed to open the volume \"" + name + "\". Extraction is impossible."));
                    inStream = null;
                    return 1;
                }
            }
            return 0;
        }

        #endregion

        #region ICryptoGetTextPassword Members

        /// <summary>
        /// Sets password for the archive
        /// </summary>
        /// <param name="password">Password for the archive</param>
        /// <returns>Zero if everything is OK</returns>
        public int CryptoGetTextPassword(out string password)
        {
            password = Password;
            return 0;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_wrappers != null)
            {
                foreach (InStreamWrapper wrap in _wrappers.Values)
                {
                    wrap.Dispose();
                }
                _wrappers = null;
            }
#if MONO
			libp7zInvokerRaw.FreeObject(Handle);	
#endif
            GC.SuppressFinalize(this);
        }

        #endregion        
    }
#endif
}
