import React from 'react';
import { Alert } from 'react-bootstrap';

export function AccessDeniedPage(props) {
    return (
        <Alert variant="danger" className="error-page-message">
            <div className="error-icon">
                <span className="fas fa-exclamation-triangle"></span>
            </div>
            <div className="error-content">
                <strong>ACCESO DENEGADO</strong><br />
                Su usuario no cuenta con permisos suficientes para acceder a este recurso
            </div>
        </Alert>
    );
}
