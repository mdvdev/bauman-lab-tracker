import React from 'react';
import './Modal.css';

type ModalProps = {
    onClose: () => void;
    children: React.ReactNode;
};

const Modal: React.FC<ModalProps> = ({ onClose, children }) => {
    return (
        <div className="modal-backdrop">
            <div className="modal">
                <button className="modal-close" onClick={onClose}>×</button>
                {children}
            </div>
        </div>
    );
};

export default Modal;
