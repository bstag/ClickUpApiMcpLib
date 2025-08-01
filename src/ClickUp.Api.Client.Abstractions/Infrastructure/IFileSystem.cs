using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ClickUp.Api.Client.Abstractions.Infrastructure
{
    /// <summary>
    /// Abstraction for file system operations to support dependency inversion principle and testability.
    /// This interface provides a testable abstraction over file system operations.
    /// </summary>
    /// <remarks>
    /// This abstraction allows for:
    /// - Easy unit testing by providing mock implementations
    /// - Cross-platform file system operations
    /// - Consistent error handling for file operations
    /// - Support for different file system strategies (local, cloud, in-memory)
    /// </remarks>
    public interface IFileSystem
    {
        /// <summary>
        /// Checks if a file exists at the specified path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>True if the file exists; otherwise, false.</returns>
        bool FileExists(string path);

        /// <summary>
        /// Checks if a directory exists at the specified path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>True if the directory exists; otherwise, false.</returns>
        bool DirectoryExists(string path);

        /// <summary>
        /// Reads all text from a file asynchronously.
        /// </summary>
        /// <param name="path">The path to the file to read.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task containing the file contents as a string.</returns>
        Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default);

        /// <summary>
        /// Writes text to a file asynchronously.
        /// </summary>
        /// <param name="path">The path to the file to write.</param>
        /// <param name="content">The content to write to the file.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads all bytes from a file asynchronously.
        /// </summary>
        /// <param name="path">The path to the file to read.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task containing the file contents as a byte array.</returns>
        Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default);

        /// <summary>
        /// Writes bytes to a file asynchronously.
        /// </summary>
        /// <param name="path">The path to the file to write.</param>
        /// <param name="bytes">The bytes to write to the file.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default);

        /// <summary>
        /// Opens a file stream for reading.
        /// </summary>
        /// <param name="path">The path to the file to open.</param>
        /// <returns>A stream for reading the file.</returns>
        Stream OpenRead(string path);

        /// <summary>
        /// Opens a file stream for writing.
        /// </summary>
        /// <param name="path">The path to the file to open.</param>
        /// <returns>A stream for writing to the file.</returns>
        Stream OpenWrite(string path);

        /// <summary>
        /// Creates a directory at the specified path.
        /// </summary>
        /// <param name="path">The path where the directory should be created.</param>
        void CreateDirectory(string path);

        /// <summary>
        /// Deletes a file at the specified path.
        /// </summary>
        /// <param name="path">The path to the file to delete.</param>
        void DeleteFile(string path);

        /// <summary>
        /// Deletes a directory at the specified path.
        /// </summary>
        /// <param name="path">The path to the directory to delete.</param>
        /// <param name="recursive">Whether to delete the directory recursively.</param>
        void DeleteDirectory(string path, bool recursive = false);

        /// <summary>
        /// Gets the files in a directory.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
        /// <param name="searchPattern">The search pattern to match files against.</param>
        /// <returns>An enumerable of file paths.</returns>
        IEnumerable<string> GetFiles(string path, string searchPattern = "*");

        /// <summary>
        /// Gets the directories in a directory.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
        /// <returns>An enumerable of directory paths.</returns>
        IEnumerable<string> GetDirectories(string path);

        /// <summary>
        /// Gets information about a file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>File information including size, creation time, etc.</returns>
        FileInfo GetFileInfo(string path);

        /// <summary>
        /// Gets information about a directory.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
        /// <returns>Directory information including creation time, etc.</returns>
        DirectoryInfo GetDirectoryInfo(string path);

        /// <summary>
        /// Combines path segments into a single path.
        /// </summary>
        /// <param name="paths">The path segments to combine.</param>
        /// <returns>The combined path.</returns>
        string CombinePath(params string[] paths);

        /// <summary>
        /// Gets the directory name from a path.
        /// </summary>
        /// <param name="path">The path to get the directory name from.</param>
        /// <returns>The directory name.</returns>
        string? GetDirectoryName(string path);

        /// <summary>
        /// Gets the file name from a path.
        /// </summary>
        /// <param name="path">The path to get the file name from.</param>
        /// <returns>The file name.</returns>
        string GetFileName(string path);

        /// <summary>
        /// Gets the file name without extension from a path.
        /// </summary>
        /// <param name="path">The path to get the file name from.</param>
        /// <returns>The file name without extension.</returns>
        string GetFileNameWithoutExtension(string path);

        /// <summary>
        /// Gets the file extension from a path.
        /// </summary>
        /// <param name="path">The path to get the extension from.</param>
        /// <returns>The file extension.</returns>
        string GetExtension(string path);
    }
}