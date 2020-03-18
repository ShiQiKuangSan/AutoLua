using System;
using System.Linq;
using Java.IO;

namespace AutoLua.Core.LuaScript.Api
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class LuaFiles
    {
        /// <summary>
        /// 返回路径path是否是文件。
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>是否是文件</returns>
        public bool isFile(string path)
        {
            return PFiles.IsFile(path);
        }

        /// <summary>
        /// 返回路径path是否是文件夹。
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>是否是文件夹</returns>
        public bool isDir(string path)
        {
            return PFiles.IsDir(path);
        }

        /// <summary>
        /// 返回文件夹path是否为空文件夹。如果该路径并非文件夹，则直接返回false。
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public bool isEmptyDir(string path)
        {
            var file = new File(path);
            return file.IsDirectory && file.List().Length == 0;
        }

        /// <summary>
        /// 连接两个路径并返回，例如files.join("/sdcard/", "1.txt")返回"/sdcard/1.txt"。
        /// </summary>
        /// <param name="path">父目录路径</param>
        /// <param name="paths">子路径</param>
        /// <returns></returns>
        public string join(string path, params string[] paths)
        {
            var file = new File(path);
            
            file = paths.Aggregate(file, (current, item) => new File(current, path));

            return file.Path;
        }

        /// <summary>
        /// 创建一个文件或文件夹并返回是否创建成功。如果文件已经存在，则直接返回false。
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public bool create(string path)
        {
            var p = this.path(path);
            return PFiles.Create(p);
        }

        /// <summary>
        /// 创建一个文件或文件夹并返回是否创建成功。如果文件所在文件夹不存在，则先创建他所在的一系列文件夹。如果文件已经存在，则直接返回false。
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public bool createWithDirs(string path)
        {
            var p = this.path(path);
            return PFiles.CreateWithDirs(p);
        }

        /// <summary>
        /// 返回在路径path处的文件是否存在。
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public bool exists(string path)
        {
            var p = this.path(path);
            return PFiles.Exists(p);
        }

        /// <summary>
        /// 确保路径path所在的文件夹存在。如果该路径所在文件夹不存在，则创建该文件夹。
        /// 例如对于路径"/sdcard/Download/ABC/1.txt"，如果/Download/文件夹不存在，则会先创建Download，再创建ABC文件夹。
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public bool ensureDir(string path)
        {
            var p = this.path(path);
            return PFiles.EnsureDir(p);
        }

        /// <summary>
        /// 读取文本文件path的所有内容并返回。如果文件不存在，则抛出FileNotFoundException。
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="encoding">字符编码，可选，默认为utf-8</param>
        /// <returns></returns>
        public string read(string path, string encoding = "")
        {
            var p = this.path(path);

            return string.IsNullOrWhiteSpace(encoding) ? PFiles.Read(p) : PFiles.Read(p, encoding);
        }

        /// <summary>
        /// 读取文件path的所有内容并返回一个字节数组。如果文件不存在，则抛出FileNotFoundException。
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public byte[] readBytes(string path)
        {
            var p = this.path(path);

            return PFiles.ReadBytes(p);
        }

        /// <summary>
        /// 把text写入到文件path中。如果文件存在则覆盖，不存在则创建。
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="text">要写入的文本内容</param>
        /// <param name="encoding">字符编码</param>
        public void write(string path, string text, string encoding = "")
        {
            var p = this.path(path);

            if (string.IsNullOrWhiteSpace(encoding))
                PFiles.Write(p, text);
            else
                PFiles.Write(p, text, encoding);
        }

        /// <summary>
        /// 把bytes写入到文件path中。如果文件存在则覆盖，不存在则创建。
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="bytes">字节数组，要写入的二进制数据</param>
        public void writeBytes(string path, byte[] bytes)
        {
            var p = this.path(path);

            PFiles.WriteBytes(p, bytes);
        }

        /// <summary>
        /// 把text追加到文件path的末尾。如果文件不存在则创建。
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="text">要写入的文本内容</param>
        /// <param name="encoding">字符编码</param>
        public void append(string path, string text, string encoding)
        {
            var p = this.path(path);

            if (string.IsNullOrWhiteSpace(encoding))
                PFiles.Append(p, text);
            else
                PFiles.Append(p, text, encoding);
        }

        /// <summary>
        /// 把bytes追加到文件path的末尾。如果文件不存在则创建。
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="bytes">字节数组，要写入的二进制数据</param>
        public void appendBytes(string path, byte[] bytes)
        {
            var p = this.path(path);

            PFiles.AppendBytes(p, bytes);
        }

        /// <summary>
        /// 复制文件，返回是否复制成功。例如files.copy("/sdcard/1.txt", "/sdcard/Download/1.txt")。
        /// </summary>
        /// <param name="pathFrom">要复制的原文件路径</param>
        /// <param name="pathTo">复制到的文件路径</param>
        /// <returns></returns>
        public bool copy(string pathFrom, string pathTo)
        {
            return PFiles.Copy(path(pathFrom), path(pathTo));
        }

        /// <summary>
        /// 移动文件，返回是否移动成功。例如files.move("/sdcard/1.txt", "/sdcard/Download/1.txt")会把1.txt文件从sd卡根目录移动到Download文件夹。
        /// </summary>
        /// <param name="pathFrom">要移动的原文件路径</param>
        /// <param name="newPath">移动到的文件路径</param>
        /// <returns></returns>
        public bool move(string pathFrom, string newPath)
        {
            return PFiles.Move(path(pathFrom), newPath);
        }

        /// <summary>
        /// 重命名文件，并返回是否重命名成功。例如files.rename("/sdcard/1.txt", "2.txt")。
        /// </summary>
        /// <param name="pathFrom">要重命名的原文件路径</param>
        /// <param name="newName">要重命名的新文件名</param>
        /// <returns></returns>
        public bool rename(string pathFrom, string newName)
        {
            return PFiles.Rename(path(pathFrom), newName);
        }

        /// <summary>
        /// 重命名文件，不包含拓展名，并返回是否重命名成功。例如files.rename("/sdcard/1.txt", "2")会把"1.txt"重命名为"2.txt"。
        /// </summary>
        /// <param name="pathFrom">要重命名的原文件路径</param>
        /// <param name="newName">要重命名的新文件名</param>
        /// <returns></returns>
        public bool renameWithoutExtension(string pathFrom, string newName)
        {
            return PFiles.RenameWithoutExtension(path(pathFrom), newName);
        }

        /// <summary>
        /// 返回文件的文件名。例如files.getName("/sdcard/1.txt")返回"1.txt"。
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <returns></returns>
        public string getName(string filePath)
        {
            return PFiles.GetName(filePath);
        }

        /// <summary>
        /// 返回不含拓展名的文件的文件名。例如files.getName("/sdcard/1.txt")返回"1"。
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <returns></returns>
        public string getNameWithoutExtension(string filePath)
        {
            return PFiles.GetNameWithoutExtension(filePath);
        }

        /// <summary>
        /// 返回文件的拓展名。例如files.getExtension("/sdcard/1.txt")返回"txt"。
        /// </summary>
        /// <param name="fileName">路径</param>
        /// <returns></returns>
        public string getExtension(string fileName)
        {
            return PFiles.GetExtension(fileName);
        }

        /// <summary>
        /// 删除文件或空文件夹，返回是否删除成功。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool remove(string path)
        {
            return PFiles.Remove(path);
        }

        /// <summary>
        /// 删除文件夹，如果文件夹不为空，则删除该文件夹的所有内容再删除该文件夹，返回是否全部删除成功。
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public bool removeDir(string path)
        {
            return PFiles.RemoveDir(this.path(path));
        }

        /// <summary>
        /// 返回SD卡路径。所谓SD卡，即外部存储器。
        /// </summary>
        /// <returns></returns>
        public string getSdcardPath()
        {
            return PFiles.GetSdcardPath();
        }

        /// <summary>
        /// 列出文件夹path下的满足条件的文件和文件夹的名称的数组。如果不加filter参数，则返回所有文件和文件夹。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string[] listDir(string path)
        {
            return PFiles.ListDir(this.path(path));
        }

        /// <summary>
        /// 返回相对路径对应的绝对路径。例如files.path("./1.png")，如果运行这个语句的脚本位于文件夹"/sdcard/脚本/"中，则返回"/sdcard/脚本/1.png"。
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <returns></returns>
        public string path(string relativePath)
        {
            return PFiles.Path(relativePath);
        }

        public class PFiles
        {
            /// <summary>
            /// 返回相对路径对应的绝对路径。例如files.path("./1.png")，如果运行这个语句的脚本位于文件夹"/sdcard/脚本/"中，则返回"/sdcard/脚本/1.png"。
            /// </summary>
            /// <param name="relativePath">相对路径</param>
            /// <returns></returns>
            public static string Path(string relativePath)
            {
                var basePath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/scriptlua/";

                var f = new File(basePath);
                var paths = relativePath.Split('/');

                foreach (var path in paths)
                {
                    if (path.Equals("."))
                        continue;
                    if (path.Equals(".."))
                    {
                        f = f.ParentFile;
                        continue;
                    }

                    f = new File(f, path);
                }

                return relativePath.EndsWith(File.Separator) ? f.Path + "/" : f.Path;
            }

            /// <summary>
            /// 返回路径path是否是文件。
            /// </summary>
            /// <param name="path">路径</param>
            /// <returns>是否是文件</returns>
            public static bool IsFile(string path)
            {
                return new File(path).IsFile;
            }

            /// <summary>
            /// 返回路径path是否是文件夹。
            /// </summary>
            /// <param name="path">路径</param>
            /// <returns>是否是文件夹</returns>
            public static bool IsDir(string path)
            {
                return new File(path).IsDirectory;
            }

            /// <summary>
            /// 返回文件夹path是否为空文件夹。如果该路径并非文件夹，则直接返回false。
            /// </summary>
            /// <param name="path">路径</param>
            /// <returns></returns>
            public static bool IsEmptyDir(string path)
            {
                var file = new File(path);
                return file.IsDirectory && file.List().Length == 0;
            }

            /// <summary>
            /// 连接两个路径并返回，例如files.join("/sdcard/", "1.txt")返回"/sdcard/1.txt"。
            /// </summary>
            /// <param name="path">父目录路径</param>
            /// <param name="paths">子路径</param>
            /// <returns></returns>
            public static string Join(string path, params string[] paths)
            {
                var file = new File(path);
                file = paths.Aggregate(file, (current, item) => new File(current, path));

                return file.Path;
            }

            /// <summary>
            /// 创建一个文件或文件夹并返回是否创建成功。如果文件已经存在，则直接返回false。
            /// </summary>
            /// <param name="path">路径</param>
            /// <returns></returns>
            public static bool Create(string path)
            {
                var f = new File(path);

                if (path.EndsWith(File.Separator))
                {
                    return f.Mkdir();
                }
                else
                {
                    try
                    {
                        return f.CreateNewFile();
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }

            /// <summary>
            /// 创建一个文件或文件夹并返回是否创建成功。如果文件所在文件夹不存在，则先创建他所在的一系列文件夹。如果文件已经存在，则直接返回false。
            /// </summary>
            /// <param name="path">路径</param>
            /// <returns></returns>
            public static bool CreateWithDirs(string path)
            {
                return CreateIfNotExists(path);
            }

            /// <summary>
            /// 返回在路径path处的文件是否存在。
            /// </summary>
            /// <param name="path">路径</param>
            /// <returns></returns>
            public static bool Exists(string path)
            {
                return new File(path).Exists();
            }

            /// <summary>
            /// 如果不存在则创建文件
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            private static bool CreateIfNotExists(string path)
            {
                EnsureDir(path);
                var file = new File(path);

                if (file.Exists()) 
                    return false;
                
                try
                {
                    return file.CreateNewFile();
                }
                catch (IOException e)
                {
                    e.PrintStackTrace();
                }
                return false;
            }

            /// <summary>
            /// 确保是文件夹
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public static bool EnsureDir(string path)
            {
                var i = path.LastIndexOf("\\", StringComparison.Ordinal);
                
                if (i < 0)
                    i = path.LastIndexOf("/", StringComparison.Ordinal);
                
                if (i >= 0)
                {
                    var folder = path.Substring(0, i);
                    var file = new File(folder);
                    return file.Exists() || file.Mkdirs();
                }
                else
                {
                    return false;
                }
            }

            #region 文件读取

            public static string Read(string path, string encoding)
            {
                return Read(new File(path), encoding);
            }

            public static string Read(string path)
            {
                return Read(new File(path));
            }

            private static string Read(File file, string encoding = "utf-8")
            {
                try
                {
                    return Read(new FileInputStream(file), encoding);
                }
                catch (FileNotFoundException e)
                {
                    throw new UncheckedIOException(e);
                }
            }

            private static string Read(InputStream inputStream, string encoding)
            {
                try
                {
                    var bytes = new byte[inputStream.Available()];
                    inputStream.Read(bytes);
                    return new Java.Lang.String(bytes, encoding).ToString();
                }
                catch (IOException e)
                {
                    throw new UncheckedIOException(e);
                }
                finally
                {
                    CloseSilently(inputStream);
                }
            }

            public static string Read(InputStream inputStream)
            {
                return Read(inputStream, "utf-8");
            }

            public static byte[] ReadBytes(string path)
            {
                try
                {
                    return ReadBytes(new FileInputStream(path));
                }
                catch (FileNotFoundException e)
                {
                    throw new UncheckedIOException(e);
                }
            }

            private static byte[] ReadBytes(InputStream inputStream)
            {
                try
                {
                    var bytes = new byte[inputStream.Available()];
                    inputStream.Read(bytes);
                    return bytes;
                }
                catch (IOException e)
                {
                    throw new UncheckedIOException(e);
                }
            }

            #endregion

            #region 文件写入

            private const int DefaultBufferSize = 8192;

            public static void Write(string path, string text)
            {
                Write(new File(path), text);
            }

            public static void Write(string path, string text, string encoding)
            {
                try
                {
                    Write(new FileOutputStream(path), new Java.Lang.String(text), encoding);
                }
                catch (FileNotFoundException e)
                {
                    throw new UncheckedIOException(e);
                }
            }

            private static void Write(File file, string text)
            {
                try
                {
                    Write(new FileOutputStream(file), text);
                }
                catch (FileNotFoundException e)
                {
                    throw new UncheckedIOException(e);
                }
            }

            private static void Write(OutputStream fileOutputStream, string text)
            {
                Write(fileOutputStream, new Java.Lang.String(text), "utf-8");
            }

            private static void Write(OutputStream outputStream, Java.Lang.String text, string encoding)
            {
                try
                {
                    outputStream.Write(text.GetBytes(encoding));
                }
                catch (IOException e)
                {
                    throw new UncheckedIOException(e);
                }
                finally
                {
                    CloseSilently(outputStream);
                }
            }

            private static void Write(InputStream inputStream, OutputStream os, bool close = true)
            {
                var buffer = new byte[DefaultBufferSize];
                try
                {
                    while (inputStream.Available() > 0)
                    {
                        var n = inputStream.Read(buffer);
                        if (n > 0)
                        {
                            os.Write(buffer, 0, n);
                        }
                    }

                    if (!close) return;
                    
                    inputStream.Close();
                    os.Close();
                }
                catch (IOException e)
                {
                    throw new UncheckedIOException(e);
                }
            }

            public static void WriteBytes(string path, byte[] bytes)
            {
                try
                {
                    WriteBytes(new FileOutputStream(path), bytes);
                }
                catch (FileNotFoundException e)
                {
                    throw new UncheckedIOException(e);
                }
            }

            private static void WriteBytes(OutputStream outputStream, byte[] bytes)
            {
                try
                {
                    outputStream.Write(bytes);
                    outputStream.Close();
                }
                catch (IOException e)
                {
                    throw new UncheckedIOException(e);
                }
            }
            #endregion

            #region 文件追加

            public static void Append(string path, string text)
            {
                Create(path);
                try
                {
                    Write(new FileOutputStream(path, true), text);
                }
                catch (FileNotFoundException e)
                {
                    throw new UncheckedIOException(e);
                }
            }

            public static void Append(string path, string text, string encoding)
            {
                Create(path);
                try
                {
                    Write(new FileOutputStream(path, true), new Java.Lang.String(text), encoding);
                }
                catch (FileNotFoundException e)
                {
                    throw new UncheckedIOException(e);
                }
            }

            public static void AppendBytes(string path, byte[] bytes)
            {
                Create(path);
                try
                {
                    WriteBytes(new FileOutputStream(path, true), bytes);
                }
                catch (IOException e)
                {
                    throw new UncheckedIOException(e);
                }
            }

            #endregion

            #region 复制

            public static bool Copy(string pathFrom, string pathTo)
            {
                try
                {
                    return CopyStream(new FileInputStream(pathFrom), pathTo);
                }
                catch (FileNotFoundException e)
                {
                    e.PrintStackTrace();
                    return false;
                }
            }

            private static bool CopyStream(InputStream inputStream, string path)
            {
                if (!EnsureDir(path))
                    return false;

                var file = new File(path);

                try
                {
                    if (!file.Exists())
                        if (!file.CreateNewFile())
                            return false;

                    var fos = new FileOutputStream(file);

                    Write(inputStream, fos);
                    return true;
                }
                catch (IOException e)
                {
                    e.PrintStackTrace();
                    return false;
                }
            }

            #endregion

            public static bool Move(string path, string newPath)
            {
                var f = new File(path);
                return f.RenameTo(new File(newPath));
            }

            public static bool Rename(string path, string newName)
            {
                var f = new File(path);
                return f.RenameTo(new File(f.Parent, newName));
            }

            public static bool RenameWithoutExtension(string path, string newName)
            {
                var file = new File(path);
                var newFile = new File(file.Parent, newName + "." + GetExtension(file.Name));

                return file.RenameTo(newFile);
            }

            public static string GetName(string filePath)
            {
                filePath = filePath.Replace('\\', '/');
                return new File(filePath).Name;
            }

            public static string GetExtension(string fileName)
            {
                var i = fileName.LastIndexOf('.');
                if (i < 0 || i + 1 >= fileName.Length - 1)
                    return "";
                return fileName.Substring(i + 1);
            }

            public static string GetNameWithoutExtension(string filePath)
            {
                var fileName = GetName(filePath);
                var b = fileName.LastIndexOf('.');
                if (b < 0)
                    b = fileName.Length;

                fileName = fileName.Substring(0, b);

                return fileName;
            }

            public static bool Remove(string path)
            {
                return new File(path).Delete();
            }

            public static bool RemoveDir(string path)
            {
                return DeleteRecursively(new File(path));
            }

            private static bool DeleteRecursively(File file)
            {
                if (file.IsFile)
                    return file.Delete();

                var children = file.ListFiles();

                if (children == null) 
                    return file.Delete();
                
                return children.All(DeleteRecursively) && file.Delete();
            }

            public static string GetSdcardPath()
            {
                return Android.OS.Environment.ExternalStorageDirectory.Path;
            }

            public static string[] ListDir(string path)
            {
                var file = new File(path);
                return WrapNonNull(file.List());
            }

            private static void CloseSilently(ICloseable closeable)
            {
                if (closeable == null)
                {
                    return;
                }
                try
                {
                    closeable.Close();
                }
                catch (IOException)
                {

                }
            }


            private static string[] WrapNonNull(string[] list)
            {
                return list ?? new string[0];
            }
        }
    }
}