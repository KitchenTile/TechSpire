import { useState } from "react";
import "./SectionCarouselComponent.css";
import HighlightCarousel from "./HighlightCarousel";
import morning from "../../../assets/MORNING.png";

const SectionCarouselComponent = () => {
  const [activeSection, setActiveSection] = useState(0);

  const sections = [
    { section: "Morning", src: morning },
    { section: "Afternoon", src: morning },
    { section: "evening", src: morning },
  ];

  return (
    <div className="carousel-container">
      {sections.map((section, id) => (
        <HighlightCarousel key={id} section={section} />
      ))}
      <span className="arrows left">
        <svg
          width="30"
          height="50"
          viewBox="0 0 9 15"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
          className="arrow"
        >
          <path
            d="M8.57129 2.14285L3.21415 7.5L8.57129 12.8571L7.49986 15L-0.000140326 7.5L7.49986 -3.57628e-06L8.57129 2.14285Z"
            fill="white"
          />
        </svg>
      </span>
      <soan className="arrows right">
        <svg
          width="30"
          height="50"
          viewBox="0 0 22 37"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
          className="arrow"
        >
          <path
            d="M0 31.7143L13.75 18.5L0 5.28571L2.75 0L22 18.5L2.75 37L0 31.7143Z"
            fill="white"
          />
        </svg>
      </soan>
    </div>
  );
};

export default SectionCarouselComponent;
