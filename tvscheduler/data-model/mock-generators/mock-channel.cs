namespace tvscheduler

{
    public static class ChannelGenerator
    {
        private static readonly string[] ChannelNames =
        {
            "News Network", "Sports Central", "Movie Mania", "Reality Hub",
            "Music Vibes", "Tech Channel", "History Vault", "Kids World"
        };

        private static readonly string[] ChannelDescriptions =
        {
            "24/7 news coverage.", "Live sports events and analysis.",
            "The best movies, all day long.", "Unscripted entertainment and drama.",
            "Non-stop music videos and concerts.", "Tech innovations and trends.",
            "Deep dives into history.", "Fun and educational content for kids."
        };

        private static readonly Random Random = new();

        public static Channel GenerateMockChannel(int channelId, DateTime scheduleStartTime, int showCount, int showDuration)
        {
            var shows = new List<Show>();
            for (var i = 0; i < showCount; i++)
            {
                shows.Add(new Show
                {
                    EvtID = Random.Next(69, 420),
                    Channel = channelId,
                    Name = ChannelNames[Random.Next(0,
                        ChannelNames.Length)],
                    StartTime = scheduleStartTime.AddMinutes(i * showDuration),
                    EndTime = scheduleStartTime.AddMinutes(i * showDuration + showDuration),
                });
            }

            return new Channel
            {
                ChannelId = channelId,
                Name = ChannelNames[channelId % ChannelNames.Length], // Cycle through names
                ChannelDescription = ChannelDescriptions[channelId % ChannelDescriptions.Length],
                ShowList = shows
            };
        }
    }
}