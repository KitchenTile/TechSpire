import "./CarouselCard.css";

const CarouselCard = ({ show, activeShow, id, addRemoveShow }) => {
  const unixToHuman = new Date(show.timeStart * 1000);

  return (
    <div
      className={`carousel-card ${id === activeShow ? "" : "hidden"}`}
      key={`${show}${id}`}
    >
      <span className="img-container">
        <img
          src={`https://msaas.img.freeviewplay.net/cache/${show.imageUrl}`}
          alt={show.name}
        />
      </span>
      <div className="info-container">
        <h2 className="show-title">{show.name}</h2>
        <div className="time-stamps">
          <span className="from">
            TODAY AT{" "}
            {unixToHuman.toLocaleTimeString("en-GB", {
              hour: "2-digit",
              minute: "2-digit",
            })}
          </span>
        </div>
        <button
          className="add-button"
          onClick={() => {
            addRemoveShow(show.showEventId);
          }}
        >
          ADD TO SCHEDULE
        </button>
        <button className="see-more">
          SEE MORE <span>{show.section}</span> SHOWS
          <span>
            <svg
              width="10"
              height="10"
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
          </span>
        </button>
      </div>
    </div>
  );
};

export default CarouselCard;
