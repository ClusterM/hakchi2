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
#define COMPRESS

namespace SevenZip
{
    using System.Collections.Generic;
    using System.IO;
#if DOTNET20
    using System.Threading;
#else
    using System.Windows.Threading;
#endif

    partial class SevenZipCompressor
    {
        #region Delegates
        private delegate void CompressFiles1Delegate(string archiveName, string[] fileFullNames);
        private delegate void CompressFiles2Delegate(Stream archiveStream, string[] fileFullNames);
        private delegate void CompressFiles3Delegate(string archiveName, int commonRootLength, string[] fileFullNames);
        private delegate void CompressFiles4Delegate(Stream archiveStream, int commonRootLength, string[] fileFullNames);

        private delegate void CompressFilesEncrypted1Delegate(string archiveName, string password, string[] fileFullNames);
        private delegate void CompressFilesEncrypted2Delegate(Stream archiveStream, string password, string[] fileFullNames);
        private delegate void CompressFilesEncrypted3Delegate(string archiveName, int commonRootLength, string password, string[] fileFullNames);
        private delegate void CompressFilesEncrypted4Delegate(Stream archiveStream, int commonRootLength, string password, string[] fileFullNames);

        private delegate void CompressDirectory1Delegate(string directory, string archiveName);
        private delegate void CompressDirectory2Delegate(string directory, Stream archiveStream);
        private delegate void CompressDirectory3Delegate(string directory, string archiveName, string password);
        private delegate void CompressDirectory4Delegate(string directory, Stream archiveStream, string password);
        private delegate void CompressDirectory5Delegate(string directory, string archiveName,
            string password, string searchPattern, bool recursion);
        private delegate void CompressDirectory6Delegate(string directory, Stream archiveStream,
            string password, string searchPattern, bool recursion);

        private delegate void CompressStream1Delegate(Stream inStream, Stream outStream);
        private delegate void CompressStream2Delegate(Stream inStream, Stream outStream, string password);

        private delegate void ModifyArchive1Delegate(string archiveName, Dictionary<int, string> newFileNames);
        private delegate void ModifyArchive2Delegate(string archiveName, Dictionary<int, string> newFileNames,
             string password);
        #endregion

