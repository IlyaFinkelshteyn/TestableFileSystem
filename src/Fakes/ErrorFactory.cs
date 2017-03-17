﻿using System;
using System.IO;
using JetBrains.Annotations;

namespace TestableFileSystem.Fakes
{
    public static class ErrorFactory
    {
        [NotNull]
        public static Exception FileIsInUse()
        {
            return new IOException("The process cannot access the file because it is being used by another process.");
        }

        [NotNull]
        public static Exception FileIsInUse([NotNull] string path)
        {
            string message = $"The process cannot access the file '{path}' because it is being used by another process.";
            return new IOException(message);
        }

        [NotNull]
        public static Exception CannotCreateBecauseFileAlreadyExists([NotNull] string path)
        {
            string message = $"The file '{path}' already exists.";
            return new IOException(message);
        }

        [NotNull]
        public static Exception CannotMoveBecauseFileAlreadyExists()
        {
            return new IOException("Cannot create a file when that file already exists.");
        }

        [NotNull]
        public static Exception CannotMoveBecauseTargetIsInvalid()
        {
            return new IOException("The filename, directory name, or volume label syntax is incorrect.");
        }

        [NotNull]
        public static Exception DirectoryIsNotEmpty()
        {
            return new IOException("The directory is not empty.");
        }

        [NotNull]
        public static Exception DirectoryNotFound()
        {
            return new DirectoryNotFoundException("Could not find a part of the path.");
        }

        [NotNull]
        public static Exception DirectoryNotFound([NotNull] string path)
        {
            return new DirectoryNotFoundException($"Could not find a part of the path '{path}'.");
        }

        [NotNull]
        public static Exception FileNotFound([NotNull] string path)
        {
            return new FileNotFoundException($"Could not find file '{path}'.");
        }

        [NotNull]
        public static Exception PathIsNotLegal([NotNull] [InvokerParameterName] string paramName)
        {
            return new ArgumentException("The path is not of a legal form.", paramName);
        }

        [NotNull]
        public static Exception PathIsInvalid()
        {
            return new IOException("The specified path is invalid.");
        }
    }
}
