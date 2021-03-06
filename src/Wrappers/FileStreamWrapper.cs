﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Win32.SafeHandles;
using TestableFileSystem.Interfaces;

namespace TestableFileSystem.Wrappers
{
    public sealed class FileStreamWrapper : IFileStream
    {
        [NotNull]
        private readonly Func<string> getName;

        [NotNull]
        private readonly Func<bool> getIsAsync;

        [NotNull]
        private readonly Func<SafeFileHandle> getSafeFileHandle;

        [NotNull]
        private readonly Action<bool> doFlush;

        [NotNull]
        private readonly Stream innerStream;

        public bool CanRead => innerStream.CanRead;

        public bool CanSeek => innerStream.CanSeek;

        public bool CanWrite => innerStream.CanWrite;

        public bool CanTimeout => innerStream.CanTimeout;

        public int ReadTimeout
        {
            get => innerStream.ReadTimeout;
            set => innerStream.ReadTimeout = value;
        }

        public int WriteTimeout
        {
            get => innerStream.WriteTimeout;
            set => innerStream.WriteTimeout = value;
        }

        public string Name => getName();

        public long Length => innerStream.Length;

        public long Position
        {
            get => innerStream.Position;
            set => innerStream.Position = value;
        }

        public bool IsAsync => getIsAsync();

        public SafeFileHandle SafeFileHandle => getSafeFileHandle();

        public FileStreamWrapper([NotNull] FileStream source)
            : this(source, () => source.Name, () => source.IsAsync, () => source.SafeFileHandle, source.Flush)
        {
        }

        public FileStreamWrapper([NotNull] Stream source, [NotNull] Func<string> getName, [NotNull] Func<bool> getIsAsync,
            [NotNull] Func<SafeFileHandle> getSafeFileHandle, [NotNull] Action<bool> doFlush)
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotNull(getName, nameof(getName));
            Guard.NotNull(getIsAsync, nameof(getIsAsync));
            Guard.NotNull(getSafeFileHandle, nameof(getSafeFileHandle));
            Guard.NotNull(doFlush, nameof(doFlush));

            innerStream = source;
            this.getName = getName;
            this.getIsAsync = getIsAsync;
            this.getSafeFileHandle = getSafeFileHandle;
            this.doFlush = doFlush;
        }

        public long Seek(long offset, SeekOrigin origin)
        {
            return innerStream.Seek(offset, origin);
        }

        public void SetLength(long value)
        {
            innerStream.SetLength(value);
        }

        public void Flush(bool flushToDisk = false)
        {
            doFlush(flushToDisk);
        }

        public Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return innerStream.FlushAsync(cancellationToken);
        }

        public int ReadByte()
        {
            return innerStream.ReadByte();
        }

        public int Read(byte[] array, int offset, int count)
        {
            return innerStream.Read(array, offset, count);
        }

        public Task<int> ReadAsync(byte[] buffer, int offset, int count,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return innerStream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public void WriteByte(byte value)
        {
            innerStream.WriteByte(value);
        }

        public void Write(byte[] array, int offset, int count)
        {
            innerStream.Write(array, offset, count);
        }

        public Task WriteAsync(byte[] buffer, int offset, int count,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return innerStream.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public void CopyTo(Stream destination, int bufferSize = 81920)
        {
            innerStream.CopyTo(destination, bufferSize);
        }

        public Task CopyToAsync(Stream destination, int bufferSize = 81920,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return innerStream.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public Stream AsStream()
        {
            return innerStream;
        }

        public void Dispose()
        {
            innerStream.Dispose();
        }
    }
}
