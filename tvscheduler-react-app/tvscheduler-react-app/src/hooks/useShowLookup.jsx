import { useMemo } from "react";

//this hook changed over time but it currently is used just to create a lookup table
//  with channels' (or potenitally any container's) information. This will be used
//  in multiple components that need to display shows.

const useShowLookup = (channelsData) => {
  return useMemo(() => {
    if (!channelsData || !channelsData.shows) {
      return {};
    }
    // Create a lookup for show details:
    const showLookup = {};
    channelsData.shows.$values.forEach((show) => {
      //keyvalue pair -- ID: show
      showLookup[show.showId] = show;
    });
    return showLookup;

    // // Merge each event with its corresponding show details
    // const events = channel.showEvents.$values || [];
    // const mergedShows = events.map((event) => {
    //   const showDetails = showLookup[event.showId] || {};
    //   //Add show instance's details
    //   return { ...event, ...showDetails };
    // });

    // return mergedShows;
  }, [channelsData]);
};

export default useShowLookup;
