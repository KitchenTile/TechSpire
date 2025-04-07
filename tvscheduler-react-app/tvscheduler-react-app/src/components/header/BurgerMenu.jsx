import { useState } from "react";
import { Link } from "react-router-dom";
import "./BurgerMenu.css";
import Navigation from "./Navigation";
import ProfilePannel from "../misc/ProfilePannel";
import MyShowsComponent from "../showScheduler/myShowsComponent";

const BurgerMenu = () => {
  const [burgerBarState, setBurgerBarState] = useState("burger-bar");
  const [expanded, setExpanded] = useState(false);

  const toggleMenu = () => {
    setBurgerBarState(
      burgerBarState === "burger-bar unclicked" ||
        burgerBarState === "burger-bar"
        ? "burger-bar clicked"
        : "burger-bar unclicked"
    );
    setExpanded(!expanded);
  };

  return (
    <>
      <div className="burger-menu" onClick={toggleMenu}>
        <span className={burgerBarState} />
        <span className={burgerBarState} />
        <span className={burgerBarState} />
      </div>

      <div className={expanded ? "menu expanded" : "menu"}>
        <MyShowsComponent position={"vertical"} mobile={true} />
        <ProfilePannel mobile={true} />
        <Navigation mobile={true} />
      </div>
    </>
  );
};

export default BurgerMenu;
