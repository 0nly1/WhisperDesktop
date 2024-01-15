using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using WhisperApiDesktop.Services;

namespace WhisperApiDesktop.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    internal string[] WhisperFormats =
    {
        "text", "srt", "json", "verbose_json", "vtt"
    };
    
    public string ApiKey { get; set; } = string.Empty;
    public double Temperature { get; set; } = 0;
    public string Prompt { get; set; } = string.Empty;
    public string LangCode { get; set; } = "en";
    public string Transcription { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;

    public bool IsText { get; set; } = true;
    public bool IsSrt { get; set; } = false;
    public bool IsJson { get; set; } = false;
    public bool IsVerboseJson { get; set; } = false;
    public bool IsVtt { get; set; } = false;

    public async Task OpenFile(Window window)
    {
        var dialog = new OpenFileDialog();
        dialog.Filters = new List<FileDialogFilter>()
        {
            new FileDialogFilter() { Name = "Audio Files", Extensions = { "mp3", "wav", "m4a" } },
            new FileDialogFilter() { Name = "Video files", Extensions = { "mp4", "mpeg", "mpga" } },
            new FileDialogFilter() { Name = "All Files", Extensions = { "*" } }
        };
        var result = await dialog.ShowAsync(window);

        if (result == null)
        {
            Console.WriteLine("Null");
            return;
        }
        
        var file = result.First();

        // TODO: do something with it 
        // if (Path.GetExtension(file).ToLower() == ".mov")
        // {
        //     Transcription = "Конвертирую в mp3...";
        //     OnPropertyChanged(nameof(Transcription));
        //
        //     var ffmpeg = new FfmpegService();
        //     file = await ffmpeg.CreateMp3(file);
        // }
        
        Console.WriteLine(file);
        FilePath = file;
        OnPropertyChanged(nameof(FilePath));
    }

    public async Task GetTranscriptionExecute()
    {
        string file = FilePath;
        
        if (string.IsNullOrEmpty(file))
        {
            Transcription = "Ошибка! Укажите путь до файла";
            OnPropertyChanged(nameof(Transcription));
            return;
        }
        
        var whisper = new WhisperService(ApiKey);
        
        try
        {
            // TODO: do something with it
            // if (Path.GetExtension(file).ToLower() == ".mov")
            // {
            //     Transcription = "Конвертирую в mp3...";
            //     OnPropertyChanged(nameof(Transcription));
            //
            //     var ffmpeg = new FfmpegService();
            //     file = await ffmpeg.CreateMp3(file);
            // }
        
            Transcription = "Отправляю запрос...";
            OnPropertyChanged(nameof(Transcription));
            
            var responseFormat = GetWhisperFormat();
            Console.WriteLine("File path: {0}\nResponse Format: {1}\nLang Code: {2}\nTemperature: {3}\nPrompt: {4}", 
                file, responseFormat, LangCode, Temperature, Prompt);

            var response = await whisper.GetTranscriptions(file, Prompt, responseFormat, LangCode, 
                Temperature);
            Console.WriteLine("Finished the request");
            
            Transcription = response;
            OnPropertyChanged(nameof(Transcription));
        
            // if (Path.GetExtension(file).ToLower() == ".mp3")
            //     File.Delete(file);
        }
        catch (Exception e)
        {
            Transcription = e.ToString();
            OnPropertyChanged(nameof(Transcription));
        }
        
    }

    public async Task CopyToClipboard()
    {
        if (string.IsNullOrEmpty(Transcription))
            return;
        
        await Application.Current?.Clipboard?.SetTextAsync(Transcription)!;
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    private string GetWhisperFormat()
    {
        if (IsText)
            return "text";
        if (IsSrt)
            return "srt";
        if (IsJson)
            return "json";
        if (IsVerboseJson)
            return "verbose_json";
        if (IsVtt)
            return "vtt";

        return string.Empty;
    }
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

public static class SelectedTest
{
    public static bool Text;
    public static bool Srt;
    public static bool Json;
}