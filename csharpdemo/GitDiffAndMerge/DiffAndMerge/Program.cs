using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace DiffAndMerge
{
    class Program
    {
        private static string _sampleText1;
        private static string _sampleText2;

        static void Main(string[] args)
        {
            InitializeSampleText();

            DiffUtil.BaseFilePath = AppDomain.CurrentDomain.BaseDirectory;
            GitUtil.GitBasePath = @"C:\Program Files (x86)\Git\cmd\";

            //Diff, generate diff patch file.
            var diffFileContent = DiffUtil.DiffText(_sampleText1, _sampleText2);
            Debug.Write(string.Format("Diff result:{0}", diffFileContent));

            //Apply Patch
            var baseFile = DiffUtil.SaveTextToFile(_sampleText1);
            var modifiedFile = DiffUtil.SaveTextToFile(_sampleText2);
            var result = DiffUtil.ApplyPatch(baseFile, modifiedFile);
            Debug.Write(string.Format("Apply patch result:{0}", result));

            //Diff
            var mergedText = DiffUtil.MergeText(_sampleText1, _sampleText2);
            Debug.Write(string.Format("Merged text:{0}", mergedText));
        }

        private static void InitializeSampleText()
        {
            var sampleFile1 = "DiffAndMerge.SampleTextFiles.file1.txt";
            var sampleFile2 = "DiffAndMerge.SampleTextFiles.file2.txt";
            var assembly = Assembly.GetExecutingAssembly();

            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                if (resourceName == sampleFile1)
                {
                    using (var reader = new StreamReader(assembly.GetManifestResourceStream(resourceName)))
                    {
                        _sampleText1 = reader.ReadToEnd();
                        reader.Close();
                    }
                }
                else if (resourceName == sampleFile2)
                {
                    using (var reader = new StreamReader(assembly.GetManifestResourceStream(resourceName)))
                    {
                        _sampleText2 = reader.ReadToEnd();
                        reader.Close();
                    }
                }
            }
        }
    }

    public class DiffUtil
    {
        /// <summary>
        /// 存放临时文件的根目录
        /// </summary>
        public static string BaseFilePath { get; set; }

        /// <summary>
        /// 比较给定的两串文本，返回比较结果。
        /// <remarks>
        /// 比较算法采用git diff命令实现。
        /// </remarks>
        /// </summary>
        /// <param name="text1"></param>
        /// <param name="text2"></param>
        public static string DiffText(string text1, string text2)
        {
            var file1 = SaveTextToFile(text1);
            var file2 = SaveTextToFile(text2);
            var resultFileName = GenerateUniqueTextFileName();
            var resultFile = string.Format("{0}{1}", BaseFilePath, resultFileName);
            var resultCode = GitUtil.DiffFile(file1, file2, resultFile);
            return GetFileContent(resultFileName, false);
        }
        /// <summary>
        /// 比较给定的两个文件，生成patch文件，并应用Patch文件
        /// <remarks>
        /// Patch采用git apply命令实现。
        /// </remarks>
        /// </summary>
        /// <param name="baseText"></param>
        /// <param name="modifiedText"></param>
        public static int ApplyPatch(string baseFile, string modifiedFile)
        {
            var resultFileName = GenerateUniqueTextFileName();
            var patchFile = string.Format("{0}{1}", BaseFilePath, resultFileName);
            GitUtil.DiffFile(baseFile, modifiedFile, patchFile);
            return GitUtil.ApplyPatch(patchFile);
        }
        /// <summary>
        /// Merge给定的两串文本，返回Merge结果。
        /// <remarks>
        /// Merge采用git merge-file命令实现。
        /// </remarks>
        /// </summary>
        /// <param name="text1"></param>
        /// <param name="text2"></param>
        public static string MergeText(string text1, string text2)
        {
            var baseFile = SaveTextToFile(text1);
            var currentFile = SaveTextToFile(text1);
            var otherFile = SaveTextToFile(text2);
            var resultFileName = GenerateUniqueTextFileName();
            var outputFile = string.Format("{0}{1}", BaseFilePath, resultFileName);
            var resultCode = GitUtil.MergeFile(baseFile, currentFile, otherFile, outputFile);
            return GetFileContent(resultFileName, false);
        }

        /// <summary>
        /// 将文本保存到文件，返回保存后的文件地址。
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string SaveTextToFile(string text)
        {
            var fileName = GenerateUniqueTextFileName();
            var filePath = string.Format("{0}{1}", BaseFilePath, fileName);
            using (var writer = new StreamWriter(filePath))
            {
                writer.Write(text);
                writer.Close();
            }
            return filePath;
        }
        /// <summary>
        /// 读取并返回指定文件的内容。
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="formatConent">表示是否需要将文件中的空格和换行符替换为html标签，默认值为true</param>
        /// <returns></returns>
        public static string GetFileContent(string fileRelativePath, bool formatConent = true)
        {
            string content;
            string filePath = string.Format("{0}\\{1}", BaseFilePath, fileRelativePath);

            using (var reader = new StreamReader(filePath, Encoding.UTF8))
            {
                content = reader.ReadToEnd();
                reader.Close();
            }
            if (formatConent && !string.IsNullOrEmpty(content))
            {
                content = content.Replace(" ", "&nbsp;");
                content = content.Replace("\n", "<br/>");
            }

            return content;
        }
        /// <summary>
        /// 产生一个唯一的文件名
        /// </summary>
        /// <returns></returns>
        public static string GenerateUniqueTextFileName()
        {
            return string.Format("{0}.txt", Guid.NewGuid().ToString());
        }
    }
    public class GitUtil
    {
        public static string GitBasePath { get; set; }

        /// <summary>
        /// Diff two files, output the result in the outputFile.
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <param name="outputFile"></param>
        /// <returns>Returns 1 means has difference, otherwise 0.</returns>
        public static int DiffFile(string file1, string file2, string outputFile)
        {
            var arguments = string.Format("git diff {0} {1} > {2}", file1, file2, outputFile);
            return RunCmd(arguments);
        }
        /// <summary>
        /// Apply patch
        /// </summary>
        /// <param name="patchFile"></param>
        /// <returns></returns>
        public static int ApplyPatch(string patchFile)
        {
            var arguments = string.Format("git apply {0}", patchFile);
            return RunCmd(arguments);
        }
        /// <summary>
        /// Merge two files based on a base file, output the result in the outputFile.
        /// Incorporates all changes that lead from the base-file to other-file into current-file,
        /// and output the merge result to a seperate output file.
        /// </summary>
        /// <param name="baseFile"></param>
        /// <param name="currentFile"></param>
        /// <param name="otherFile"></param>
        /// <param name="outputFile"></param>
        /// <returns></returns>
        public static int MergeFile(string baseFile, string currentFile, string otherFile, string outputFile)
        {
            var arguments = string.Format("git merge-file --stdout {0} {1} {2} > {3}", currentFile, baseFile, otherFile, outputFile);
            return RunCmd(arguments);
        }

        /// <summary>
        /// Start a cmd command with the given arguments.
        /// </summary>
        /// <param name="arguments"></param>
        private static int RunCmd(string arguments)
        {
            int exitCode = 0;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.WorkingDirectory = GitBasePath;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardInput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            Process process = new Process();
            process.StartInfo = startInfo;
            process.EnableRaisingEvents = true;
            process.Exited += (sender, e) =>
            {
                exitCode = (sender as Process).ExitCode;
            };
            process.Start();
            process.StandardInput.WriteLine(arguments);
            process.StandardInput.WriteLine("exit");
            process.WaitForExit();

            return exitCode;
        }
    }
}
