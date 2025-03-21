import React, { useEffect, useState } from "react";
import ReactDom from "react-dom";
import "./Modal.css";

//here we create a modal using react portals
const Modal = ({ open, handleModalClose }) => {
  const [selected, setSelected] = useState([]);

  const genres = [
    "Action",
    "Comedy",
    "Drama",
    "News",
    "Horror",
    "Sci-Fi",
    "Thriller",
    "Romance",
    "Documentary",
    "Animation",
    "Fantasy",
  ];

  const onButtonClick = (e) => {
    const genre = e.target.value;
    setSelected((prev) =>
      prev.includes(genre) ? prev.filter((g) => g !== genre) : [...prev, genre]
    );
  };

  useEffect(() => {
    console.log(selected);
  }, [selected]);

  if (!open) return null;

  return ReactDom.createPortal(
    <>
      <div className="background-modal" />
      <div className="modal-container">
        <button
          className="close-bttn"
          id="modal"
          onClick={handleModalClose}
        ></button>

        <h1 className="title">Welcome to ViewQue!</h1>
        <p>
          Please let us know your favourite tv genres so we can tailor
          recommendations for you!
        </p>
        <div className="buttons-container">
          {genres.map((genre) => (
            <>
              <input
                type="checkbox"
                value={genre}
                id={genre}
                checked={selected.includes(genre)}
                onChange={onButtonClick}
              />
              <label htmlFor={genre} className="genre-button">
                {genre}
              </label>
            </>
          ))}

          <button className="submit-button">Done</button>
        </div>
      </div>
    </>,
    document.getElementById("portal")
  );
};

export default Modal;