        #region CompressFiles overloads
#if !DOTNET20
        /// <summary>
        /// Packs files into the archive asynchronously.
        /// </summary>
        /// <param name="fileFullNames">Array of file names to pack.</param>
        /// <param name="archiveName">The archive file name.</param>
        /// <param name="eventPriority">The priority of events, relative to the other pending operations in the System.Windows.Threading.Dispatcher event queue, the specified method is invoked.</param>
#else
        /// <summary>
        /// Packs files into the archive asynchronously.
        /// </summary>
        /// <param name="fileFullNames">Array of file names to pack.</param>
        /// <param name="archiveName">The archive file name.</param>
#endif
        public void BeginCompressFiles(
            string archiveName
#if !DOTNET20
            , DispatcherPriority eventPriority
#endif                        
            , params string[] fileFullNames
        )
        {
            SaveContext(
#if !DOTNET20
                eventPriority
#endif
            );
            (new CompressFiles1Delegate(CompressFiles)).BeginInvoke(archiveName, fileFullNames,
                AsyncCallbackImplementation, this);
        }

#if !DOTNET20
        /// <summary>
        /// Packs files into the archive asynchronously.
        /// </summary>
        /// <param name="fileFullNames">Array of file names to pack.</param>
        /// <param name="archiveStream">The archive output stream. 
        /// Use CompressFiles(string archiveName ... ) overloads for archiving to disk.</param>
        /// <param name="eventPriority">The priority of events, relative to the other pending operations in the System.Windows.Threading.Dispatcher event queue, the specified method is invoked.</param>
#else
        /// <summary>
        /// Packs files into the archive asynchronously.
        /// </summary>
        /// <param name="fileFullNames">Array of file names to pack.</param>
        /// <param name="archiveStream">The archive output stream. 
        /// Use CompressFiles(string archiveName ... ) overloads for archiving to disk.</param>
#endif
        public void BeginCompressFiles(
            Stream archiveStream
#if !DOTNET20
            , DispatcherPriority eventPriority
#endif            
            , params string[] fileFullNames          
        )
        {
            SaveContext(
#if !DOTNET20
                eventPriority
#endif
            );
            (new CompressFiles2Delegate(CompressFiles)).BeginInvoke(archiveStream, fileFullNames,
                AsyncCallbackImplementation, this);
        }

#if !DOTNET20
        /// <summary>
        /// Packs files into the archive asynchronously.
        /// </summary>
        /// <param name="fileFullNames">Array of file names to pack.</param>
        /// <param name="commonRootLength">The length of the common root of the file names.</param>
        /// <param name="archiveName">The archive file name.</param>
        /// <param name="eventPriority">The priority of events, relative to the other pending operations in the System.Windows.Threading.Dispatcher event queue, the specified method is invoked.</param>
#else
        /// <summary>
        /// Packs files into the archive asynchronously.
        /// </summary>
        /// <param name="fileFullNames">Array of file names to pack.</param>
        /// <param name="commonRootLength">The length of the common root of the file names.</param>
        /// <param name="archiveName">The archive file name.</param>
#endif
        public void BeginCompressFiles(
            string archiveName, int commonRootLength
#if !DOTNET20
            , DispatcherPriority eventPriority
#endif             
            , params string[] fileFullNames           
        )
        {
            SaveContext(
#if !DOTNET20
                eventPriority
#endif
            );
            (new CompressFiles3Delegate(CompressFiles)).BeginInvoke(archiveName, commonRootLength, fileFullNames,
                AsyncCallbackImplementation, this);
        }

#if !DOTNET20
        /// <summary>
        /// Packs files into the archive asynchronously.
        /// </summary>
        /// <param name="fileFullNames">Array of file names to pack.</param>
        /// <param name="commonRootLength">The length of the common root of the file names.</param>
        /// <param name="archiveStream">The archive output stream.
        /// Use CompressFiles(string archiveName, ... ) overloads for archiving to disk.</param>
        /// <param name="eventPriority">The priority of events, relative to the other pending operations in the System.Windows.Threading.Dispatcher event queue, the specified method is invoked.</param>
#else
        /// <summary>
        /// Packs files into the archive asynchronously.
        /// </summary>
        /// <param name="fileFullNames">Array of file names to pack.</param>
        /// <param name="commonRootLength">The length of the common root of the file names.</param>
        /// <param name="archiveStream">The archive output stream.
        /// Use CompressFiles(string archiveName, ... ) overloads for archiving to disk.</param>
#endif
        public void BeginCompressFiles(
            Stream archiveStream, int commonRootLength
#if !DOTNET20
            , DispatcherPriority eventPriority
#endif              
            , params string[] fileFullNames          
        )
        {
            SaveContext(
#if !DOTNET20
                eventPriority
#endif
            );
            (new CompressFiles4Delegate(CompressFiles)).BeginInvoke(archiveStream, commonRootLength, fileFullNames,
                AsyncCallbackImplementation, this);
        }

#if !DOTNET20
        /// <summary>
        /// Packs files into the archive asynchronously.
        /// </summary>
        /// <param name="fileFullNames">Array of file names to pack.</param>
        /// <param name="archiveName">The archive file name</param>
        /// <param name="password">The archive password.</param>
        /// <param name="eventPriority">The priority of events, relative to the other pending operations in the System.Windows.Threading.Dispatcher event queue, the specified method is invoked.</param>
#else
        /// <summary>
        /// Packs files into the archive asynchronously.
        /// </summary>
        /// <param name="fileFullNames">Array of file names to pack.</param>
        /// <param name="archiveName">The archive file name</param>
        /// <param name="password">The archive password.</param>
#endif
        public void BeginCompressFilesEncrypted(
            string archiveName, string password
#if !DOTNET20
            , DispatcherPriority eventPriority
#endif            
            , params string[] fileFullNames        
        )
        {
            SaveContext(
#if !DOTNET20
                eventPriority
#endif
            );
            (new CompressFilesEncrypted1Delegate(CompressFilesEncrypted)).BeginInvoke(archiveName, password, fileFullNames,
                AsyncCallbackImplementation, this);
        }

#if !DOTNET20
        /// <summary>
        /// Packs files into the archive asynchronously.
        /// </summary>
        /// <param name="fileFullNames">Array of file names to pack.</param>
        /// <param name="archiveStream">The archive output stream.
        /// Use CompressFiles( ... string archiveName ... ) overloads for archiving to disk.</param>
        /// <param name="password">The archive password.</param>
        /// <param name="eventPriority">The priority of events, relative to the other pending operations in the System.Windows.Threading.Dispatcher event queue, the specified method is invoked.</param>
#else
        /// <summary>
        /// Packs files into the archive asynchronously.
        /// </summary>
        /// <param name="fileFullNames">Array of file names to pack.</param>
        /// <param name="archiveStream">The archive output stream.
        /// Use CompressFiles( ... string archiveName ... ) overloads for archiving to disk.</param>
        /// <param name="password">The archive password.</param>
#endif
        public void BeginCompressFilesEncrypted(
            Stream archiveStream, string password
#if !DOTNET20
            , DispatcherPriority eventPriority
#endif              
            , params string[] fileFullNames          
        )
        {
            SaveContext(
#if !DOTNET20
                eventPriority
#endif
            );
            (new CompressFilesEncrypted2Delegate(CompressFilesEncrypted)).BeginInvoke(archiveStream, password, fileFullNames,
                AsyncCallbackImplementation, this);
        }

#if !DOTNET20
        /// <summary>
        /// Packs files into the archive asynchronously.
        /// </summary>
        /// <param name="fileFullNames">Array of file names to pack.</param>
        /// <param name="archiveName">The archive file name</param>
        /// <param name="password">The archive password.</param>
        /// <param name="commonRootLength">The length of the common root of the file names.</param>
        /// <param name="eventPriority">The priority of events, relative to the other pending operations in the System.Windows.Threading.Dispatcher event queue, the specified method is invoked.</param>
#else
        /// <summary>
        /// Packs files into the archive asynchronously.
        /// </summary>
        /// <param name="fileFullNames">Array of file names to pack.</param>
        /// <param name="archiveName">The archive file name</param>
        /// <param name="password">The archive password.</param>
        /// <param name="commonRootLength">The length of the common root of the file names.</param>
#endif
        public void BeginCompressFilesEncrypted(
            string archiveName, int commonRootLength, string password
#if !DOTNET20
            , DispatcherPriority eventPriority
#endif            
            , params string[] fileFullNames         
        )
        {
            SaveContext(
#if !DOTNET20
                eventPriority
#endif
            );
            (new CompressFilesEncrypted3Delegate(CompressFilesEncrypted)).BeginInvoke(archiveName, commonRootLength, password,
                fileFullNames, AsyncCallbackImplementation, this);
        }

#if !DOTNET20
        /// <summary>
        /// Packs files into the archive asynchronously.
        /// </summary>
        /// <param name="fileFullNames">Array of file names to pack.</param>
        /// <param name="archiveStream">The archive output stream.
        /// Use CompressFiles( ... string archiveName ... ) overloads for archiving to disk.</param>
        /// <param name="password">The archive password.</param>
        /// <param name="commonRootLength">The length of the common root of the file names.</param>
        /// <param name="eventPriority">The priority of events, relative to the other pending operations in the System.Windows.Threading.Dispatcher event queue, the specified method is invoked.</param>
#else
        /// <summary>
        /// Packs files into the archive asynchronously.
        /// </summary>
        /// <param name="fileFullNames">Array of file names to pack.</param>
        /// <param name="archiveStream">The archive output stream.
        /// Use CompressFiles( ... string archiveName ... ) overloads for archiving to disk.</param>
        /// <param name="password">The archive password.</param>
        /// <param name="commonRootLength">The length of the common root of the file names.</param>
#endif
        public void BeginCompressFilesEncrypted(
            Stream archiveStream, int commonRootLength, string password
#if !DOTNET20
, DispatcherPriority eventPriority
#endif             
            , params string[] fileFullNames           
        )
        {
            SaveContext(
#if !DOTNET20
                eventPriority
#endif
            );
            (new CompressFilesEncrypted4Delegate(CompressFilesEncrypted)).BeginInvoke(archiveStream, commonRootLength, password,
                fileFullNames, AsyncCallbackImplementation, this);
        }
        #endregion

