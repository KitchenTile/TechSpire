import "./loadingComponent.css";

const LoadingComponent = () => {
  return (
    <div className="loading-container">
      <svg
        className="main-circle"
        viewBox="0 0 100 100"
        xmlns="http://www.w3.org/2000/svg"
      >
        <circle cx="50" cy="50" r="40" />
      </svg>
      <svg
        className="background"
        viewBox="0 0 100 100"
        xmlns="http://www.w3.org/2000/svg"
      >
        <circle cx="50" cy="50" r="40" />
      </svg>
    </div>
  );
};

export default LoadingComponent;
