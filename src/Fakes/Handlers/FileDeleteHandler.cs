﻿using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using TestableFileSystem.Fakes.HandlerArguments;
using TestableFileSystem.Fakes.Resolvers;
using TestableFileSystem.Interfaces;

namespace TestableFileSystem.Fakes.Handlers
{
    internal sealed class FileDeleteHandler : FakeOperationHandler<FileDeleteArguments, object>
    {
        public FileDeleteHandler([NotNull] DirectoryEntry root)
            : base(root)
        {
        }

        public override object Handle(FileDeleteArguments arguments)
        {
            Guard.NotNull(arguments, nameof(arguments));

            var resolver = new FileResolver(Root);
            (DirectoryEntry containingDirectory, FileEntry existingFileOrNull, string _) =
                resolver.TryResolveFile(arguments.Path);

            if (existingFileOrNull != null)
            {
                DeleteFile(existingFileOrNull, containingDirectory, arguments);
            }

            return Missing.Value;
        }

        private void DeleteFile([NotNull] FileEntry existingFile, [NotNull] DirectoryEntry containingDirectory,
            [NotNull] FileDeleteArguments arguments)
        {
            AssertIsNotReadOnly(existingFile, arguments.Path);
            AssertHasExclusiveAccess(existingFile, arguments.Path);

            containingDirectory.DeleteFile(existingFile.Name);
        }

        [AssertionMethod]
        private void AssertIsNotReadOnly([NotNull] FileEntry fileEntry, [NotNull] AbsolutePath absolutePath)
        {
            if (fileEntry.Attributes.HasFlag(FileAttributes.ReadOnly))
            {
                throw ErrorFactory.System.UnauthorizedAccess(absolutePath.GetText());
            }
        }

        private static void AssertHasExclusiveAccess([NotNull] FileEntry file, [NotNull] AbsolutePath absolutePath)
        {
            if (file.IsOpen())
            {
                throw ErrorFactory.System.FileIsInUse(absolutePath.GetText());
            }
        }
    }
}
