import useIntersectionObserver from "../../hooks/useIntersectionObserver";
import LoadingComponent from "../loadingComponents/loadingComponent";
import ShowRowComponent from "./ShowRowComponenet";
import "./ChannelShowComponent.css";

const ChannelShowComponent = ({
  // we will hopefully remove the two channel props
  // channels,
  channel,
  addRemoveShow,
  myShows,
}) => {
  //options for intersection observer custom hook
  const options = {
    root: null,
    threshold: 0.1,
    rootMargin: "200px",
  };

  const [channelRef, isVisible] = useIntersectionObserver(options);

  return (
    <div
      key={channel.channelid}
      className={`channel-show-container ${isVisible ? "visible" : ""}`}
      ref={channelRef}
    >
      <div className={`title-image-container `}>
        <h3>{channel.name}</h3>
        <span className="image-container">
          <img src={channel.logoUrl} alt={channel.channelname} />
        </span>
      </div>
      {isVisible ? (
        <>
          <ShowRowComponent
            // channels={channels}
            channel={channel}
            addRemoveShow={addRemoveShow}
            myShows={myShows}
          />
        </>
      ) : (
        <LoadingComponent />
      )}
    </div>
  );
};

export default ChannelShowComponent;