        #region BeginCompressDirectory overloads

#if !CS4
#if !DOTNET20
        /// <summary>
        /// Recursively packs all files in the specified directory.
        /// </summary>
        /// <param name="directory">The directory to compress.</param>
        /// <param name="archiveName">The archive file name.</param>
        /// <param name="eventPriority">The priority of events, relative to the other pending operations in the System.Windows.Threading.Dispatcher event queue, the specified method is invoked.</param>
#else
        /// <summary>
        /// Recursively packs all files in the specified directory.
        /// </summary>
        /// <param name="directory">The directory to compress.</param>
        /// <param name="archiveName">The archive file name.</param>
#endif
        public void BeginCompressDirectory(
            string directory, string archiveName
#if !DOTNET20
            , DispatcherPriority eventPriority
#endif
        )
        {
            SaveContext(
#if !DOTNET20
                eventPriority
#endif
            );
            (new CompressDirectory1Delegate(CompressDirectory)).BeginInvoke(directory, archiveName,
                AsyncCallbackImplementation, this);
        }

#if !DOTNET20
        /// <summary>
        /// Recursively packs all files in the specified directory.
        /// </summary>
        /// <param name="directory">The directory to compress.</param>
        /// <param name="archiveStream">The archive output stream.
        /// Use CompressDirectory( ... string archiveName ... ) overloads for archiving to disk.</param>
        /// <param name="eventPriority">The priority of events, relative to the other pending operations in the System.Windows.Threading.Dispatcher event queue, the specified method is invoked.</param>
#else
        /// <summary>
        /// Recursively packs all files in the specified directory.
        /// </summary>
        /// <param name="directory">The directory to compress.</param>
        /// <param name="archiveStream">The archive output stream.
        /// Use CompressDirectory( ... string archiveName ... ) overloads for archiving to disk.</param>
#endif
        public void BeginCompressDirectory(
            string directory, Stream archiveStream
#if !DOTNET20
            , DispatcherPriority eventPriority
#endif
        )
        {
            SaveContext(
#if !DOTNET20
                eventPriority
#endif
            );
            (new CompressDirectory2Delegate(CompressDirectory)).BeginInvoke(directory, archiveStream,
                AsyncCallbackImplementation, this);
        }

#if !DOTNET20
        /// <summary>
        /// Recursively packs all files in the specified directory.
        /// </summary>
        /// <param name="directory">The directory to compress.</param>
        /// <param name="archiveName">The archive file name.</param>
        /// <param name="password">The archive password.</param>
        /// <param name="eventPriority">The priority of events, relative to the other pending operations in the System.Windows.Threading.Dispatcher event queue, the specified method is invoked.</param>
#else
        /// <summary>
        /// Recursively packs all files in the specified directory.
        /// </summary>
        /// <param name="directory">The directory to compress.</param>
        /// <param name="archiveName">The archive file name.</param>
        /// <param name="password">The archive password.</param>
#endif
        public void BeginCompressDirectory(
            string directory, string archiveName, string password
#if !DOTNET20
            , DispatcherPriority eventPriority
#endif
        )
        {
            SaveContext(
#if !DOTNET20
                eventPriority
#endif
            );
            (new CompressDirectory3Delegate(CompressDirectory)).BeginInvoke(directory, archiveName,
                password, AsyncCallbackImplementation, this);
        }

#if !DOTNET20
        /// <summary>
        /// Recursively packs all files in the specified directory.
        /// </summary>
        /// <param name="directory">The directory to compress.</param>
        /// <param name="archiveStream">The archive output stream.
        /// Use CompressDirectory( ... string archiveName ... ) overloads for archiving to disk.</param>
        /// <param name="password">The archive password.</param>
        /// <param name="eventPriority">The priority of events, relative to the other pending operations in the System.Windows.Threading.Dispatcher event queue, the specified method is invoked.</param>
#else
        /// <summary>
        /// Recursively packs all files in the specified directory.
        /// </summary>
        /// <param name="directory">The directory to compress.</param>
        /// <param name="archiveStream">The archive output stream.
        /// Use CompressDirectory( ... string archiveName ... ) overloads for archiving to disk.</param>
        /// <param name="password">The archive password.</param>
#endif
        public void BeginCompressDirectory(
            string directory, Stream archiveStream, string password
#if !DOTNET20
            , DispatcherPriority eventPriority
#endif
        )
        {
            SaveContext(
#if !DOTNET20
                eventPriority
#endif
            );
            (new CompressDirectory4Delegate(CompressDirectory)).BeginInvoke(directory, archiveStream,
                password, AsyncCallbackImplementation, this);
        }
#endif

#if !DOTNET20
        /// <summary>
        /// Packs all files in the specified directory asynchronously.
        /// </summary>
        /// <param name="directory">The directory to compress.</param>
        /// <param name="archiveName">The archive file name.</param>        
        /// <param name="password">The archive password.</param>
        /// <param name="searchPattern">Search string, such as "*.txt".</param>
        /// <param name="recursion">If true, files will be searched for recursively; otherwise, not.</param>
        /// <param name="eventPriority">The priority of events, relative to the other pending operations in the System.Windows.Threading.Dispatcher event queue, the specified method is invoked.</param>
#else
        /// <summary>
        /// Packs all files in the specified directory asynchronously.
        /// </summary>
        /// <param name="directory">The directory to compress.</param>
        /// <param name="archiveName">The archive file name.</param>        
        /// <param name="password">The archive password.</param>
        /// <param name="searchPattern">Search string, such as "*.txt".</param>
        /// <param name="recursion">If true, files will be searched for recursively; otherwise, not.</param>
#endif
        public void BeginCompressDirectory(string directory, string archiveName,
            string password 
#if CS4
            = ""
#endif
            , string searchPattern 
#if CS4
            = "*"
#endif
            , bool recursion 
#if CS4
            = true
#endif
#if !DOTNET20
            , DispatcherPriority eventPriority
#if CS4
            = DispatcherPriority.Normal
#endif
#endif
)
        {
            SaveContext(
#if !DOTNET20
                eventPriority
#endif
            );
            (new CompressDirectory5Delegate(CompressDirectory)).BeginInvoke(directory, archiveName,
                password, searchPattern, recursion, AsyncCallbackImplementation, this);
        }

#if !DOTNET20
        /// <summary>
        /// Packs all files in the specified directory asynchronously.
        /// </summary>
        /// <param name="directory">The directory to compress.</param>
        /// <param name="archiveStream">The archive output stream.
        /// Use CompressDirectory( ... string archiveName ... ) overloads for archiving to disk.</param>        
        /// <param name="password">The archive password.</param>
        /// <param name="searchPattern">Search string, such as "*.txt".</param>
        /// <param name="recursion">If true, files will be searched for recursively; otherwise, not.</param>
        /// <param name="eventPriority">The priority of events, relative to the other pending operations in the System.Windows.Threading.Dispatcher event queue, the specified method is invoked.</param>
#else
        /// <summary>
        /// Packs all files in the specified directory asynchronously.
        /// </summary>
        /// <param name="directory">The directory to compress.</param>
        /// <param name="archiveStream">The archive output stream.
        /// Use CompressDirectory( ... string archiveName ... ) overloads for archiving to disk.</param>        
        /// <param name="password">The archive password.</param>
        /// <param name="searchPattern">Search string, such as "*.txt".</param>
        /// <param name="recursion">If true, files will be searched for recursively; otherwise, not.</param>
#endif
        public void BeginCompressDirectory(string directory, Stream archiveStream,
            string password 
#if CS4
            = ""
#endif
            , string searchPattern 
#if CS4
            = "*"
#endif
            , bool recursion 
#if CS4
            = true
#endif
#if !DOTNET20
            , DispatcherPriority eventPriority 
#if CS4
            = DispatcherPriority.Normal
#endif
#endif
            )
        {
            SaveContext(
#if !DOTNET20
                eventPriority
#endif
            );
            (new CompressDirectory6Delegate(CompressDirectory)).BeginInvoke(directory, archiveStream,
                password, searchPattern, recursion, AsyncCallbackImplementation, this);
        }
#endregion

