using System;
using System.IO;
using System.Threading;

namespace MTM_RE_160.Classes
{
    class CopyFiles
    {

        public string TargetPath { get; set; }
        public string SourcePath { get; set; }
        public string DirectoryName { get; set; }

        public Action<string> EVENTSTRING;
        public Action<int,int> EVENTCOUNT;
        public Action EVENTCOMPLETE;
        public Action EVENTERROR;
        public CopyFiles(string source, string target, string directoryName) 
        {
            this.SourcePath = source;
            
            this.DirectoryName = directoryName;
            this.TargetPath = target;
        }

        public void Start()
        {
            Thread thread = new Thread(StartCopy);
            thread.Start();
                
        }

        private void StartCopy()
        {

            EVENTSTRING?.Invoke(string.Format("Проверяем доступ к директории: {0}\r\n", TargetPath));

            if (!Directory.Exists(TargetPath)) 
            {

                try {
                    Directory.CreateDirectory(TargetPath);
                } catch {
                    EVENTSTRING?.Invoke(string.Format("Директория: {0} недоступна\r\n", TargetPath));
                    EVENTERROR?.Invoke();
                    return; }              
            }
            EVENTSTRING?.Invoke(string.Format("Директория: {0} доступна\r\n", TargetPath));

            string targetPath = Path.Combine(TargetPath, DirectoryName);
            string sourcePath = Path.Combine(SourcePath, DirectoryName);            

            if (!Directory.Exists(targetPath))
            {
                EVENTSTRING?.Invoke(string.Format("Создаем директорию: {0}\r\n", targetPath));
                Directory.CreateDirectory(targetPath);
            }

            int count = 0;
            if (Directory.Exists(sourcePath))
            {
                EVENTSTRING?.Invoke(string.Format("Подготавливаем файлы для копирования\r\n"));
                string[] files = System.IO.Directory.GetFiles(sourcePath);
                EVENTCOUNT?.Invoke(count, files.Length);
                foreach (string file in files)
                {

                    string fileName = System.IO.Path.GetFileName(file);
                    string destFile = System.IO.Path.Combine(targetPath, fileName);
                    EVENTSTRING?.Invoke(string.Format("Копируем {0} \r\n", file.Substring(file.LastIndexOf('\\') + 1)));
                    if (!File.Exists(destFile))
                        File.Copy(file, destFile, true);
                    EVENTCOUNT?.Invoke(count < files.Length ? ++count : count, files.Length);
                }
            }
            else
            {
                EVENTSTRING?.Invoke(string.Format("Отсутствует источник копирования\r\n"));
                EVENTERROR?.Invoke();
                return;
            }
            EVENTSTRING?.Invoke(string.Format("Копирование файлов завершенно.\r\n"));
            EVENTCOMPLETE?.Invoke();
        }
    }
}
