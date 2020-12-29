using System;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using EHunter.Data;
using EHunter.Data.Pixiv;
using EHunter.DependencyInjection;
using EHunter.Pixiv.Downloader.Manual;
using EHunter.Provider.Pixiv.Services.Download;
using Meowtrix.PixivApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable CA2007

Console.Write("Storage:");
var storageSetting = new DummyStorageSetting(Console.ReadLine()!);

Console.Write("Proxy:");
string? proxy = Console.ReadLine();
var webProxy = string.IsNullOrEmpty(proxy) ? null : new WebProxy(proxy);

Console.Write("Pixiv refresh token:");
var pixivClient = new PixivClient(webProxy);
await pixivClient.LoginAsync(Console.ReadLine()!);

Console.Write("LocalDB database name:");
string connectionString = @"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Database=" + Console.ReadLine();

var sp = new ServiceCollection()
    .AddPooledDbContextFactory<EHunterDbContext>(o => o.UseSqlServer(connectionString))
    .AddPooledDbContextFactory<PixivDbContext>(o => o.UseSqlServer(connectionString))
    .BuildServiceProvider();

var eFactory = sp.GetRequiredService<IDbContextFactory<EHunterDbContext>>();
var pFactory = sp.GetRequiredService<IDbContextFactory<PixivDbContext>>();

using (var eContext = eFactory.CreateDbContext())
    await eContext.Database.MigrateAsync();
using (var pContext = pFactory.CreateDbContext())
    await pContext.Database.MigrateAsync();

var downloadService = new DownloaderService(
    pixivClient.AsServiceResolver(),
    eFactory.AsServiceResolver(),
    pFactory.AsServiceResolver(),
    storageSetting);

Console.Write("Download by id?:");
if (int.TryParse(Console.ReadLine(), out int id))
{
    var task = await downloadService.CreateDownloadTaskAsync(id);
    var tcs = new TaskCompletionSource();
    task.Progress
        .Subscribe(
            p => Console.WriteLine($"Progress: {p:P}"),
            ex => tcs.SetException(ex),
            () => tcs.SetResult());
    task.Start();
    await tcs.Task;
    return;
}