        #region BeginCompressStream overloads
#if !CS4
#if !DOTNET20
        /// <summary>
        /// Compresses the specified stream.
        /// </summary>
        /// <param name="inStream">The source uncompressed stream.</param>
        /// <param name="outStream">The destination compressed stream.</param>
        /// <exception cref="ArgumentException">ArgumentException: at least one of the specified streams is invalid.</exception>
        /// <param name="eventPriority">The priority of events, relative to the other pending operations in the System.Windows.Threading.Dispatcher event queue, the specified method is invoked.</param>
#else
        /// <summary>
        /// Compresses the specified stream.
        /// </summary>
        /// <param name="inStream">The source uncompressed stream.</param>
        /// <param name="outStream">The destination compressed stream.</param>
        /// <exception cref="System.ArgumentException">ArgumentException: at least one of the specified streams is invalid.</exception>
#endif
        public void BeginCompressStream(Stream inStream, Stream outStream
#if !DOTNET20
            , DispatcherPriority eventPriority 
#endif
            )
        {
            SaveContext(
#if !DOTNET20
                eventPriority
#endif
                );
            (new CompressStream1Delegate(CompressStream)).BeginInvoke(inStream, outStream, AsyncCallbackImplementation, this);            
        }       
#endif

#if !DOTNET20
        /// <summary>
        /// Compresses the specified stream.
        /// </summary>
        /// <param name="inStream">The source uncompressed stream.</param>
        /// <param name="outStream">The destination compressed stream.</param>
        /// <param name="password">The archive password.</param>
        /// <exception cref="System.ArgumentException">ArgumentException: at least one of the specified streams is invalid.</exception>
        /// <param name="eventPriority">The priority of events, relative to the other pending operations in the System.Windows.Threading.Dispatcher event queue, the specified method is invoked.</param>
#else
        /// <summary>
        /// Compresses the specified stream.
        /// </summary>
        /// <param name="inStream">The source uncompressed stream.</param>
        /// <param name="outStream">The destination compressed stream.</param>
        /// <param name="password">The archive password.</param>
        /// <exception cref="System.ArgumentException">ArgumentException: at least one of the specified streams is invalid.</exception>
#endif
        public void BeginCompressStream(Stream inStream, Stream outStream, string password
#if !DOTNET20
            , DispatcherPriority eventPriority
#if CS4
            = DispatcherPriority.Normal
#endif
#endif
        )
        {
            SaveContext(
#if !DOTNET20
                eventPriority
#endif
                );
            (new CompressStream2Delegate(CompressStream)).BeginInvoke(inStream, outStream, password, AsyncCallbackImplementation, this);            

        }
        #endregion
        
