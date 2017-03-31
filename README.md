# TestableFileSystem
Abstractions for `System.IO.*` including in-memory fakes, intended for unit-testing.

Requires Visual Studio 2017. This project is a work in progress, please be patient for stable bits...

# Features of the fake filesystem
* Concurrent access to the in-memory filesystem is thread-safe
* You'll get appropriate exceptions for files that are in use
* Fails on changing readonly files and directories
* Supports absolute and relative paths, based on settable current directory
* Supports local and UNC (Universal Naming Convention) network paths
* Paths are case-insensitive

# Limitations of the fake filesystem
* Limitations around MAXPATH do not apply (paths starting with `\\?\` are allowed)
* Device namespaces (for example: `\\.\COM56`) are not supported
* Exceptions may have slightly different messages (but matching type)
* Some file attributes, such as Compressed/Encrypted you will never get (they are set by nonstandard APIs)
* NTFS permissions are not implemented
* A file cannot be opened by multiple writers at the same time
* Hard links, junctions and symbolic links (reparse points) are not implemented
* 8.3 aliases for file names are not implemented
