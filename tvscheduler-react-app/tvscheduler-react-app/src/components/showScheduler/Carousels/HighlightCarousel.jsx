import { useState, useEffect } from "react";
import morning from "../../../assets/MORNING.png";

import "./HighlightCarousel.css";

const HighlightCarousel = ({ section }) => {
  const [activeShow, setActiveShow] = useState(0);

  useEffect(() => {
    setTimeout(() => {
      setActiveShow(activeShow === cards.length - 1 ? 0 : activeShow + 1);
    }, 4000);
  });

  const cards = [
    { section: "Morning", src: morning },
    { section: "Afternoon", src: morning },
    { section: "evening", src: morning },
  ];

  return (
    <div className="highlight-carousel-container">
      {cards.map((card, id) => (
        <div
          className={`carousel-card ${id === activeShow ? "" : "hidden"}`}
          key={`${card}${id}`}
        >
          <h1 className="section-title h1">{`shows to improve your ${card.section}`}</h1>
          <img src={card.src} alt="" className="carousel-image" />
        </div>
      ))}
    </div>
  );
};

export default HighlightCarousel;
