using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTFO.Utilities
{
    /// <summary>
    /// Represents a path inside the game
    /// </summary>
    public abstract class GamePath
    {
        public abstract GameFile PathFile { get; }
        public abstract GameDirectory PathDir { get; }
        public abstract IDirectory GetCurrentDirectory();

        internal static GamePath Create(string path)
        {
            return new GameFolderPath(path);
        }

        private sealed class GameFolderPath : GamePath
        {
            private readonly string m_folder;

            public override GameDirectory PathDir { get; }
            public override GameFile PathFile { get; }

            public GameFolderPath(string folder)
            {
                this.m_folder = folder;
                this.PathDir = new GameFolderDirectory(this);
                this.PathFile = new GameFolderFile(this);
            }

            private string ResolvePath(string path)
            {
                return Path.Combine(this.m_folder, path);
            }

            public override IDirectory GetCurrentDirectory()
            {
                return new Dir(this.m_folder);
            }

            private class GameFolderDirectory : GameDirectory
            {
                private readonly GameFolderPath m_path;

                public GameFolderDirectory(GameFolderPath path)
                {
                    this.m_path = path;
                }

                private string ResolvePath(string path)
                {
                    return this.m_path.ResolvePath(path);
                }

                public override void CreateDirectory(string path)
                {
                    Directory.CreateDirectory(this.ResolvePath(path));
                }

                public override void Delete(string path)
                {
                    Directory.Delete(this.ResolvePath(path));
                }

                public override void Delete(string path, bool recursive)
                {
                    Directory.Delete(this.ResolvePath(path), recursive);
                }

                public override bool Exists(string path)
                {
                    return Directory.Exists(this.ResolvePath(path));
                }

                public override IDirectoryFile[] GetFiles(string path)
                {
                    var filesPath = Directory.GetFiles(this.ResolvePath(path));
                    IDirectoryFile[] files = new IDirectoryFile[filesPath.Length];
                    for (int index = 0; index < filesPath.Length; index++)
                    {
                        files[index] = new Fi(filesPath[index]);
                    }

                    return files;
                }

                public override IDirectoryFile[] GetFiles(string path, string searchPattern)
                {
                    var filesPath = Directory.GetFiles(this.ResolvePath(path), searchPattern);
                    IDirectoryFile[] files = new IDirectoryFile[filesPath.Length];
                    for (int index = 0; index < filesPath.Length; index++)
                    {
                        files[index] = new Fi(filesPath[index]);
                    }

                    return files;
                }

                public override IDirectoryFile[] GetFiles(string path, string searchPattern, SearchOption searchOptions)
                {
                    var filesPath = Directory.GetFiles(this.ResolvePath(path), searchPattern, searchOptions);
                    IDirectoryFile[] files = new IDirectoryFile[filesPath.Length];
                    for (int index = 0; index < filesPath.Length; index++)
                    {
                        files[index] = new Fi(filesPath[index]);
                    }

                    return files;
                }

                public override IDirectory[] GetDirectories(string path)
                {
                    var directoriesPath = Directory.GetDirectories(this.ResolvePath(path));

                    IDirectory[] directories = new IDirectory[directoriesPath.Length];
                    for (int index = 0; index < directoriesPath.Length; index++)
                    {
                        directories[index] = new Dir(directoriesPath[index]);
                    }

                    return directories;
                }

                public override IDirectory GetDirectory(string path)
                {
                    return new Dir(this.ResolvePath(path));
                }

                public override IDirectoryFile GetFile(string path)
                {
                    return new Fi(this.ResolvePath(path));
                }
            }

            private class Dir : IDirectory
            {
                private readonly string m_path;

                public Dir(string path)
                {
                    this.m_path = path;
                }

                public override string ToString()
                {
                    return this.m_path;
                }

                private string ResolvePath(string path)
                {
                    return Path.Combine(this.m_path, path);
                }

                public string Name => Path.GetFileName(this.m_path);

                public void Create()
                {
                    Directory.CreateDirectory(this.m_path);
                }

                public void CreateSubDirectory(string name)
                {
                    Directory.CreateDirectory(this.ResolvePath(name));
                }

                public void Delete()
                {
                    Directory.Delete(this.m_path);
                }

                public void Delete(bool recursive)
                {
                    Directory.Delete(this.m_path, recursive);
                }

                public GamePath GetPath()
                {
                    return new GameFolderPath(this.m_path);
                }

                public IDirectory GetDirectory(string name)
                {
                    return new Dir(this.ResolvePath(name));
                }

                public IDirectoryFile GetFile(string name)
                {
                    return new Fi(this.ResolvePath(name));
                }

                public bool Exists()
                {
                    return Directory.Exists(this.m_path);
                }

                public IDirectory[] GetDirectories()
                {
                    var directoriesPath = Directory.GetDirectories(this.m_path);

                    IDirectory[] directories = new IDirectory[directoriesPath.Length];
                    for (int index = 0; index < directoriesPath.Length; index++)
                    {
                        directories[index] = new Dir(directoriesPath[index]);
                    }

                    return directories;
                }

                public IDirectoryFile[] GetFiles()
                {
                    var filesPath = Directory.GetFiles(this.m_path);

                    IDirectoryFile[] files = new IDirectoryFile[filesPath.Length];
                    for (int index = 0; index < filesPath.Length; index++)
                    {
                        files[index] = new Fi(filesPath[index]);
                    }

                    return files;
                }

                public IDirectoryFile[] GetFiles(string searchPattern)
                {
                    var filesPath = Directory.GetFiles(this.m_path, searchPattern);

                    IDirectoryFile[] files = new IDirectoryFile[filesPath.Length];
                    for (int index = 0; index < filesPath.Length; index++)
                    {
                        files[index] = new Fi(filesPath[index]);
                    }

                    return files;
                }

                public IDirectoryFile[] GetFiles(string searchPattern, SearchOption searchOptions)
                {
                    var filesPath = Directory.GetFiles(this.m_path, searchPattern, searchOptions);

                    IDirectoryFile[] files = new IDirectoryFile[filesPath.Length];
                    for (int index = 0; index < filesPath.Length; index++)
                    {
                        files[index] = new Fi(filesPath[index]);
                    }

                    return files;
                }
            }

            public class Fi : IDirectoryFile
            {
                private readonly string m_path;

                public Fi(string path)
                {
                    this.m_path = path;
                }

                public override string ToString()
                {
                    return this.m_path;
                }

                public string Name => Path.GetFileName(this.m_path);

                public string NameWithoutExtension => Path.GetFileNameWithoutExtension(this.m_path);

                public string DirectoryName => Path.GetDirectoryName(this.m_path);

                public bool Exists()
                {
                    return File.Exists(this.m_path);
                }

                public byte[] ReadAllBytes()
                {
                    return File.ReadAllBytes(this.m_path);
                }

                public Task<byte[]> ReadAllBytesAsync(CancellationToken cancellationToken = default)
                {
                    return File.ReadAllBytesAsync(this.m_path, cancellationToken);
                }

                public string[] ReadAllLines()
                {
                    return File.ReadAllLines(this.m_path);
                }

                public string[] ReadAllLines(Encoding encoding)
                {
                    return File.ReadAllLines(this.m_path, encoding);
                }

                public Task<string[]> ReadAllLinesAsync(CancellationToken cancellationToken = default)
                {
                    return File.ReadAllLinesAsync(this.m_path, cancellationToken);
                }

                public Task<string[]> ReadAllLinesAsync(Encoding encoding, CancellationToken cancellationToken = default)
                {
                    return File.ReadAllLinesAsync(this.m_path, encoding, cancellationToken);
                }

                public string ReadAllText()
                {
                    return File.ReadAllText(this.m_path);
                }

                public string ReadAllText(Encoding encoding)
                {
                    return File.ReadAllText(this.m_path, encoding);
                }

                public Task<string> ReadAllTextAsync(Encoding encoding, CancellationToken cancellationToken = default)
                {
                    return File.ReadAllTextAsync(this.m_path, encoding, cancellationToken);
                }

                public Task<string> ReadAllTextAsync(CancellationToken cancellationToken = default)
                {
                    return File.ReadAllTextAsync(this.m_path, cancellationToken);
                }

                public IEnumerable<string> ReadLines()
                {
                    return File.ReadLines(this.m_path);
                }

                public IEnumerable<string> ReadLines(Encoding encoding)
                {
                    return File.ReadLines(this.m_path, encoding);
                }

                public void WriteAllBytes(byte[] bytes)
                {
                    File.WriteAllBytes(this.m_path, bytes);
                }

                public Task WriteAllBytesAsync(byte[] bytes, CancellationToken cancellationToken = default)
                {
                    return File.WriteAllBytesAsync(this.m_path, bytes, cancellationToken);
                }

                public void WriteAllLines(IEnumerable<string> contents)
                {
                    File.WriteAllLines(this.m_path, contents);
                }

                public void WriteAllLines(IEnumerable<string> contents, Encoding encoding)
                {
                    File.WriteAllLines(this.m_path, contents, encoding);
                }

                public void WriteAllLines(string[] contents)
                {
                    File.WriteAllLines(this.m_path, contents);
                }

                public void WriteAllLines(string[] contents, Encoding encoding)
                {
                    File.WriteAllLines(this.m_path, contents, encoding);
                }

                public Task WriteAllLinesAsync(IEnumerable<string> contents, Encoding encoding, CancellationToken cancellationToken = default)
                {
                    return File.WriteAllLinesAsync(this.m_path, contents, encoding, cancellationToken);
                }

                public Task WriteAllLinesAsync(IEnumerable<string> contents, CancellationToken cancellationToken = default)
                {
                    return File.WriteAllLinesAsync(this.m_path, contents, cancellationToken);
                }

                public void WriteAllText(string contents)
                {
                    File.WriteAllText(this.m_path, contents);
                }

                public void WriteAllText(string contents, Encoding encoding)
                {
                    File.WriteAllText(this.m_path, contents, encoding);
                }

                public Task WriteAllTextAsync(string contents, Encoding encoding, CancellationToken cancellationToken = default)
                {
                    return File.WriteAllTextAsync(this.m_path, contents, encoding, cancellationToken);
                }

                public Task WriteAllTextAsync(string contents, CancellationToken cancellationToken = default)
                {
                    return File.WriteAllTextAsync(this.m_path, contents, cancellationToken);
                }
            }

            private class GameFolderFile : GameFile
            {
                private readonly GameFolderPath m_path;

                public GameFolderFile(GameFolderPath path)
                {
                    this.m_path = path;
                }

                private string ResolvePath(string path)
                {
                    return this.m_path.ResolvePath(path);
                }

                public override void Create(string path)
                {
                    using var _ = File.Create(this.ResolvePath(path));
                }

                public override void Decrypt(string path)
                {
                    File.Decrypt(this.ResolvePath(path));
                }

                public override void Delete(string path)
                {
                    File.Delete(this.ResolvePath(path));
                }

                public override void Encrypt(string path)
                {
                    File.Encrypt(this.ResolvePath(path));
                }

                public override bool Exists(string path)
                {
                    return File.Exists(this.ResolvePath(path));
                }

                public override byte[] ReadAllBytes(string path)
                {
                    return File.ReadAllBytes(this.ResolvePath(path));
                }

                public override Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
                {
                    return File.ReadAllBytesAsync(this.ResolvePath(path), cancellationToken);
                }

                public override string[] ReadAllLines(string path)
                {
                    return File.ReadAllLines(this.ResolvePath(path));
                }

                public override string[] ReadAllLines(string path, Encoding encoding)
                {
                    return File.ReadAllLines(this.ResolvePath(path), encoding);
                }

                public override Task<string[]> ReadAllLinesAsync(string path, Encoding encoding, CancellationToken cancellationToken = default)
                {
                    return File.ReadAllLinesAsync(this.ResolvePath(path), encoding, cancellationToken);
                }

                public override Task<string[]> ReadAllLinesAsync(string path, CancellationToken cancellationToken = default)
                {
                    return File.ReadAllLinesAsync(this.ResolvePath(path), cancellationToken);
                }

                public override string ReadAllText(string path)
                {
                    return File.ReadAllText(this.ResolvePath(path));
                }

                public override string ReadAllText(string path, Encoding encoding)
                {
                    return File.ReadAllText(this.ResolvePath(path), encoding);
                }

                public override Task<string> ReadAllTextAsync(string path, Encoding encoding, CancellationToken cancellationToken = default)
                {
                    return File.ReadAllTextAsync(this.ResolvePath(path), encoding, cancellationToken);
                }

                public override Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default)
                {
                    return File.ReadAllTextAsync(this.ResolvePath(path), cancellationToken);
                }

                public override IEnumerable<string> ReadLines(string path)
                {
                    return File.ReadLines(this.ResolvePath(path));
                }

                public override IEnumerable<string> ReadLines(string path, Encoding encoding)
                {
                    return File.ReadLines(this.ResolvePath(path), encoding);
                }

                public override void WriteAllBytes(string path, byte[] bytes)
                {
                    File.WriteAllBytes(this.ResolvePath(path), bytes);
                }

                public override Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default)
                {
                    return File.WriteAllBytesAsync(this.ResolvePath(path), bytes, cancellationToken);
                }

                public override void WriteAllLines(string path, IEnumerable<string> contents)
                {
                    File.WriteAllLines(this.ResolvePath(path), contents);
                }

                public override void WriteAllLines(string path, IEnumerable<string> contents, Encoding encoding)
                {
                    File.WriteAllLines(this.ResolvePath(path), contents, encoding);
                }

                public override void WriteAllLines(string path, string[] contents)
                {
                    File.WriteAllLines(this.ResolvePath(path), contents);
                }

                public override void WriteAllLines(string path, string[] contents, Encoding encoding)
                {
                    File.WriteAllLines(this.ResolvePath(path), contents, encoding);
                }

                public override Task WriteAllLinesAsync(string path, IEnumerable<string> contents, Encoding encoding, CancellationToken cancellationToken = default)
                {
                    return File.WriteAllLinesAsync(this.ResolvePath(path), contents, encoding, cancellationToken);
                }

                public override Task WriteAllLinesAsync(string path, IEnumerable<string> contents, CancellationToken cancellationToken = default)
                {
                    return File.WriteAllLinesAsync(this.ResolvePath(path), contents, cancellationToken);
                }

                public override void WriteAllText(string path, string contents)
                {
                    File.WriteAllText(this.ResolvePath(path), contents);
                }

                public override void WriteAllText(string path, string contents, Encoding encoding)
                {
                    File.WriteAllText(this.ResolvePath(path), contents, encoding);
                }

                public override Task WriteAllTextAsync(string path, string contents, Encoding encoding, CancellationToken cancellationToken = default)
                {
                    return File.WriteAllTextAsync(this.ResolvePath(path), contents, encoding, cancellationToken);
                }

                public override Task WriteAllTextAsync(string path, string contents, CancellationToken cancellationToken = default)
                {
                    return File.WriteAllTextAsync(this.ResolvePath(path), contents, cancellationToken);
                }
            }
        }
    }

    public interface IDirectory
    {
        string Name { get; }

        void Create();
        void CreateSubDirectory(string name);
        void Delete();
        void Delete(bool recursive);
        bool Exists();
        IDirectory GetDirectory(string name);
        IDirectoryFile GetFile(string name);
        IDirectoryFile[] GetFiles();
        IDirectoryFile[] GetFiles(string searchPattern);
        IDirectoryFile[] GetFiles(string searchPattern, SearchOption searchOptions);
        IDirectory[] GetDirectories();

        GamePath GetPath();
    }

    public interface IDirectoryFile
    {
        string Name { get; }
        string NameWithoutExtension { get; }
        string DirectoryName { get; }

        bool Exists();
        byte[] ReadAllBytes();
        Task<byte[]> ReadAllBytesAsync(CancellationToken cancellationToken = default);
        string[] ReadAllLines();
        string[] ReadAllLines(Encoding encoding);
        Task<string[]> ReadAllLinesAsync(CancellationToken cancellationToken = default);
        Task<string[]> ReadAllLinesAsync(Encoding encoding, CancellationToken cancellationToken = default);
        string ReadAllText();
        string ReadAllText(Encoding encoding);
        Task<string> ReadAllTextAsync(Encoding encoding, CancellationToken cancellationToken = default);
        Task<string> ReadAllTextAsync(CancellationToken cancellationToken = default);
        IEnumerable<string> ReadLines();
        IEnumerable<string> ReadLines(Encoding encoding);
        void WriteAllBytes(byte[] bytes);
        Task WriteAllBytesAsync(byte[] bytes, CancellationToken cancellationToken = default);
        void WriteAllLines(IEnumerable<string> contents);
        void WriteAllLines(IEnumerable<string> contents, Encoding encoding);
        void WriteAllLines(string[] contents);
        void WriteAllLines(string[] contents, Encoding encoding);
        Task WriteAllLinesAsync(IEnumerable<string> contents, Encoding encoding, CancellationToken cancellationToken = default);
        Task WriteAllLinesAsync(IEnumerable<string> contents, CancellationToken cancellationToken = default);
        void WriteAllText(string contents);
        void WriteAllText(string contents, Encoding encoding);
        Task WriteAllTextAsync(string contents, Encoding encoding, CancellationToken cancellationToken = default);
        Task WriteAllTextAsync(string contents, CancellationToken cancellationToken = default);
    }

    public abstract class GameDirectory
    {
        public abstract void CreateDirectory(string path);
        public abstract void Delete(string path);
        public abstract void Delete(string path, bool recursive);
        public abstract bool Exists(string path);

        public abstract IDirectory GetDirectory(string path);
        public abstract IDirectoryFile GetFile(string path);
        public abstract IDirectoryFile[] GetFiles(string path);

        public abstract IDirectoryFile[] GetFiles(string path, string searchPattern);
        public abstract IDirectoryFile[] GetFiles(string path, string searchPattern, SearchOption searchOptions);
        public abstract IDirectory[] GetDirectories(string path);
    }

    public abstract class GameFile
    {
        public abstract void Create(string path);
        public abstract void Decrypt(string path);
        public abstract void Delete(string path);
        public abstract void Encrypt(string path);
        public abstract bool Exists(string path);
        public abstract byte[] ReadAllBytes(string path);
        public abstract Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default);
        public abstract string[] ReadAllLines(string path);
        public abstract string[] ReadAllLines(string path, Encoding encoding);
        public abstract Task<string[]> ReadAllLinesAsync(string path, Encoding encoding, CancellationToken cancellationToken = default);
        public abstract Task<string[]> ReadAllLinesAsync(string path, CancellationToken cancellationToken = default);
        public abstract string ReadAllText(string path);
        public abstract string ReadAllText(string path, Encoding encoding);
        public abstract Task<string> ReadAllTextAsync(string path, Encoding encoding, CancellationToken cancellationToken = default);
        public abstract Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default);
        public abstract IEnumerable<string> ReadLines(string path);
        public abstract IEnumerable<string> ReadLines(string path, Encoding encoding);
        public abstract void WriteAllBytes(string path, byte[] bytes);
        public abstract Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default);
        public abstract void WriteAllLines(string path, IEnumerable<string> contents);
        public abstract void WriteAllLines(string path, IEnumerable<string> contents, Encoding encoding);
        public abstract void WriteAllLines(string path, string[] contents);
        public abstract void WriteAllLines(string path, string[] contents, Encoding encoding);
        public abstract Task WriteAllLinesAsync(string path, IEnumerable<string> contents, Encoding encoding, CancellationToken cancellationToken = default);
        public abstract Task WriteAllLinesAsync(string path, IEnumerable<string> contents, CancellationToken cancellationToken = default);
        public abstract void WriteAllText(string path, string contents);
        public abstract void WriteAllText(string path, string contents, Encoding encoding);
        public abstract Task WriteAllTextAsync(string path, string contents, Encoding encoding, CancellationToken cancellationToken = default);
        public abstract Task WriteAllTextAsync(string path, string contents, CancellationToken cancellationToken = default);
    }
}
