import { useEffect, useMemo, useState } from "react";
import MyShowsContext from "./myShowsContext";

export const MyShowsContextProvider = ({ children, channels }) => {
  const [myShows, setMyShows] = useState([]);

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
