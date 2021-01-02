using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using EHunter.Data;
using EHunter.Data.Pixiv;
using EHunter.DependencyInjection;
using EHunter.Pixiv.Downloader.Manual;
using EHunter.Provider.Pixiv.Services;
using EHunter.Provider.Pixiv.Services.Download;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Models;
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

{
    Console.Write("Download by id?:");
    if (int.TryParse(Console.ReadLine(), out int id))
    {
        var task = await downloadService.CreateDownloadTaskAsync(id);
        await task.Start().ConsumeAsync();
        return;
    }
}

Console.Write("Seed folder:");
var folder = new DirectoryInfo(Console.ReadLine()!);

var skipped = new List<(int id, FileInfo file)>();

foreach (var f in folder.EnumerateFiles("*", SearchOption.AllDirectories))
{
    if (!int.TryParse(f.Name.Split('_')[0], out int id))
        continue;

    if (await downloadService.CanDownloadAsync(id) != true)
    {
        Console.WriteLine($"{id} already exists. Skipping.");
        continue;
    }


    Illust i;
    try
    {
        i = await pixivClient.GetIllustDetailAsync(id);
        if (i.User.Id == 0)
            throw new InvalidOperationException("Incomplete metadata.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unable to get metadata for {id}. Skipping. ({ex.Message})");
        skipped.Add((id, f));
        continue;
    }

    try
    {
        Console.WriteLine($"Downloading {id} started.");

        var task = await downloadService.CreateDownloadTaskAsync(i);
        await task.Start(f.CreationTimeUtc.ToLocalTime()).ConsumeAsync();

        Console.WriteLine($"Downloading {id} complete.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Downloading {id} failed: {ex.Message}.");
        continue;
    }
}

if (skipped.Count > 0)
{
    var oldColor = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Skipped images:");
    foreach (int i in skipped.Select(x => x.id).Distinct())
        Console.WriteLine(i);
    Console.ForegroundColor = oldColor;

    Console.Write("Add skipped to missing?:(y/n)");
    if (Console.ReadLine()!.StartsWith("y", StringComparison.OrdinalIgnoreCase))
    {
        var regex = new Regex(@"(\d+)_p(\d+).*", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        foreach (var g in skipped.GroupBy(x => x.id))
        {
            var orders = g.Select(x =>
            {
                var match = regex.Match(x.file.Name);
                string n = match.Groups[2].Value;
                return (order: int.Parse(n, NumberFormatInfo.InvariantInfo), x.file);
            }).ToArray();
            using var eContext = eFactory.CreateDbContext();
            var post = new Post
            {
                Provider = "Pixiv:Illust",
                Identifier = g.Key,
                Url = new Uri($"https://www.pixiv.net/artworks/{g.Key}")
            };

            foreach (var o in orders)
            {
                Directory.CreateDirectory(Path.Combine(storageSetting.StorageRoot!.FullName, "Pixiv", "Deleted"));

                string dest = Path.Combine("Pixiv", "Deleted", o.file.Name);
                o.file.CopyTo(Path.Combine(storageSetting.StorageRoot!.FullName, dest), overwrite: true);

                post.Images.Add(new ImageEntry(ImageType.Static, dest)
                {
                    PostOrderId = o.order
                });
                post.FavoritedTime = o.file.CreationTimeUtc.ToLocalTime();
            }

            if (post.Images.Count == 1)
            {
                post.Images[0].Tags.Add(new ImageTag("Pixiv:Meta", "Deleted"));
            }
            else
            {
                var gallery = new PostGallery
                {
                    Post = post,
                    Tags =
                    {
                        new GalleryTag("Pixiv:Meta", "Deleted")
                    }
                };
                eContext.PostGalleries.Add(gallery);
            }

            eContext.Posts.Add(post);

            await eContext.SaveChangesAsync();
            Console.WriteLine($"Added deleted {g.Key}.");
        }
    }
}
