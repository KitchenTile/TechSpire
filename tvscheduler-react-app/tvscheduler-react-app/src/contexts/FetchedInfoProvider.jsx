import { useCallback, useEffect, useMemo, useState } from "react";
import ChannelsContext from "./channelsContext";

//this function decouples the api calls with any specific page and sets it as a context provider :)
const FetchedInfoProvider = ({ children }) => {
  const [channels, setChannels] = useState(null);

  const loadChannels = useCallback(async () => {
    const token = localStorage.getItem("JWToken");
    console.log(token);
    try {
      const response = await fetch("http://localhost:5171/main", {
        method: "GET",
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
      });
      if (!response.ok) {
        throw new Error("Failed to fetch channels :(");
      }
      const data = await response.json();
      setChannels(data);

      console.log("fetched data: ", data);
    } catch (error) {
      console.log("error: ", error);
    }
  }, []);
  //fetch Data on page load
  useEffect(() => {
    loadChannels();
  }, [loadChannels]);

  const contextValue = useMemo(
    () => ({ channels, refreshChannels: loadChannels }),
    [channels, loadChannels]
  );

  return (
    <ChannelsContext.Provider value={contextValue}>
      {children}
    </ChannelsContext.Provider>
  );
};

export default FetchedInfoProvider;
