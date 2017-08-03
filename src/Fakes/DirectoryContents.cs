﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using TestableFileSystem.Interfaces;

namespace TestableFileSystem.Fakes
{
    internal sealed class DirectoryContents
    {
        [NotNull]
        private readonly DirectoryEntry owner;

        [NotNull]
        private readonly IDictionary<string, BaseEntry> entries =
            new Dictionary<string, BaseEntry>(StringComparer.OrdinalIgnoreCase);

        public bool IsEmpty => !entries.Any();

        public DirectoryContents([NotNull] DirectoryEntry owner)
        {
            Guard.NotNull(owner, nameof(owner));
            this.owner = owner;
        }

        [NotNull]
        public DirectoryEntry GetEntryAsDirectory([NotNull] string name)
        {
            DirectoryEntry directory = TryGetEntryAsDirectory(name);
            if (directory == null)
            {
                throw ErrorFactory.Internal.UnknownError($"Directory '{name}' not found.");
            }

            return directory;
        }

        [CanBeNull]
        private DirectoryEntry TryGetEntryAsDirectory([NotNull] string name, bool throwIfExistsAsFile = true)
        {
            Guard.NotNull(name, nameof(name));

            if (entries.ContainsKey(name))
            {
                var directory = entries[name] as DirectoryEntry;
                if (directory != null)
                {
                    return directory;
                }

                if (throwIfExistsAsFile)
                {
                    string pathUpToHere = Path.Combine(owner.GetAbsolutePath(), name);
                    throw ErrorFactory.System.CannotCreateBecauseFileOrDirectoryAlreadyExists(pathUpToHere);
                }
            }

            return null;
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<DirectoryEntry> GetDirectoryEntries()
        {
            foreach (DirectoryEntry entry in entries.Values.OfType<DirectoryEntry>())
            {
                yield return entry;
            }
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<FileEntry> GetFileEntries()
        {
            foreach (FileEntry entry in entries.Values.OfType<FileEntry>())
            {
                yield return entry;
            }
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<BaseEntry> GetEntries(EnumerationFilter filter)
        {
            switch (filter)
            {
                case EnumerationFilter.Files:
                    return entries.Values.OfType<FileEntry>();
                case EnumerationFilter.Directories:
                    return entries.Values.OfType<DirectoryEntry>();
                case EnumerationFilter.All:
                    return entries.Values;
                default:
                    throw new NotSupportedException($"Unsupported filter '{filter}'.");
            }
        }

        public void Add([NotNull] BaseEntry entry)
        {
            Guard.NotNull(entry, nameof(entry));

            entries[entry.Name] = entry;
        }

        public void Remove([NotNull] string name)
        {
            Guard.NotNull(name, nameof(name));

            entries.Remove(name);
        }

        [AssertionMethod]
        public void AssertEntryDoesNotExist([NotNull] string name)
        {
            Guard.NotNull(name, nameof(name));

            if (entries.ContainsKey(name))
            {
                throw entries[name] is DirectoryEntry
                    ? ErrorFactory.Internal.UnknownError($"Expected not to find an existing directory named '{name}'.")
                    : ErrorFactory.Internal.UnknownError($"Expected not to find an existing file named '{name}'.");
            }
        }

        public override string ToString()
        {
            return
                $"{entries.Values.OfType<FileEntry>().Count()} files, {entries.Values.OfType<DirectoryEntry>().Count()} directories";
        }
    }
}
