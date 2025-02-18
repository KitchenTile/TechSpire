import { useMemo } from "react";

const useShowLookup = (channelsData) => {
  return useMemo(() => {
    if (!channelsData || !channelsData.shows) {
      return {};
    }

    const mapping = {};
    channelsData.shows.$values.forEach((show) => {
      mapping[show.showId] = show;
    });
    console.log(mapping);
    return mapping;
  }, [channelsData]);
};

export default useShowLookup;
