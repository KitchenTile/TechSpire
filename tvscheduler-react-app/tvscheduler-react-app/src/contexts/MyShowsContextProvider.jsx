import { useContext, useEffect, useMemo, useState } from "react";
import MyShowsContext from "./myShowsContext";
import ChannelsContext from "./channelsContext";

// Same as the other providers, we encapsulate the logic of managing my shows array and set it as a provider (Y).
export const MyShowsContextProvider = ({ children }) => {
  const [myShows, setMyShows] = useState([]);
  const { channels } = useContext(ChannelsContext);

  useEffect(() => {
    console.log(myShows);
  }, [myShows]);

  useEffect(() => {
    const syncShows = async () => {
      if (channels) {
        const scheduledShowIds = channels.schedule.map(
          (show) => show.showEvent.showEventId
        );
        setMyShows(scheduledShowIds);
      }
    };
    syncShows();
  }, [channels]);

  const myShowsValue = useMemo(() => ({ myShows, setMyShows }), [myShows]);

  return (
    <MyShowsContext.Provider value={myShowsValue}>
      {children}
    </MyShowsContext.Provider>
  );
};

export default MyShowsContextProvider;
