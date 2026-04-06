import React, { useRef } from 'react';
import { withKeyNavigation } from './WithKeyNavigation';

function InternalCellItemList(props) {
    const cellRef = useRef(null);
    const buttonRef = useRef(null);

    const getWidthFromSiblingsToTheRight = () => {
        let siblings = [];
        let sibling;

        sibling = cellRef.current.parentElement.nextElementSibling;

        while (sibling) {
            siblings.push(sibling);

            sibling = sibling.nextElementSibling;
        }

        return siblings.reduce((prev, curr) => prev + curr.clientWidth, 0);
    }

    function handleClick(evt) {
        evt.preventDefault();

        props.openDropdown(props.column.id, props.rowId, buttonRef.current.getBoundingClientRect().right, buttonRef.current.getBoundingClientRect().top, getWidthFromSiblingsToTheRight(), buttonRef.current.clientWidth);
    }

    const handleKeydown = (evt) => {
        switch (evt.which) {
            case 35:
                props.navigation.handleEnd(evt.target);
                evt.preventDefault();
                break;
            case 36:
                props.navigation.handleHome(evt.target);
                evt.preventDefault();
                break;
            case 37:
                props.navigation.handleLeft(evt.target);
                evt.preventDefault();
                break;
            case 39:
                props.navigation.handleRight(evt.target);
                evt.preventDefault();
                break;
            case 38:
                props.navigation.handleUp(evt.target);
                evt.preventDefault();
                break;
            case 40:
                props.navigation.handleDown(evt.target);
                evt.preventDefault();
                break;
            case 13:
                props.navigation.handleDown(evt.target);
                evt.preventDefault();
                break;
            case 9:
                props.navigation.handleTab(evt.target, evt.shiftKey);
                evt.preventDefault();
                break;
        }
    };

    const addHighlight = (evt) => {
        props.toggleCellHighlight(evt.shiftKey);
    }

    return (
        <div
            ref={cellRef}
            className="gr-cell-itemlist dropdown"
            onKeyDown={handleKeydown}
            onFocus={addHighlight}
            tabIndex={0}
        >
            <button
                ref={buttonRef}
                className="gr-btn"
                onClick={handleClick}
                title={props.label}
                disabled={props.rowIsNew || props.column.items.filter(i => props.disabledButtons.indexOf(i.id) === -1).length === 0}
            >
                <i className="fas fa-bars" />
            </button>
        </div>
    );
}

export const CellItemList = withKeyNavigation(InternalCellItemList);