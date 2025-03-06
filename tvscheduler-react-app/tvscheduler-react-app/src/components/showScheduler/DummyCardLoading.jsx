import { memo } from "react";
import "./DummyCardLoading.css";

const DummyCardLoading = () => {
  //placeholder for the cards who have not rendered yet
  return (
    <div className="dummy-show">
      <span className="line"></span>
      <span className="line"></span>
      <span className="line"></span>
      <span className="add-button small">+</span>
    </div>
  );
};

export default memo(DummyCardLoading);
