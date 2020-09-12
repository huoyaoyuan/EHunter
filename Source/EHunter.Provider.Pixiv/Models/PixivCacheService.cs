using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using EHunter.Settings;

namespace EHunter.Provider.Pixiv.Models
{
    public class PixivCacheService
    {
        private readonly ICommonSetting _commonSetting;

        public PixivCacheService(ICommonSetting commonSetting) => _commonSetting = commonSetting;

        public Task<Uri> GetLocalFileAsync(HttpResponseMessage response)
            => Task.Run(async () =>
            {
                string root = _commonSetting.StorageRoot;
                string localPath = response.RequestMessage!.RequestUri!.LocalPath;
                Debug.Assert(localPath.StartsWith('/'));
                string fullPath = Path.Combine(root, localPath.Substring(1));
                if (!File.Exists(fullPath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
                    using var fileStream = new FileStream(fullPath,
                        FileMode.CreateNew, FileAccess.Write, FileShare.None,
                        4096, FileOptions.Asynchronous);

                    var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    await responseStream.CopyToAsync(fileStream).ConfigureAwait(false);
                }

                return new Uri(fullPath);
            });
    }
}
