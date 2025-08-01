using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ClickUp.Api.Client.Abstractions.Infrastructure;

namespace ClickUp.Api.Client.Infrastructure
{
    /// <summary>
    /// Concrete implementation of IFileSystem that uses system file operations.
    /// This implementation provides dependency inversion while using standard system file I/O operations.
    /// </summary>
    /// <remarks>
    /// This implementation allows the ClickUp SDK to:
    /// - Use standard system file operations for production scenarios
    /// - Support dependency injection and testability
    /// - Provide consistent file handling across the application
    /// - Support cross-platform file operations
    /// </remarks>
    public class SystemFileSystem : IFileSystem
    {
        /// <summary>
        /// Checks if a file exists at the specified path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>True if the file exists; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when path is null.</exception>
        public bool FileExists(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return File.Exists(path);
        }

        /// <summary>
        /// Checks if a directory exists at the specified path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>True if the directory exists; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when path is null.</exception>
        public bool DirectoryExists(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return Directory.Exists(path);
        }

        /// <summary>
        /// Reads all text from a file asynchronously.
        /// </summary>
        /// <param name="path">The path to the file to read.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task containing the file contents as a string.</returns>
        /// <exception cref="ArgumentNullException">Thrown when path is null.</exception>
        public async Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return await File.ReadAllTextAsync(path, cancellationToken);
        }

        /// <summary>
        /// Writes text to a file asynchronously.
        /// </summary>
        /// <param name="path">The path to the file to write.</param>
        /// <param name="content">The content to write to the file.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when path or content is null.</exception>
        public async Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken = default)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            await File.WriteAllTextAsync(path, content, cancellationToken);
        }

        /// <summary>
        /// Reads all bytes from a file asynchronously.
        /// </summary>
        /// <param name="path">The path to the file to read.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task containing the file contents as a byte array.</returns>
        /// <exception cref="ArgumentNullException">Thrown when path is null.</exception>
        public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return await File.ReadAllBytesAsync(path, cancellationToken);
        }

        /// <summary>
        /// Writes bytes to a file asynchronously.
        /// </summary>
        /// <param name="path">The path to the file to write.</param>
        /// <param name="bytes">The bytes to write to the file.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when path or bytes is null.</exception>
        public async Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            await File.WriteAllBytesAsync(path, bytes, cancellationToken);
        }

        /// <summary>
        /// Opens a file stream for reading.
        /// </summary>
        /// <param name="path">The path to the file to open.</param>
        /// <returns>A stream for reading the file.</returns>
        /// <exception cref="ArgumentNullException">Thrown when path is null.</exception>
        public Stream OpenRead(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return File.OpenRead(path);
        }

        /// <summary>
        /// Opens a file stream for writing.
        /// </summary>
        /// <param name="path">The path to the file to open.</param>
        /// <returns>A stream for writing to the file.</returns>
        /// <exception cref="ArgumentNullException">Thrown when path is null.</exception>
        public Stream OpenWrite(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return File.OpenWrite(path);
        }

        /// <summary>
        /// Creates a directory at the specified path.
        /// </summary>
        /// <param name="path">The path where the directory should be created.</param>
        /// <exception cref="ArgumentNullException">Thrown when path is null.</exception>
        public void CreateDirectory(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            Directory.CreateDirectory(path);
        }

        /// <summary>
        /// Deletes a file at the specified path.
        /// </summary>
        /// <param name="path">The path to the file to delete.</param>
        /// <exception cref="ArgumentNullException">Thrown when path is null.</exception>
        public void DeleteFile(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Deletes a directory at the specified path.
        /// </summary>
        /// <param name="path">The path to the directory to delete.</param>
        /// <param name="recursive">Whether to delete the directory recursively.</param>
        /// <exception cref="ArgumentNullException">Thrown when path is null.</exception>
        public void DeleteDirectory(string path, bool recursive = false)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive);
            }
        }

        /// <summary>
        /// Gets the files in a directory.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
        /// <param name="searchPattern">The search pattern to match files against.</param>
        /// <returns>An enumerable of file paths.</returns>
        /// <exception cref="ArgumentNullException">Thrown when path or searchPattern is null.</exception>
        public IEnumerable<string> GetFiles(string path, string searchPattern = "*")
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (searchPattern == null)
                throw new ArgumentNullException(nameof(searchPattern));

            return Directory.GetFiles(path, searchPattern);
        }

        /// <summary>
        /// Gets the directories in a directory.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
        /// <returns>An enumerable of directory paths.</returns>
        /// <exception cref="ArgumentNullException">Thrown when path is null.</exception>
        public IEnumerable<string> GetDirectories(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return Directory.GetDirectories(path);
        }

        /// <summary>
        /// Gets information about a file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>File information including size, creation time, etc.</returns>
        /// <exception cref="ArgumentNullException">Thrown when path is null.</exception>
        public FileInfo GetFileInfo(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return new FileInfo(path);
        }

        /// <summary>
        /// Gets information about a directory.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
        /// <returns>Directory information including creation time, etc.</returns>
        /// <exception cref="ArgumentNullException">Thrown when path is null.</exception>
        public DirectoryInfo GetDirectoryInfo(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return new DirectoryInfo(path);
        }

        /// <summary>
        /// Combines path segments into a single path.
        /// </summary>
        /// <param name="paths">The path segments to combine.</param>
        /// <returns>The combined path.</returns>
        /// <exception cref="ArgumentNullException">Thrown when paths is null.</exception>
        public string CombinePath(params string[] paths)
        {
            if (paths == null)
                throw new ArgumentNullException(nameof(paths));

            return Path.Combine(paths);
        }

        /// <summary>
        /// Gets the directory name from a path.
        /// </summary>
        /// <param name="path">The path to get the directory name from.</param>
        /// <returns>The directory name.</returns>
        /// <exception cref="ArgumentNullException">Thrown when path is null.</exception>
        public string? GetDirectoryName(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return Path.GetDirectoryName(path);
        }

        /// <summary>
        /// Gets the file name from a path.
        /// </summary>
        /// <param name="path">The path to get the file name from.</param>
        /// <returns>The file name.</returns>
        /// <exception cref="ArgumentNullException">Thrown when path is null.</exception>
        public string GetFileName(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return Path.GetFileName(path);
        }

        /// <summary>
        /// Gets the file name without extension from a path.
        /// </summary>
        /// <param name="path">The path to get the file name from.</param>
        /// <returns>The file name without extension.</returns>
        /// <exception cref="ArgumentNullException">Thrown when path is null.</exception>
        public string GetFileNameWithoutExtension(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return Path.GetFileNameWithoutExtension(path);
        }

        /// <summary>
        /// Gets the file extension from a path.
        /// </summary>
        /// <param name="path">The path to get the extension from.</param>
        /// <returns>The file extension.</returns>
        /// <exception cref="ArgumentNullException">Thrown when path is null.</exception>
        public string GetExtension(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return Path.GetExtension(path);
        }
    }
}