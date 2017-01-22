/*  This file is part of SevenZipSharp.

    SevenZipSharp is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    SevenZipSharp is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General public License for more details.

    You should have received a copy of the GNU Lesser General public License
    along with SevenZipSharp.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
#if !WINCE
using FILETIME=System.Runtime.InteropServices.ComTypes.FILETIME;
#elif WINCE
using FILETIME=OpenNETCF.Runtime.InteropServices.ComTypes.FILETIME;
#endif

namespace SevenZip
{
    #if UNMANAGED

    /// <summary>
    /// The structure to fix x64 and x32 variant size mismatch.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct PropArray
    {        
        uint _cElems;
        IntPtr _pElems;
    }

    /// <summary>
    /// COM VARIANT structure with special interface routines.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct PropVariant
    {
        [FieldOffset(0)] private ushort _vt;

        /// <summary>
        /// IntPtr variant value.
        /// </summary>
        [FieldOffset(8)] private IntPtr _value;

        /*/// <summary>
        /// Byte variant value.
        /// </summary>
        [FieldOffset(8)] 
        private byte _ByteValue;*/

        /// <summary>
        /// Signed int variant value.
        /// </summary>
        [FieldOffset(8)]
        private Int32 _int32Value;

        /// <summary>
        /// Unsigned int variant value.
        /// </summary>
        [FieldOffset(8)] private UInt32 _uInt32Value;

        /// <summary>
        /// Long variant value.
        /// </summary>
        [FieldOffset(8)] private Int64 _int64Value;

        /// <summary>
        /// Unsigned long variant value.
        /// </summary>
        [FieldOffset(8)] private UInt64 _uInt64Value;

        /// <summary>
        /// FILETIME variant value.
        /// </summary>
        [FieldOffset(8)] private FILETIME _fileTime;

        /// <summary>
        /// The PropArray instance to fix the variant size on x64 bit systems.
        /// </summary>
        [FieldOffset(8)]
        private PropArray _propArray;

        /// <summary>
        /// Gets or sets variant type.
        /// </summary>
        public VarEnum VarType
        {
            private get
            {
                return (VarEnum) _vt;
            }

            set
            {
                _vt = (ushort) value;
            }
        }

        /// <summary>
        /// Gets or sets the pointer value of the COM variant
        /// </summary>
        public IntPtr Value
        {
            private get
            {
                return _value;
            }

            set
            {
                _value = value;
            }
        }

        /*
        /// <summary>
        /// Gets or sets the byte value of the COM variant
        /// </summary>
        public byte ByteValue
        {
            get
            {
                return _ByteValue;
            }

            set
            {
                _ByteValue = value;
            }
        }
*/

        /// <summary>
        /// Gets or sets the UInt32 value of the COM variant.
        /// </summary>
        public UInt32 UInt32Value
        {
            private get
            {
                return _uInt32Value;
            }
            set
            {
                _uInt32Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the UInt32 value of the COM variant.
        /// </summary>
        public Int32 Int32Value
        {
            private get
            {
                return _int32Value;
            }
            set
            {
                _int32Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the Int64 value of the COM variant
        /// </summary>
        public Int64 Int64Value
        {
            private get
            {
                return _int64Value;
            }

            set
            {
                _int64Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the UInt64 value of the COM variant
        /// </summary>
        public UInt64 UInt64Value
        {
            private get
            {
                return _uInt64Value;
            }
            set
            {
                _uInt64Value = value;
            }
        }

        /*
        /// <summary>
        /// Gets or sets the FILETIME value of the COM variant
        /// </summary>
        public System.Runtime.InteropServices.ComTypes.FILETIME FileTime
        {
            get
            {
                return _fileTime;
            }

            set
            {
                _fileTime = value;
            }
        }
*/

        /*/// <summary>
        /// Gets or sets variant type (ushort).
        /// </summary>
        public ushort VarTypeNative
        {
            get
            {
                return _vt;
            }

            set
            {
                _vt = value;
            }
        }*/

        /*/// <summary>
        /// Clears variant
        /// </summary>
        public void Clear()
        {
            switch (VarType)
            {
                case VarEnum.VT_EMPTY:
                    break;
                case VarEnum.VT_NULL:
                case VarEnum.VT_I2:
                case VarEnum.VT_I4:
                case VarEnum.VT_R4:
                case VarEnum.VT_R8:
                case VarEnum.VT_CY:
                case VarEnum.VT_DATE:
                case VarEnum.VT_ERROR:
                case VarEnum.VT_BOOL:
                case VarEnum.VT_I1:
                case VarEnum.VT_UI1:
                case VarEnum.VT_UI2:
                case VarEnum.VT_UI4:
                case VarEnum.VT_I8:
                case VarEnum.VT_UI8:
                case VarEnum.VT_INT:
                case VarEnum.VT_UINT:
                case VarEnum.VT_HRESULT:
                case VarEnum.VT_FILETIME:
                    _vt = 0;
                    break;
                default:
                    if (NativeMethods.PropVariantClear(ref this) != (int)OperationResult.Ok)
                    {
                        throw new ArgumentException("PropVariantClear has failed for some reason.");
                    }
                    break;
            }
        }*/

        /// <summary>
        /// Gets the object for this PropVariant.
        /// </summary>
        /// <returns></returns>
        public object Object
        {
            get
            {
#if !WINCE
                var sp = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
                sp.Demand();
#endif
                switch (VarType)
                {
                    case VarEnum.VT_EMPTY:
                        return null;
                    case VarEnum.VT_FILETIME:
                        try
                        {
                            return DateTime.FromFileTime(Int64Value);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            return DateTime.MinValue;
                        }
                    default:
                        GCHandle propHandle = GCHandle.Alloc(this, GCHandleType.Pinned);
                        try
                        {
                            return Marshal.GetObjectForNativeVariant(propHandle.AddrOfPinnedObject());
                        }
#if WINCE
                        catch (NotSupportedException)
                        {
                            switch (VarType)
                            {
                                case VarEnum.VT_UI8:
                                    return UInt64Value;
                                case VarEnum.VT_UI4:
                                    return UInt32Value;
                                case VarEnum.VT_I8:
                                    return Int64Value;
                                case VarEnum.VT_I4:
                                    return Int32Value;
                                default:
                                    return 0;
                            }
                        }
#endif
                        finally
                        {
                            propHandle.Free();
                        }
                }
            }
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current PropVariant.
        /// </summary>
        /// <param name="obj">The System.Object to compare with the current PropVariant.</param>
        /// <returns>true if the specified System.Object is equal to the current PropVariant; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return (obj is PropVariant) ? Equals((PropVariant) obj) : false;
        }

        /// <summary>
        /// Determines whether the specified PropVariant is equal to the current PropVariant.
        /// </summary>
        /// <param name="afi">The PropVariant to compare with the current PropVariant.</param>
        /// <returns>true if the specified PropVariant is equal to the current PropVariant; otherwise, false.</returns>
        private bool Equals(PropVariant afi)
        {
            if (afi.VarType != VarType)
            {
                return false;
            }
            if (VarType != VarEnum.VT_BSTR)
            {
                return afi.Int64Value == Int64Value;
            }
            return afi.Value == Value;
        }

        /// <summary>
        ///  Serves as a hash function for a particular type.
        /// </summary>
        /// <returns> A hash code for the current PropVariant.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Returns a System.String that represents the current PropVariant.
        /// </summary>
        /// <returns>A System.String that represents the current PropVariant.</returns>
        public override string ToString()
        {
            return "[" + Value + "] " + Int64Value.ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Determines whether the specified PropVariant instances are considered equal.
        /// </summary>
        /// <param name="afi1">The first PropVariant to compare.</param>
        /// <param name="afi2">The second PropVariant to compare.</param>
        /// <returns>true if the specified PropVariant instances are considered equal; otherwise, false.</returns>
        public static bool operator ==(PropVariant afi1, PropVariant afi2)
        {
            return afi1.Equals(afi2);
        }

        /// <summary>
        /// Determines whether the specified PropVariant instances are not considered equal.
        /// </summary>
        /// <param name="afi1">The first PropVariant to compare.</param>
        /// <param name="afi2">The second PropVariant to compare.</param>
        /// <returns>true if the specified PropVariant instances are not considered equal; otherwise, false.</returns>
        public static bool operator !=(PropVariant afi1, PropVariant afi2)
        {
            return !afi1.Equals(afi2);
        }
    }

    /// <summary>
    /// Stores file extraction modes.
    /// </summary>
    internal enum AskMode
    {
        /// <summary>
        /// Extraction mode
        /// </summary>
        Extract = 0,
        /// <summary>
        /// Test mode
        /// </summary>
        Test,
        /// <summary>
        /// Skip mode
        /// </summary>
        Skip
    }

    /// <summary>
    /// Stores operation result values
    /// </summary>
    public enum OperationResult
    {
        /// <summary>
        /// Success
        /// </summary>
        Ok = 0,
        /// <summary>
        /// Method is unsupported
        /// </summary>
        UnsupportedMethod,
        /// <summary>
        /// Data error has occured
        /// </summary>
        DataError,
        /// <summary>
        /// CrcError has occured
        /// </summary>
        CrcError
    }

    /// <summary>
    /// Codes of item properities
    /// </summary>
    internal enum ItemPropId : uint
    {
        /// <summary>
        /// No property
        /// </summary>
        NoProperty = 0,
        /// <summary>
        /// Handler item index
        /// </summary>
        HandlerItemIndex = 2,
        /// <summary>
        /// Item path
        /// </summary>
        Path,
        /// <summary>
        /// Item name
        /// </summary>
        Name,
        /// <summary>
        /// Item extension
        /// </summary>
        Extension,
        /// <summary>
        /// true if the item is a folder; otherwise, false
        /// </summary>
        IsDirectory,
        /// <summary>
        /// Item size
        /// </summary>
        Size,
        /// <summary>
        /// Item packed sise; usually absent
        /// </summary>
        PackedSize,
        /// <summary>
        /// Item attributes; usually absent
        /// </summary>
        Attributes,
        /// <summary>
        /// Item creation time; usually absent
        /// </summary>
        CreationTime,
        /// <summary>
        /// Item last access time; usually absent
        /// </summary>
        LastAccessTime,
        /// <summary>
        /// Item last write time
        /// </summary>
        LastWriteTime,
        /// <summary>
        /// true if the item is solid; otherwise, false
        /// </summary>
        Solid,
        /// <summary>
        /// true if the item is commented; otherwise, false
        /// </summary>
        Commented,
        /// <summary>
        /// true if the item is encrypted; otherwise, false
        /// </summary>
        Encrypted,
        /// <summary>
        /// (?)
        /// </summary>
        SplitBefore,
        /// <summary>
        /// (?)
        /// </summary>
        SplitAfter,
        /// <summary>
        /// Dictionary size(?)
        /// </summary>
        DictionarySize,
        /// <summary>
        /// Item CRC checksum
        /// </summary>
        Crc,
        /// <summary>
        /// Item type(?)
        /// </summary>
        Type,
        /// <summary>
        /// (?)
        /// </summary>
        IsAnti,
        /// <summary>
        /// Compression method
        /// </summary>
        Method,
        /// <summary>
        /// (?); usually absent
        /// </summary>
        HostOS,
        /// <summary>
        /// Item file system; usually absent
        /// </summary>
        FileSystem,
        /// <summary>
        /// Item user(?); usually absent
        /// </summary>
        User,
        /// <summary>
        /// Item group(?); usually absent
        /// </summary>
        Group,
        /// <summary>
        /// Bloack size(?)
        /// </summary>
        Block,
        /// <summary>
        /// Item comment; usually absent
        /// </summary>
        Comment,
        /// <summary>
        /// Item position
        /// </summary>
        Position,
        /// <summary>
        /// Item prefix(?)
        /// </summary>
        Prefix,
        /// <summary>
        /// Number of subdirectories
        /// </summary>
        NumSubDirs,
        /// <summary>
        /// Numbers of subfiles
        /// </summary>
        NumSubFiles,
        /// <summary>
        /// The archive legacy unpacker version
        /// </summary>
        UnpackVersion,
        /// <summary>
        /// Volume(?)
        /// </summary>
        Volume,
        /// <summary>
        /// Is a volume
        /// </summary>
        IsVolume,
        /// <summary>
        /// Offset value(?)
        /// </summary>
        Offset,
        /// <summary>
        /// Links(?)
        /// </summary>
        Links,
        /// <summary>
        /// Number of blocks
        /// </summary>
        NumBlocks,
        /// <summary>
        /// Number of volumes(?)
        /// </summary>
        NumVolumes,
        /// <summary>
        /// Time type(?)
        /// </summary>
        TimeType,
        /// <summary>
        /// 64-bit(?)
        /// </summary>
        Bit64,
        /// <summary>
        /// BigEndian
        /// </summary>
        BigEndian,
        /// <summary>
        /// Cpu(?)
        /// </summary>
        Cpu,
        /// <summary>
        /// Physical archive size
        /// </summary>
        PhysicalSize,
        /// <summary>
        /// Headers size
        /// </summary>
        HeadersSize,
        /// <summary>
        /// Archive checksum
        /// </summary>
        Checksum,
        /// <summary>
        /// (?)
        /// </summary>
        TotalSize = 0x1100,
        /// <summary>
        /// (?)
        /// </summary>
        FreeSpace,
        /// <summary>
        /// Cluster size(?)
        /// </summary>
        ClusterSize,
        /// <summary>
        /// Volume name(?)
        /// </summary>
        VolumeName,
        /// <summary>
        /// Local item name(?); usually absent
        /// </summary>
        LocalName = 0x1200,
        /// <summary>
        /// (?)
        /// </summary>
        Provider,
        /// <summary>
        /// User defined property; usually absent
        /// </summary>
        UserDefined = 0x10000
    }

    /*/// <summary>
    /// Codes of archive properties or modes.
    /// </summary>
    internal enum ArchivePropId : uint
    {
        Name = 0,
        ClassID,
        Extension,
        AddExtension,
        Update,
        KeepName,
        StartSignature,
        FinishSignature,
        Associate
    }*/

    /// <summary>
    /// PropId string names dictionary wrapper.
    /// </summary>
    internal static class PropIdToName
    {
        /// <summary>
        /// PropId string names
        /// </summary>
        public static readonly Dictionary<ItemPropId, string> PropIdNames =
        #region Initialization
            new Dictionary<ItemPropId, string>(46)
            {
                {ItemPropId.Path, "Path"},
                {ItemPropId.Name, "Name"},
                {ItemPropId.IsDirectory, "Folder"},
                {ItemPropId.Size, "Size"},
                {ItemPropId.PackedSize, "Packed Size"},
                {ItemPropId.Attributes, "Attributes"},
                {ItemPropId.CreationTime, "Created"},
                {ItemPropId.LastAccessTime, "Accessed"},
                {ItemPropId.LastWriteTime, "Modified"},
                {ItemPropId.Solid, "Solid"},
                {ItemPropId.Commented, "Commented"},
                {ItemPropId.Encrypted, "Encrypted"},
                {ItemPropId.SplitBefore, "Split Before"},
                {ItemPropId.SplitAfter, "Split After"},
                {
                    ItemPropId.DictionarySize,
                    "Dictionary Size"
                    },
                {ItemPropId.Crc, "CRC"},
                {ItemPropId.Type, "Type"},
                {ItemPropId.IsAnti, "Anti"},
                {ItemPropId.Method, "Method"},
                {ItemPropId.HostOS, "Host OS"},
                {ItemPropId.FileSystem, "File System"},
                {ItemPropId.User, "User"},
                {ItemPropId.Group, "Group"},
                {ItemPropId.Block, "Block"},
                {ItemPropId.Comment, "Comment"},
                {ItemPropId.Position, "Position"},
                {ItemPropId.Prefix, "Prefix"},
                {
                    ItemPropId.NumSubDirs,
                    "Number of subdirectories"
                    },
                {
                    ItemPropId.NumSubFiles,
                    "Number of subfiles"
                    },
                {
                    ItemPropId.UnpackVersion,
                    "Unpacker version"
                    },
                {ItemPropId.Volume, "Volume"},
                {ItemPropId.IsVolume, "IsVolume"},
                {ItemPropId.Offset, "Offset"},
                {ItemPropId.Links, "Links"},
                {
                    ItemPropId.NumBlocks,
                    "Number of blocks"
                    },
                {
                    ItemPropId.NumVolumes,
                    "Number of volumes"
                    },
                {ItemPropId.TimeType, "Time type"},
                {ItemPropId.Bit64, "64-bit"},
                {ItemPropId.BigEndian, "Big endian"},
                {ItemPropId.Cpu, "CPU"},
                {
                    ItemPropId.PhysicalSize,
                    "Physical Size"
                    },
                {ItemPropId.HeadersSize, "Headers Size"},
                {ItemPropId.Checksum, "Checksum"},
                {ItemPropId.FreeSpace, "Free Space"},
                {ItemPropId.ClusterSize, "Cluster Size"}
            };
        #endregion
    }

    /// <summary>
    /// 7-zip IArchiveOpenCallback imported interface to handle the opening of an archive.
    /// </summary>
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000600100000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IArchiveOpenCallback
    {
        // ref ulong replaced with IntPtr because handlers often pass null value
        // read actual value with Marshal.ReadInt64
        /// <summary>
        /// Sets total data size
        /// </summary>
        /// <param name="files">Files pointer</param>
        /// <param name="bytes">Total size in bytes</param>
        void SetTotal(
            IntPtr files,
            IntPtr bytes);

        /// <summary>
        /// Sets completed size
        /// </summary>
        /// <param name="files">Files pointer</param>
        /// <param name="bytes">Completed size in bytes</param>
        void SetCompleted(
            IntPtr files,
            IntPtr bytes);
    }

    /// <summary>
    /// 7-zip ICryptoGetTextPassword imported interface to get the archive password.
    /// </summary>
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000500100000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICryptoGetTextPassword
    {
        /// <summary>
        /// Gets password for the archive
        /// </summary>
        /// <param name="password">Password for the archive</param>
        /// <returns>Zero if everything is OK</returns>
        [PreserveSig]
        int CryptoGetTextPassword(
            [MarshalAs(UnmanagedType.BStr)] out string password);
    }

    /// <summary>
    /// 7-zip ICryptoGetTextPassword2 imported interface for setting the archive password.
    /// </summary>
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000500110000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICryptoGetTextPassword2
    {
        /// <summary>
        /// Sets password for the archive
        /// </summary>
        /// <param name="passwordIsDefined">Specifies whether archive has a password or not (0 if not)</param>
        /// <param name="password">Password for the archive</param>
        /// <returns>Zero if everything is OK</returns>
        [PreserveSig]
        int CryptoGetTextPassword2(
            ref int passwordIsDefined,
            [MarshalAs(UnmanagedType.BStr)] out string password);
    }

    /// <summary>
    /// 7-zip IArchiveExtractCallback imported interface.
    /// </summary>
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000600200000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IArchiveExtractCallback
    {
        /// <summary>
        /// Gives the size of the unpacked archive files
        /// </summary>
        /// <param name="total">Size of the unpacked archive files (in bytes)</param>
        void SetTotal(ulong total);

        /// <summary>
        /// SetCompleted 7-zip function
        /// </summary>
        /// <param name="completeValue"></param>
        void SetCompleted([In] ref ulong completeValue);

        /// <summary>
        /// Gets the stream for file extraction
        /// </summary>
        /// <param name="index">File index in the archive file table</param>
        /// <param name="outStream">Pointer to the stream</param>
        /// <param name="askExtractMode">Extraction mode</param>
        /// <returns>S_OK - OK, S_FALSE - skip this file</returns>
        [PreserveSig]
        int GetStream(
            uint index,
            [Out, MarshalAs(UnmanagedType.Interface)] out ISequentialOutStream outStream,
            AskMode askExtractMode);

        /// <summary>
        /// PrepareOperation 7-zip function
        /// </summary>
        /// <param name="askExtractMode">Ask mode</param>
        void PrepareOperation(AskMode askExtractMode);

        /// <summary>
        /// Sets the operaton result
        /// </summary>
        /// <param name="operationResult">The operation result</param>
        void SetOperationResult(OperationResult operationResult);
    }

    /// <summary>
    /// 7-zip IArchiveUpdateCallback imported interface.
    /// </summary>
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000600800000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IArchiveUpdateCallback
    {
        /// <summary>
        /// Gives the size of the unpacked archive files.
        /// </summary>
        /// <param name="total">Size of the unpacked archive files (in bytes)</param>
        void SetTotal(ulong total);

        /// <summary>
        /// SetCompleted 7-zip internal function.
        /// </summary>
        /// <param name="completeValue"></param>
        void SetCompleted([In] ref ulong completeValue);

        /// <summary>
        /// Gets archive update mode.
        /// </summary>
        /// <param name="index">File index</param>
        /// <param name="newData">1 if new, 0 if not</param>
        /// <param name="newProperties">1 if new, 0 if not</param>
        /// <param name="indexInArchive">-1 if doesn't matter</param>
        /// <returns></returns>
        [PreserveSig]
        int GetUpdateItemInfo(
            uint index, ref int newData,
            ref int newProperties, ref uint indexInArchive);

        /// <summary>
        /// Gets the archive item property data.
        /// </summary>
        /// <param name="index">Item index</param>
        /// <param name="propId">Property identificator</param>
        /// <param name="value">Property value</param>
        /// <returns>Zero if Ok</returns>
        [PreserveSig]
        int GetProperty(uint index, ItemPropId propId, ref PropVariant value);

        /// <summary>
        /// Gets the stream for reading.
        /// </summary>
        /// <param name="index">The item index.</param>
        /// <param name="inStream">The ISequentialInStream pointer for reading.</param>
        /// <returns>Zero if Ok</returns>
        [PreserveSig]
        int GetStream(
            uint index,
            [Out, MarshalAs(UnmanagedType.Interface)] out ISequentialInStream inStream);

        /// <summary>
        /// Sets the result for currently performed operation.
        /// </summary>
        /// <param name="operationResult">The result value.</param>
        void SetOperationResult(OperationResult operationResult);

        /// <summary>
        /// EnumProperties 7-zip internal function.
        /// </summary>
        /// <param name="enumerator">The enumerator pointer.</param>
        /// <returns></returns>
        long EnumProperties(IntPtr enumerator);
    }

    /// <summary>
    /// 7-zip IArchiveOpenVolumeCallback imported interface to handle archive volumes.
    /// </summary>
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000600300000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IArchiveOpenVolumeCallback
    {
        /// <summary>
        /// Gets the archive property data.
        /// </summary>
        /// <param name="propId">The property identificator.</param>
        /// <param name="value">The property value.</param>
        [PreserveSig]
        int GetProperty(
            ItemPropId propId, ref PropVariant value);

        /// <summary>
        /// Gets the stream for reading the volume.
        /// </summary>
        /// <param name="name">The volume file name.</param>
        /// <param name="inStream">The IInStream pointer for reading.</param>
        /// <returns>Zero if Ok</returns>
        [PreserveSig]
        int GetStream(
            [MarshalAs(UnmanagedType.LPWStr)] string name,
            [Out, MarshalAs(UnmanagedType.Interface)] out IInStream inStream);
    }    

    /// <summary>
    /// 7-zip ISequentialInStream imported interface
    /// </summary>
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000300010000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISequentialInStream
    {
        /// <summary>
        /// Writes data to 7-zip packer
        /// </summary>
        /// <param name="data">Array of bytes available for writing</param>
        /// <param name="size">Array size</param>
        /// <returns>S_OK if success</returns>
        /// <remarks>If (size > 0) and there are bytes in stream, 
        /// this function must read at least 1 byte.
        /// This function is allowed to read less than "size" bytes.
        /// You must call Read function in loop, if you need exact amount of data.
        /// </remarks>
        int Read(
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] data,
            uint size);
    }

    /// <summary>
    /// 7-zip ISequentialOutStream imported interface
    /// </summary>
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000300020000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISequentialOutStream
    {
        /// <summary>
        /// Writes data to unpacked file stream
        /// </summary>
        /// <param name="data">Array of bytes available for reading</param>
        /// <param name="size">Array size</param>
        /// <param name="processedSize">Processed data size</param>
        /// <returns>S_OK if success</returns>
        /// <remarks>If size != 0, return value is S_OK and (*processedSize == 0),
        ///  then there are no more bytes in stream.
        /// If (size > 0) and there are bytes in stream, 
        /// this function must read at least 1 byte.
        /// This function is allowed to rwrite less than "size" bytes.
        /// You must call Write function in loop, if you need exact amount of data.
        /// </remarks>
        [PreserveSig]
        int Write(
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] data,
            uint size, IntPtr processedSize);
    }

    /// <summary>
    /// 7-zip IInStream imported interface
    /// </summary>
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000300030000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IInStream
    {
        /// <summary>
        /// Read routine
        /// </summary>
        /// <param name="data">Array of bytes to set</param>
        /// <param name="size">Array size</param>
        /// <returns>Zero if Ok</returns>
        int Read(
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] data,
            uint size);

        /// <summary>
        /// Seek routine
        /// </summary>
        /// <param name="offset">Offset value</param>
        /// <param name="seekOrigin">Seek origin value</param>
        /// <param name="newPosition">New position pointer</param>
        void Seek(
            long offset, SeekOrigin seekOrigin, IntPtr newPosition);
    }

    /// <summary>
    /// 7-zip IOutStream imported interface
    /// </summary>
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000300040000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IOutStream
    {
        /// <summary>
        /// Write routine
        /// </summary>
        /// <param name="data">Array of bytes to get</param>
        /// <param name="size">Array size</param>
        /// <param name="processedSize">Processed size</param>
        /// <returns>Zero if Ok</returns>
        [PreserveSig]
        int Write(
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] data,
            uint size,
            IntPtr processedSize);

        /// <summary>
        /// Seek routine
        /// </summary>
        /// <param name="offset">Offset value</param>
        /// <param name="seekOrigin">Seek origin value</param>
        /// <param name="newPosition">New position pointer</param>       
        void Seek(
            long offset, SeekOrigin seekOrigin, IntPtr newPosition);

        /// <summary>
        /// Set size routine
        /// </summary>
        /// <param name="newSize">New size value</param>
        /// <returns>Zero if Ok</returns>
        [PreserveSig]
        int SetSize(long newSize);
    }

    /// <summary>
    /// 7-zip essential in archive interface
    /// </summary>
    [ComImport]  
	[Guid("23170F69-40C1-278A-0000-000600600000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]	
    internal interface IInArchive
    {
        /// <summary>
        /// Opens archive for reading.
        /// </summary>
        /// <param name="stream">Archive file stream</param>
        /// <param name="maxCheckStartPosition">Maximum start position for checking</param>
        /// <param name="openArchiveCallback">Callback for opening archive</param>
        /// <returns></returns>
        [PreserveSig]
        int Open(
            IInStream stream,
            [In] ref ulong maxCheckStartPosition,
            [MarshalAs(UnmanagedType.Interface)] IArchiveOpenCallback openArchiveCallback);

        /// <summary>
        /// Closes the archive.
        /// </summary>
        void Close();

        /// <summary>
        /// Gets the number of files in the archive file table  .          
        /// </summary>
        /// <returns>The number of files in the archive</returns>
        uint GetNumberOfItems();

        /// <summary>
        /// Retrieves specific property data.
        /// </summary>
        /// <param name="index">File index in the archive file table</param>
        /// <param name="propId">Property code</param>
        /// <param name="value">Property variant value</param>
        void GetProperty(
            uint index,
            ItemPropId propId,
            ref PropVariant value); // PropVariant

        /// <summary>
        /// Extracts files from the opened archive.
        /// </summary>
        /// <param name="indexes">indexes of files to be extracted (must be sorted)</param>
        /// <param name="numItems">0xFFFFFFFF means all files</param>
        /// <param name="testMode">testMode != 0 means "test files operation"</param>
        /// <param name="extractCallback">IArchiveExtractCallback for operations handling</param>
        /// <returns>0 if success</returns>
        [PreserveSig]
        int Extract(
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] uint[] indexes,
            uint numItems,
            int testMode,
            [MarshalAs(UnmanagedType.Interface)] IArchiveExtractCallback extractCallback);

        /// <summary>
        /// Gets archive property data
        /// </summary>
        /// <param name="propId">Archive property identificator</param>
        /// <param name="value">Archive property value</param>
        void GetArchiveProperty(
            ItemPropId propId, // PROPID
            ref PropVariant value); // PropVariant

        /// <summary>
        /// Gets the number of properties
        /// </summary>
        /// <returns>The number of properties</returns>
        uint GetNumberOfProperties();

        /// <summary>
        /// Gets property information
        /// </summary>
        /// <param name="index">Item index</param>
        /// <param name="name">Name</param>
        /// <param name="propId">Property identificator</param>
        /// <param name="varType">Variant type</param>
        void GetPropertyInfo(
            uint index,
            [MarshalAs(UnmanagedType.BStr)] out string name,
            out ItemPropId propId, // PROPID
            out ushort varType); //VARTYPE

        /// <summary>
        /// Gets the number of archive properties
        /// </summary>
        /// <returns>The number of archive properties</returns>
        uint GetNumberOfArchiveProperties();

        /// <summary>
        /// Gets the archive property information
        /// </summary>
        /// <param name="index">Item index</param>
        /// <param name="name">Name</param>
        /// <param name="propId">Property identificator</param>
        /// <param name="varType">Variant type</param>
        void GetArchivePropertyInfo(
            uint index,
            [MarshalAs(UnmanagedType.BStr)] out string name,
            out ItemPropId propId, // PROPID
            out ushort varType); //VARTYPE
    }

    /// <summary>
    /// 7-zip essential out archive interface
    /// </summary>
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000600A00000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IOutArchive
    {
        /// <summary>
        /// Updates archive items
        /// </summary>
        /// <param name="outStream">The ISequentialOutStream pointer for writing the archive data</param>
        /// <param name="numItems">Number of archive items</param>
        /// <param name="updateCallback">The IArchiveUpdateCallback pointer</param>
        /// <returns>Zero if Ok</returns>
        [PreserveSig]
        int UpdateItems(
            [MarshalAs(UnmanagedType.Interface)] ISequentialOutStream outStream,
            uint numItems,
            [MarshalAs(UnmanagedType.Interface)] IArchiveUpdateCallback updateCallback);

        /// <summary>
        /// Gets file time type(?)
        /// </summary>
        /// <param name="type">Type pointer</param>
        void GetFileTimeType(IntPtr type);
    }

    /// <summary>
    /// 7-zip ISetProperties interface for setting various archive properties
    /// </summary>
    [ComImport]
    [Guid("23170F69-40C1-278A-0000-000600030000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISetProperties
    {
        /// <summary>
        /// Sets the archive properties
        /// </summary>
        /// <param name="names">The names of the properties</param>
        /// <param name="values">The values of the properties</param>
        /// <param name="numProperties">The properties count</param>
        /// <returns></returns>        
        int SetProperties(IntPtr names, IntPtr values, int numProperties);
    }
#endif
}