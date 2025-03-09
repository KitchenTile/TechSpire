import { useEffect, useState } from "react";
import ChannelsContext from "./channelsContext";

//this function decouples the api calls with any specific page and sets it as a context provider :)
const FetchedInfoProvider = ({ children }) => {
  const [channels, setChannels] = useState({ channels: [] });

  //fetch Data on page load
  useEffect(() => {
    const loadChannels = async () => {
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
    };
    loadChannels();
  }, []);

  return (
    <ChannelsContext.Provider value={channels}>
      {children}
    </ChannelsContext.Provider>
  );
};

export default FetchedInfoProvider;
