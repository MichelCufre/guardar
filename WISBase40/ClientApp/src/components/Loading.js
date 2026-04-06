import React from 'react';
import { Spinner } from 'react-bootstrap';

export function Loading(props){
    return (
        <Spinner animation="border" role="status" size={props.size}>
            <span className="sr-only">Cargando...</span>
        </Spinner>
    );
}