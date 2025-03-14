import "./GenreFilterComponent.css";

const GenreFilterComponent = ({ handleFilter }) => {
  const genres = [
    "All",
    "Animation",
    "Comedy",
    "Cooking",
    "Documentary",
    "Drama",
    "Game Show",
    "Horror",
    "Mystery",
    "News",
    "Reality TV",
    "Sci-Fi",
    "Thriller",
  ];

  return (
    <div className="genres-container">
      <span>Genres</span>
      <div className="genres-list">
        {genres.map((genre, idx) => (
          <div className="input-container" key={idx}>
            <input
              type="radio"
              id={genre}
              name="genre"
              value={genre}
              className="input"
              onClick={() => handleFilter(event.target.value)}
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

export default GenreFilterComponent;
