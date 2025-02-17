using System.Collections.Generic;
using tvscheduler.Models;

namespace tvscheduler.Utilities;

public class DatabaseChannelsInit
{
    private readonly List<Channel> Channels = new List<Channel>
    {
        new Channel
        {
            ChannelId = 10005,
            Name = "ITV1 HD",
            Description = "ITV is the home of high quality, popular television from the biggest entertainment events, to original drama, soaps, sport, factual series and independent news, both national, and regional.",
            Lcn = 103,
            LogoUrl = "https://msaas.img.freeviewplay.net/cache/ms/img/chan/chan/ITV1HD-Online-Logo-135x76.jpg",
            Tstv = true
        },
        new Channel
        {
            ChannelId = 1540,
            Name = "Channel 4 HD",
            Description = "Award-winning comedy, groundbreaking documentaries, distinctive drama and entertainment with an edge, now in high definition",
            Lcn = 104,
            LogoUrl = "https://msaas.img.freeviewplay.net/cache/ms/img/chan/chan/Channel4HD-Web-Logo-135x76.png",
            Tstv = false
        },
        new Channel
        {
            ChannelId = 1547,
            Name = "Channel 5 HD",
            Description = "Channel 5 HD is the home of great television for all the family, with loads of quality shows that includes entertainment, documentaries, sport, drama from the UK and US, films, reality, comedy, children's and news.",
            Lcn = 105,
            LogoUrl = "https://msaas.img.freeviewplay.net/cache/ms/img/chan/chan/C5HD-Web-Logo-135x76.jpg",
            Tstv = true
        }
    };

    private readonly AppDbContext DbContext;

    public DatabaseChannelsInit(AppDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public void SeedDatabase()
    {
        if (!DbContext.Channels.Any())
        {
            DbContext.Channels.AddRange(Channels);
            DbContext.SaveChanges();
        }
    }
}