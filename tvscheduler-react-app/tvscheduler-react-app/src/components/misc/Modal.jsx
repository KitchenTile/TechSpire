import React, { useEffect, useState } from "react";
import ReactDom from "react-dom";
import "./Modal.css";

//here we create a modal using react portals -- Rudraa
const Modal = ({ open, handleModalClose }) => {
  const [selected, setSelected] = useState([]);

  const genres = {
    Documentary: 1,
    Thriller: 2,
    Comedy: 3,
    News: 4,
    Romance: 5,
    Drama: 6,
    Action: 7,
    Animation: 8,
    SciFi: 10,
    Horror: 11,
    Fantasy: 12,
  };
  // adds genre to selected genre list
  const onButtonClick = (e) => {
    const genre = Number(e.target.value);
    setSelected((prev) =>
      prev.includes(genre) ? prev.filter((g) => g !== genre) : [...prev, genre]
    );
  };

  useEffect(() => {
    console.log(selected);
  }, [selected]);

  const handleSubmit = async () => {
    if (selected.length !== 0) {
      const token = localStorage.getItem("JWToken");
      try {
        const response = await fetch(
          "http://localhost:5171/set-favourite-tags",
          {
            method: "POST",
            headers: {
              Authorization: `Bearer ${token}`,
              "Content-Type": "application/json",
            },
            body: JSON.stringify({ tagIds: selected }),
          }
        );
        if (!response.ok) {
          throw new Error("Failed to post favourite tags");
        }
      } catch (err) {
        console.error(err);
      }
      handleModalClose();
    } else {
      handleModalClose();
    }
  };

  if (!open) return null;

  // create portal to display modal
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
          {Object.entries(genres).map((genre, idx) => (
            <div key={idx}>
              <input
                type="checkbox"
                value={genre[1]}
                id={genre}
                checked={selected.includes(genre[1])}
                onChange={onButtonClick}
              />
              <label htmlFor={genre} className="genre-button">
                {genre[0]}
              </label>
            </div>
          ))}

          <button className="submit-button" onClick={handleSubmit}>
            {selected.length !== 0 ? "Done" : "Maybe later"}
          </button>
        </div>
      </div>
    </>,
    document.getElementById("portal")
  );
};

export default Modal;
