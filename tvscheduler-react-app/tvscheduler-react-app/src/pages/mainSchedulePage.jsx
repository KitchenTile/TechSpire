import React from "react";
import { useEffect, useState } from "react";
import "./MainSchedulePage.css";
import ShowCard from "../components/ShowCard";

const MainSchedulePage = () => {
  const [channels, setChannels] = useState(null);
  const [myShows, setMyShows] = useState(null);

  /*useEffect(() => {
    const loadChannels = async () => {
      try {
        const response = await fetch("http://localhost:5171");
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
  }, []);*/
  useEffect(() => {
    setChannels({
      guideData: [
        { channelid: "1", channelname: "Channel 1" },
        { channelid: "2", channelname: "Channel 2" }
      ],
      programData: {
        "1": [{ event: [{ name: "Show 1", startTime: 1700000000, duration: 3600, image: "image1.jpg", description: "Sample description for Show 1" }] }],
        "2": [{ event: [{ name: "Show 2", startTime: 1700003600, duration: 1800, image: "image2.jpg", description: "Sample description for Show 2" }] }]
      }
    });
  }, []);
  

  return (
    <>
      {channels ? (
        <>
          <h1 className="title">All Shows</h1>
          <div className="grid-container">
            {/* get the first x elements of the guide data array -- 129 is too long man */}
            {channels.guideData.slice(0, 5).map((channel) => (
              <div key={channel.channelid} className="channel-show-container">
                <h3>{channel.channelname}</h3>
                <div className="shows-row">
                  {/* if the channel has shows, get the first x shows for each channel -- consider writing a variable for more readable code */}
                  {channels.programData[channel.channelid].length > 0 ? (
                    channels.programData[channel.channelid][0].event
                      .slice(0, 5)
                      .map((show, id) => <ShowCard key={id} show={show} />)
                  ) : (
                    <p>No shows available for ${channel.name}</p>
                  )}
                </div>
              </div>
            ))}

            {/* my shows display */}
            <h1 className="title">My Shows</h1>

            <div className="myshow-container"></div>
          </div>
        </>
      ) : (
        <p>loading...</p>
      )}
    </>
  );
};

export default MainSchedulePage;
