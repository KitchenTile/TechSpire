import React from "react";
import { render, screen, fireEvent } from "@testing-library/react";
import ShowCard from "../components/ShowCard";

describe("ShowCard Component", () => {
  const showData = {
    name: "Test Show",
    startTime: 1700000000,
    duration: 3600,
    image: "test-image.jpg",
    description: "This is a sample description for the show.",
  };

// Test Ensures the ShowCard component renders without crashing
  test("renders ShowCard component without crashing", () => {
    render(<ShowCard show={showData} />);
    expect(screen.getByText("Test Show")).toBeInTheDocument();
  });

  // Test Checks if show description is displayed
  test("renders show description", () => {
    render(<ShowCard show={showData} />);
    expect(screen.getByText(/This is a sample description/i)).toBeInTheDocument();
  });

  // Test Ensures the component formats and displays right for the start and end times
  test("displays correct start and end times", () => {
    render(<ShowCard show={showData} />);
    
    // Convert Unix timestamp to expected time format
    const startTime = new Date(showData.startTime * 1000).toLocaleTimeString("en-GB", {
      hour: "2-digit",
      minute: "2-digit",
    });
    const endTime = new Date((showData.startTime + showData.duration) * 1000).toLocaleTimeString("en-GB", {
      hour: "2-digit",
      minute: "2-digit",
    });

    expect(screen.getByText(startTime)).toBeInTheDocument();
    expect(screen.getByText(endTime)).toBeInTheDocument();
  });

  // Test Ensures the "Read More" button expands and "Read Less" button closes
  test("toggles description text on Read More click", () => {
    render(<ShowCard show={showData} />);
    const readMoreButton = screen.getByText("Read More...");
    
    // After clicking "Read Less" button appear, and the full description is visible
    expect(screen.getByText(/This is a sample description/i)).toBeInTheDocument();
    
    fireEvent.click(readMoreButton);
    
    // After clicking, the full description should be displayed
    expect(screen.getByText("Read Less...")).toBeInTheDocument();
  });

// Test Ensures the "+" (Add) button exists in the component
  test("checks if Add button exists", () => {
    render(<ShowCard show={showData} />);
    const addButton = screen.getByText("+");
    expect(addButton).toBeInTheDocument();
  });
});