        #region BeginModifyArchive overloads
#if !CS4
#if !DOTNET20
        /// <summary>
        /// Modifies the existing archive asynchronously (renames files or deletes them).
        /// </summary>
        /// <param name="archiveName">The archive file name.</param>
        /// <param name="newFileNames">New file names. Null value to delete the corresponding index.</param>
        /// <param name="eventPriority">The priority of events, relative to the other pending operations in the System.Windows.Threading.Dispatcher event queue, the specified method is invoked.</param>
#else
        /// <summary>
        /// Modifies the existing archive asynchronously (renames files or deletes them).
        /// </summary>
        /// <param name="archiveName">The archive file name.</param>
        /// <param name="newFileNames">New file names. Null value to delete the corresponding index.</param>
#endif
        public void BeginModifyArchive(string archiveName, Dictionary<int, string> newFileNames
#if !DOTNET20
            , DispatcherPriority eventPriority
#endif
        )
        {
            SaveContext(
#if !DOTNET20
                eventPriority
#endif
            );
            (new ModifyArchive1Delegate(ModifyArchive)).BeginInvoke(archiveName, newFileNames, AsyncCallbackImplementation, this);
        }
#endif

#if !DOTNET20
        /// <summary>
        /// Modifies the existing archive asynchronously (renames files or deletes them).
        /// </summary>
        /// <param name="archiveName">The archive file name.</param>
        /// <param name="newFileNames">New file names. Null value to delete the corresponding index.</param>
        /// <param name="password">The archive password.</param>
        /// <param name="eventPriority">The priority of events, relative to the other pending operations in the System.Windows.Threading.Dispatcher event queue, the specified method is invoked.</param>
#else
        /// <summary>
        /// Modifies the existing archive asynchronously (renames files or deletes them).
        /// </summary>
        /// <param name="archiveName">The archive file name.</param>
        /// <param name="newFileNames">New file names. Null value to delete the corresponding index.</param>
        /// <param name="password">The archive password.</param>
#endif
        public void BeginModifyArchive(string archiveName, Dictionary<int, string> newFileNames,
            string password
#if CS4
            = ""
#endif
#if !DOTNET20
            , DispatcherPriority eventPriority
#if CS4
            = DispatcherPriority.Normal
#endif
#endif
        )
        {
            SaveContext(
#if !DOTNET20
                eventPriority
#endif
                );
            (new ModifyArchive2Delegate(ModifyArchive)).BeginInvoke(archiveName, newFileNames, password, AsyncCallbackImplementation, this);
        }
        #endregion
    }
}
