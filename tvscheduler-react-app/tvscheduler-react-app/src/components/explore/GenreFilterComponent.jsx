import { memo, useMemo } from "react";
import "./GenreFilterComponent.css";

// component to filter explore page by genre -- Rudraa
const GenreFilterComponent = ({ handleFilter, section }) => {
  const genres = useMemo(
    () => [
      "All",
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
    ],
    []
  );

  // display a tooltip of the differnet genres from the GPT call
  return (
    <div className="genres-container">
      <span>Genres</span>
      <div className="genres-list" id={section}>
        {genres.map((genre, idx) => (
          <div className="input-container" key={idx}>
            <input
              type="radio"
              id={genre}
              name="genre"
              value={genre}
              className="input"
              onClick={(event) => handleFilter(event.target.value)}
            />
            <svg>
              <polyline points="1,5 6,9 14,1"></polyline>
            </svg>
            <label htmlFor={genre}>{genre}</label>
          </div>
        ))}
      </div>
    </div>
  );
};

export default memo(GenreFilterComponent);
