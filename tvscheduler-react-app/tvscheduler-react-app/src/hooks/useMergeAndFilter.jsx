import { useMemo, useContext } from "react";
import useShowLookup from "./useShowLookup";
import ChannelsContext from "../contexts/channelsContext";

// kinda messy hook, its used to... merge and filter data. I found that we were basically just doing these same operations
// and repeating the same logic over and over, so I figured I'd just make this. -- BLUE
const useMergeAndFilter = (param) => {
  const { channels } = useContext(ChannelsContext);
  const showLookup = useShowLookup(channels);

  // sorting function for the sort my schedule sort method
  const compareStartTime = (a, b) => {
    return a.timeStart - b.timeStart;
  };

  //Fisher-Yates shuffle to get random elements of an array
  const getRandomElements = (array, n) => {
    const copy = [...array];
    for (let i = copy.length - 1; i > 0; i--) {
      const random = Math.floor(Math.random() * (i + 1));
      [copy[i], copy[random]] = [copy[random], copy[i]];
    }
    return copy.slice(0, n);
  };

  // Memoize the merged show data
  const mergedShows = useMemo(() => {
    if (!channels) return [];

    return channels.channels.flatMap(
      (channel) =>
        channel.showEvents?.map((event) => ({
          ...event,
          ...showLookup[event.showId],
        })) || []
    );
  }, [channels, showLookup]);

  // Memoize the filtered sorted results
  return useMemo(() => {
    switch (param) {
      case "Morning":
        return mergedShows.filter((event) => {
          const time = new Date(event.timeStart * 1000).getHours();
          return time > 1 && time < 10;
        });

      case "Afternoon":
        return mergedShows.filter((event) => {
          const time = new Date(event.timeStart * 1000).getHours();
          return time > 10 && time < 17;
        });

      case "Evening":
        return mergedShows.filter((event) => {
          const time = new Date(event.timeStart * 1000).getHours();
          return time > 17 && time < 23;
        });

      case "All":
        return [...mergedShows].sort(compareStartTime);

      case "carousel":
        const morningShows = mergedShows
          .filter((event) => {
            const time = new Date(event.timeStart * 1000).getHours();
            return time > 1 && time < 10;
          })
          .map((show) => ({ ...show, section: "Morning" }));

        const afternoonShows = mergedShows
          .filter((event) => {
            const time = new Date(event.timeStart * 1000).getHours();
            return time > 10 && time < 17;
          })
          .map((show) => ({ ...show, section: "Afternoon" }));

        const eveningShows = mergedShows
          .filter((event) => {
            const time = new Date(event.timeStart * 1000).getHours();
            return time > 17 && time < 23;
          })
          .map((show) => ({ ...show, section: "Evening" }));

        const sectionsObject = {
          Morning: getRandomElements(morningShows, 5),
          Afternoon: getRandomElements(afternoonShows, 5),
          Evening: getRandomElements(eveningShows, 5),
        };
        return sectionsObject;

      // case for "channels" is handled elsewhere for now
      case "Channels":
        return [];

      default:
        return [];
    }
  }, [param, mergedShows]);
};

export default useMergeAndFilter;
