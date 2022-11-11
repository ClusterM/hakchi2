---
Name: tiny7zx
Category: System
Creator: Péter Szabó
Module Contributors: cluster, madmonkey and KMFDManic
---
This module will add support for 7z Compressed Games to RetroArch

Credit goes to Péter Szabó

pts-tiny-7z-sfx is a tiny 7-Zip (.7z archive) extractor and self-extractor
(SFX) written in standard C. It's Unix-only, actively tested on Linux.

## Features:

* Small (the Linux statically linked executable is less than 17 kB).
* Can be used stand-alone to extract .7z archives.
* Can be used to create an SFX (self-extracting) executable by prepending it to a .7z archive. (Same as the `7z -sfx` flag.)
* It supports file and directory attributes (i.e. it calls chmod(2)).
* It sets the mtime (i.e. it calls utimes(2)) of extracted files.
* It can extract symlinks.
* It has a command-line syntax compatible with the regular console SFX binaries.
* It's implemented in standard C (and C++).
* Its command-line is compatible with `7z`, `rar`, `unrar` and `unzip`.
* It refuses to modify files with unsafe names (e.g. ../../../etc/passwd).

## Limitations:

* It supports only these compressors: LZMA, LZMA2, BCJ, BCJ2, COPY.
* Memory usage can be high, especially for solid archives. See below.
* It always extracts to the current directory.
* It does not support (and may misbehave for) encryption in archives.
* It works on Unix only (tested on Linux). Doesn't work on Windows.
* It doesn't restore Unix file owners (UID, GID).
* It doesn't restore Unix file extended attributes.
* It doesn't restore Unix character devices, block devices, sockets or pipes.
* It is not able to run a command (via RunProgram) after extraction.

## Memory usage
Info about memory usage during extraction:

* It keeps an uncompressed version of each file in memory before writing the file.
* It decompresses each solid block (it can be the whole .7z archive) to memory.
* You can limit the memory usage of decompression by specifying `7z -m...' flags when creating the .7z archive.
* The dictionary size (`7z -md=...') doesn't matter for memory usage.
* Only the solid block size (`7z -ms=...') matters. The default can be very high (up to 4 GB), so always specify something small (e.g. `7z -ms=50000000b') or turn off solid blocks (`7z -ms=off').
* The memory usage of c-minidiet.c will be (most of the time):
```
  total_memory_usage_for_tiny7zx_decompression <=
      static_memory_size +
      archive_header_size +
      listing_structures_size +
      max([solid_block_size] + uncompressed_file_sizes).
  static_memory_size == 100 000 bytes.
  archive_header_size == file_count * 32 bytes + sum(filename_sizes).
  filename_sizes counts each character as 2 bytes (because of UTF-16 encoding).
  listing_structures_size == file_count * 56 bytes.
  solid_block_size == value of `7z -ms=...', or 0 if `7z -ms=off'.
      Be careful, the default can be as large as 4 GB.
  uncompressed_file_sizes: List of uncompressed file sizes in the archive.
  file_count and file_size include both files and directories (folders).
```

## License
pts-tiny-7z-sfx is released under the GNU GPL v2.

## Related software
See http://sourceforge.net/p/sevenzip/discussion/45797/thread/233f5efd about 7zS2con.sfx, a similar software for Windows.

Forked from 7z922.tar.bz2 from http://sourceforge.net/projects/sevenzip/files/7-Zip/9.22/7z922.tar.bz2/download

Module by Péter Szabó/Updated by cluster, madmonkey and KMFDManic  
Hakchi module system by madmonkey  
NES Mini shell integration by Cluster  
(c) 2016-2017
