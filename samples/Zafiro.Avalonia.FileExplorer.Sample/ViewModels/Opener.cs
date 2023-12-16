using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Zafiro.Avalonia.FileExplorer.Items;
using Zafiro.IO;
using Process = System.Diagnostics.Process;
using Result = CSharpFunctionalExtensions.Result;

#if ANDROID
using AndroidX.Core.Content;
using Android.App;
using Android.Content;
using Android.Webkit;
#endif

namespace Zafiro.Avalonia.FileExplorer.Sample.ViewModels;

public class Opener : ISystemOpen
{
    public Task<Result> Open(IObservable<byte> fileContents, string fileName)
    {
        return Result.Try(async () =>
        {
            var tempFileName = Path.Combine(Path.GetTempPath(), fileName);
            await using (var fileStream = File.Create(tempFileName))
            {
                await fileContents.DumpTo(fileStream);
            }

#if ANDROID
            OpenAndroid(tempFileName);
#else
            OpenDesktop(tempFileName);
#endif
        });
    }

    private static void OpenDesktop(string tempFileName)
    {
        Process.Start(new ProcessStartInfo()
        {
            FileName = tempFileName,
            UseShellExecute = true,
        });
    }

#if ANDROID
    static void OpenAndroid(string documentPath)
    {
        Java.IO.File file = new Java.IO.File(documentPath);
        var uri = FileProvider.GetUriForFile(Application.Context, Application.Context.PackageName + ".fileprovider", file);

        Intent intent = new Intent(Intent.ActionView);
        intent.SetDataAndType(uri, GetMimeType(uri));
        intent.SetDataAndTypeAndNormalize(uri, GetMimeType(uri));
        intent.SetFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission | ActivityFlags.NewTask);
        
        Application.Context.StartActivity(intent);
    }

    public static string GetMimeType(Android.Net.Uri uri) {           
        String mimeType = null;
        if (ContentResolver.SchemeContent.Equals(uri.Scheme)) {
            ContentResolver cr = Application.Context.ContentResolver;
            mimeType = cr.GetType(uri);
        } else {
            String fileExtension = MimeTypeMap.GetFileExtensionFromUrl(uri.ToString());
            mimeType = MimeTypeMap.Singleton.GetMimeTypeFromExtension(fileExtension.ToLower());
        }
        return mimeType;
    }
#endif
}