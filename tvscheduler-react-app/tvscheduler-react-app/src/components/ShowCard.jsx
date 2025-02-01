// import { useEffect, useState } from "react";
import "./ShowCard.css";

const ShowCard = (show) => {
  const endTime = show.startTime - show.duration;
  //this will be devided into several components

  console.log((show = show.show));
  return (
    <div className="card-container">
      <span className="img-container">
        <img
          src={`https://msaas.img.freeviewplay.net/cache/${show.image}`}
          alt={show.name}
        />
      </span>
      <div className="info-container">
        <h2 className="show-title">{show.name}</h2>
        <div className="time-stamps">
          <span className="from">{show.startTime}</span> -{" "}
          <span className="to"></span>
        </div>
        <p className="show-description">{show.description}</p>
        <button className="add-button">+</button>
      </div>
    </div>
  );
};

export default ShowCard;
