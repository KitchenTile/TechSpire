import { useEffect, useState } from "react";
import "./MainSchedulePage.css";
import ShowCard from "../components/ShowCard";

const MainSchedulePage = () => {
  const [channels, setChannels] = useState(null);
  const [hoveredChannel, setHoveredChannel] = useState(null);

  useEffect(() => {
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
  }, []);

  //this will be devided into several components
  return (
    <>
      <div>
        <h2>ALL CHANNELS</h2>
        {channels ? (
          <div className="channel-container">
            <ul className="channel-list">
              {channels.guideData.map((channel) => (
                <li
                  key={channel.channelid}
                  onMouseEnter={() => setHoveredChannel(channel.channelid)}
                  onMouseLeave={() => setHoveredChannel(null)}
                >
                  {channel.channelname}
                </li>
              ))}
            </ul>

            {hoveredChannel && channels.programData[hoveredChannel] && (
              <div className="selected-channel-container">
                <h3>Next up</h3>
                <h4>{channels.programData[hoveredChannel][0].event[0].name}</h4>
                <p>
                  {channels.programData[hoveredChannel][0].event[0].description}
                </p>
                <ShowCard
                  show={channels.programData[hoveredChannel][0].event[0]}
                />
              </div>
            )}
          </div>
        ) : (
          <p>loading...</p>
        )}
      </div>
    </>
  );
};

export default MainSchedulePage;
