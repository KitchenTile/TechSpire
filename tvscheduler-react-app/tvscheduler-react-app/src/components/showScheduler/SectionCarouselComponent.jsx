import { useState } from "react";
import morning from "../../assets/MORNING.png";
import "./SectionCarouselComponent.css";

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
        <div className="carousel-card" key={`${section}${id}`}>
          <h1 className="section-title h1">{`shows to improve your ${section.section}`}</h1>
          <span className="carousel-img-container">
            <img src={section.src} alt="" className="carousel-image" />
          </span>
        </div>
      ))}
    </div>
  );
};

export default SectionCarouselComponent;
