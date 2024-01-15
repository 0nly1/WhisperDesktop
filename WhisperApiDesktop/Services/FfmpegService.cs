using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace WhisperApiDesktop.Services;

public class FfmpegService
{
    public async Task<string> CreateMp3(string filePath)
    {
        string newFilePath = filePath.Replace(' ', '_');
        string filename = $"{Path.GetFileNameWithoutExtension(newFilePath)}.mp3";
        
        string? path = Path.GetDirectoryName(filePath);

        if (string.IsNullOrEmpty(path))
            return string.Empty;
        
        string mp3Path = Path.Combine(path, filename);
        string command = $"ffmpeg -i {filePath} -b:a 64K -y {mp3Path}";

        await ExecuteCommand(command);

        return mp3Path;
    }
    
    private async Task ExecuteCommand(string command)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "/bin/bash", 
            Arguments = $"-c \"{command}\"",
            WindowStyle = ProcessWindowStyle.Normal, // ProcessWindowStyle.Hidden,
            UseShellExecute = false,
            RedirectStandardOutput = true, // false,
            CreateNoWindow = true,
        };

        using var process = new Process { StartInfo = startInfo };
        process.Start();
        await process.WaitForExitAsync();
    }
}