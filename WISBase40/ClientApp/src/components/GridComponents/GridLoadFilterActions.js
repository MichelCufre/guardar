import React from 'react';
import { Button } from 'react-bootstrap';
import { withPageContext } from '../WithPageContext';

export function GridLoadFilterActionsInternal(props) {

    const handleLoad = () => {
        props.closeLoadFilterModal();

        props.loadFilter(props.filterId);
    };

    const handleRemove = () => {
        props.removeFilter(props.filterId);
    };

    const style = {
        width: "1.5rem"
    };

    return (
        <div className="filter-list-actions">
            <Button variant="primary" size="sm" style={style} onClick={handleLoad}>
                <i className="fas fa-check" />
            </Button>
            <Button variant="danger" size="sm" style={style} onClick={handleRemove}>
                <i className="fas fa-times" />
            </Button>
        </div>
    );
}

export const GridLoadFilterActions = withPageContext(GridLoadFilterActionsInternal);