import { useState } from "react";
import { Link } from "react-router-dom";
import "./BurgerMenu.css";

const BurgerMenu = () => {
  const [burgerBarState, setBurgerBarState] = useState("burgerBar");
  const [menuVisible, setMenuVisible] = useState(false);
  const [activeSection, setActiveSection] = useState(null);

  const sections = {
    services: ["Tech Club", "Tech Camp", "Breakfast Club", "After School Club"],
  };

  const toggleSection = (section) => {
    setActiveSection(activeSection === section ? null : section);
  };

  const toggleMenu = () => {
    setBurgerBarState(
      burgerBarState === "burgerBar unclicked" || burgerBarState === "burgerBar"
        ? "burgerBar clicked"
        : "burgerBar unclicked"
    );
    setMenuVisible(!menuVisible);
  };

  return (
    <>
      <div className="burgerMenu" onClick={toggleMenu}>
        <span className={burgerBarState} />
        <span className={burgerBarState} />
        <span className={burgerBarState} />
      </div>

      <div
        className={menuVisible ? "menu show" : "menu hidden"}
        id="down"
        style={{ transform: menuVisible ? "scaleY(1)" : "scaleY(0)" }}
      >
        <div
          className={
            activeSection === "services"
              ? "slidingDownContainer clicked"
              : "slidingDownContainer"
          }
          onClick={() => toggleSection("services")}
          id="down"
        >
          <h3>Our Services</h3>
          <div
            className={
              activeSection === "services" ? "subMenu show" : "subMenu hidden"
            }
            style={{
              transform:
                activeSection === "services" ? "scaleY(1)" : "scaleY(0)",
            }}
            id="down"
          >
            {sections.services.map((service, index) => (
              <Link
                as={Link}
                to={`/${service.replace(/\s/g, "-").toLowerCase()}`}
                className="linkDown"
                key={index}
              >
                {service}
              </Link>
            ))}
          </div>
        </div>
        <Link as={Link} to={"/faqs"} id="account">
          FAQs
        </Link>

        <Link as={Link} to={"/about-us"} id="account">
          About Us
        </Link>

        <Link as={Link} to={"/account"} id="account">
          Account
        </Link>
      </div>
    </>
  );
};

export default BurgerMenu;
