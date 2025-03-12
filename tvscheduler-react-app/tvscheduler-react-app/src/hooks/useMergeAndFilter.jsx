import { useContext, useEffect, useMemo, useState } from "react";
import useShowLookup from "./useShowLookup";
import ChannelsContext from "../contexts/channelsContext";

const useMergeAndFilter = (param) => {
  const channels = useContext(ChannelsContext);

  const showLookup = useShowLookup(channels);

  // sorting function for the sort my schedule sort method
  const compareStartTime = (a, b) => {
    return a.timeStart - b.timeStart;
  };

  // get all the shows flattened with the lookup table
  const mergedShows = channels
    ? channels?.channels
        .map((channel) => {
          if (!channel.showEvents || !channel.showEvents) return [];
          return channel.showEvents.map((event) => {
            return { ...event, ...showLookup[event.showId] };
          });
        })
        .flat()
    : null;

  switch (param) {
    case "Morning":
      const morningShows = mergedShows
        ?.filter((event) => {
          const time = new Date(event.timeStart * 1000);
          return 1 < time.getHours() && time.getHours() < 10;
        })
        .map((show) => ({ ...show, section: "Morning" }));
      return morningShows;

    case "Afternoon":
      const afternoonShows = mergedShows
        ?.filter((event) => {
          const time = new Date(event.timeStart * 1000);
          return 10 < time.getHours() && time.getHours() < 17;
        })
        .map((show) => ({ ...show, section: "Afternoon" }));
      return afternoonShows;

    case "Evening":
      const eveningShows = mergedShows
        ?.filter((event) => {
          const time = new Date(event.timeStart * 1000);
          return 17 < time.getHours() && time.getHours() < 23;
        })
        .map((show) => ({ ...show, section: "Evening" }));
      return eveningShows;

    case "All":
      const sortedAllShows = mergedShows?.sort(compareStartTime);
      return sortedAllShows;

    case "Channels":
      return [];
  }
};

export default useMergeAndFilter;
