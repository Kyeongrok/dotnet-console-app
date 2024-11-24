
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        const string targetProcessName = "m2m-v0.1.0"; // 프로세스 이름 (확장자 제외)
        string logsDirectory = @"c:\logs\logs"; // 로그 파일이 있는 디렉토리
        

        Console.WriteLine($"Monitoring process: {targetProcessName}.exe");
        Console.WriteLine("Press Ctrl+C to exit.\n");

        while (true)
        {
            try
            {
                // 실행 중인 프로세스를 확인
                bool isRunning = IsProcessRunning(targetProcessName);

                // 화면 지우기
                Console.Clear();
                Console.WriteLine($"Monitoring process: {targetProcessName}.exe");
                Console.WriteLine("Press Ctrl+C to exit.\n");

                // 프로세스 상태 출력
                if (isRunning)
                {
                    Console.ForegroundColor = ConsoleColor.Green; // 초록색
                    Console.WriteLine("Process running: YES\n");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red; // 빨간색
                    Console.WriteLine("Process running: NO\n");
                }

                // 색상 초기화
                Console.ResetColor();

                // 최신 로그 파일 출력
                DisplayLatestLogFile(logsDirectory);

                // 1초 대기
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                break;
            }
        }
    }

    /// <summary>
    /// 특정 프로세스가 실행 중인지 확인하는 메서드
    /// </summary>
    /// <param name="processName">확장자를 제외한 프로세스 이름</param>
    /// <returns>실행 여부</returns>
    static bool IsProcessRunning(string processName)
    {
        // 현재 실행 중인 모든 프로세스를 가져옴
        Process[] processes = Process.GetProcessesByName(processName);
        return processes.Length > 0;
    }

    /// <summary>
    /// 가장 최신의 로그 파일을 찾아서 내용 출력
    /// </summary>
    /// <param name="directory">로그 파일이 있는 디렉토리</param>
    static void DisplayLatestLogFile(string directory)
    {
        try
        {
            // 디렉토리 내 모든 로그 파일 목록 가져오기
            var logFiles = Directory.GetFiles(directory, "*.log");

            // 가장 최신 파일 찾기
            var latestFile = logFiles
                .Select(file => new FileInfo(file))
                .OrderByDescending(file => file.LastWriteTime)
                .FirstOrDefault();

            if (latestFile != null)
            {

                // 최신 파일 내용 출력
                Console.WriteLine($"Latest log file: {latestFile.Name}");
                Console.WriteLine($"Last modified: {latestFile.LastWriteTime}\n");

                // 파일에서 마지막 20줄만 읽어서 출력
                var last20Lines = File.ReadLines(latestFile.FullName)
                                       .Reverse()
                                       .Take(20)
                                       .Reverse();

                foreach (var line in last20Lines)
                {
                    Console.WriteLine(line);
                }
            }
            else
            {
                Console.WriteLine("No log files found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading log files: {ex.Message}");
        }
    }
}
