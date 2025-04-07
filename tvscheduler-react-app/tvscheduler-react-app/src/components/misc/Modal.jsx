import React, { useContext, useEffect, useState } from "react";
import ReactDom from "react-dom";
import "./Modal.css";
import ChannelsContext from "../../contexts/channelsContext";
import GenreSelectionCompoenet from "./GenreSelectionCompoenet";

//here we create a modal using react portals -- Rudraa
const Modal = ({ open, handleModalClose, children }) => {
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
        {children}
      </div>
    </>,
    document.getElementById("portal")
  );
};

export default Modal;
