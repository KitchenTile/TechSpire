import React, { useContext, useEffect, useState } from "react";
import "./GenreSelectionComponent.css";
import ChannelsContext from "../../contexts/channelsContext";
6.5;
//here we create a modal using react portals -- Rudraa
const GenreSelectionCompoenet = ({ handleModalClose, inModal = false }) => {
  const [selected, setSelected] = useState([]);
  const { refreshChannels, channels } = useContext(ChannelsContext);

  useEffect(() => {
    const syncTags = async () => {
      if (channels) {
        const selectedTags = channels.favTags.map((tag) => tag.tagId);
        setSelected(selectedTags);
      }
    };
    syncTags();
  }, [channels]);

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
      refreshChannels();
    }
    inModal ? handleModalClose() : null;
  };

  return (
    <>
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

        <button
          className="submit-button"
          onClick={handleSubmit}
          disabled={!inModal && selected.length === 0}
        >
          {selected.length !== 0 ? "Done" : "Maybe later"}
        </button>
      </div>
    </>
  );
};

export default GenreSelectionCompoenet;
