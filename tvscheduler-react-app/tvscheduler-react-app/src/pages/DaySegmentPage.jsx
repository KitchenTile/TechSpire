import { useContext } from "react";
import { useParams } from "react-router-dom";
import ChannelsContext from "../contexts/channelsContext";

const DaySegmentPage = () => {
  const { section } = useParams();

  const channels = useContext(ChannelsContext);

  console.log(channels);

  return <div>{section}</div>;
};

export default DaySegmentPage;
