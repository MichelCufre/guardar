import React from 'react';
import { Alert } from 'react-bootstrap';

export function UnexpectedErrorPage(props) {
    return (
        <Alert variant="danger" className="error-page-message">
            <div className="error-icon">
                <span className="fas fa-exclamation-triangle"></span>
            </div>
            <div className="error-content">
                <strong>ERROR INESPERADO</strong><br />
                Ocurrió un error inesperado al intentar acceder al recurso
            </div>
        </Alert>
    );
}