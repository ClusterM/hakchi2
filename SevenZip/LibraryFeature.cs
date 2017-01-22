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

namespace SevenZip
{
    /// <summary>
    /// The set of features supported by the library.
    /// </summary>
    [Flags]
    [CLSCompliant(false)]
    public enum LibraryFeature : uint
    {
        /// <summary>
        /// Default feature.
        /// </summary>
        None = 0,
        /// <summary>
        /// The library can extract 7zip archives compressed with LZMA method.
        /// </summary>
        Extract7z = 0x1,
        /// <summary>
        /// The library can extract 7zip archives compressed with LZMA2 method.
        /// </summary>
        Extract7zLZMA2 = 0x2,
        /// <summary>
        /// The library can extract 7z archives compressed with all known methods.
        /// </summary>
        Extract7zAll = Extract7z|Extract7zLZMA2|0x4,
        /// <summary>
        /// The library can extract zip archives.
        /// </summary>
        ExtractZip = 0x8,
        /// <summary>
        /// The library can extract rar archives.
        /// </summary>
        ExtractRar = 0x10,
        /// <summary>
        /// The library can extract gzip archives.
        /// </summary>
        ExtractGzip = 0x20,
        /// <summary>
        /// The library can extract bzip2 archives.
        /// </summary>
        ExtractBzip2 = 0x40,
        /// <summary>
        /// The library can extract tar archives.
        /// </summary>
        ExtractTar = 0x80,
        /// <summary>
        /// The library can extract xz archives.
        /// </summary>
        ExtractXz = 0x100,
        /// <summary>
        /// The library can extract all types of archives supported.
        /// </summary>
        ExtractAll = Extract7zAll|ExtractZip|ExtractRar|ExtractGzip|ExtractBzip2|ExtractTar|ExtractXz,
        /// <summary>
        /// The library can compress data to 7zip archives with LZMA method.
        /// </summary>
        Compress7z = 0x200,
        /// <summary>
        /// The library can compress data to 7zip archives with LZMA2 method.
        /// </summary>
        Compress7zLZMA2 = 0x400,
        /// <summary>
        /// The library can compress data to 7zip archives with all methods known.
        /// </summary>
        Compress7zAll = Compress7z|Compress7zLZMA2|0x800,
        /// <summary>
        /// The library can compress data to tar archives.
        /// </summary>
        CompressTar = 0x1000,
        /// <summary>
        /// The library can compress data to gzip archives.
        /// </summary>
        CompressGzip = 0x2000,
        /// <summary>
        /// The library can compress data to bzip2 archives.
        /// </summary>
        CompressBzip2 = 0x4000,
        /// <summary>
        /// The library can compress data to xz archives.
        /// </summary>
        CompressXz = 0x8000,
        /// <summary>
        /// The library can compress data to zip archives.
        /// </summary>
        CompressZip = 0x10000,
        /// <summary>
        /// The library can compress data to all types of archives supported.
        /// </summary>
        CompressAll = Compress7zAll|CompressTar|CompressGzip|CompressBzip2|CompressXz|CompressZip,
        /// <summary>
        /// The library can modify archives.
        /// </summary>
        Modify = 0x20000
    }
}
