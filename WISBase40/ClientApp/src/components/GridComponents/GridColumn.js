import React, { useRef, useState } from 'react';
import { ColumnSortButton } from './GridColumnSortButton';
import { useTranslation } from 'react-i18next';
import { columnFixed } from '../Enums';
import { ColumnMenu } from './GridColumnMenu';

export function Column(props) {    
    const elementRef = useRef(null);
    const { t } = useTranslation();
    const dragPosition = useRef({ x: 0, y: 0 });

    const handleMouseDown = (evt) => {
        if (elementRef.current) {
            const elementOffset = props.resizeLeft ? elementRef.current.offsetLeft : elementRef.current.offsetLeft - props.getScrollLeft();
            const elementWidth = elementRef.current.offsetWidth;

            props.columnResizeBegin(props.columnId, elementOffset, elementWidth, evt.clientX, props.resizeLeft);

            evt.preventDefault();
            evt.stopPropagation();
        }
    };
    const handleClick = (evt) => {
        evt.preventDefault();

        if (props.allowsSorting) {
            props.applySort(props.columnId);
        }
    }

    const columnStyle = {
        opacity: props.isDragging ? 0.5 : 1,
        minWidth: props.width
    };

    const className = "gr-header-label" + (props.displayBorderLeft && props.isFirst ? " display-border-left" : "");

    const index = props.sorts.findIndex(s => s.columnId === props.columnId);
    const sort = props.sorts[index];

    const sortInfo = {
        direction: sort ? sort.direction : 0,
        order: (index >= 0 ? index + 1 : null)
    };

    let sortingButton = null;

    const handleDragStart = (evt) => {
        console.log("dada");
        dragPosition.current.x = evt.clientX;
        dragPosition.current.y = evt.clientY;

        evt.dataTransfer.effectAllowed = "move";

        props.setDraggedColumn({
            id: props.columnId,
            order: props.order,
            target: evt.target
        });
    }
    const handleDragEnd = (evt) => {
    }
    const handleDragOver = (evt) => {
        dragPosition.current.x = evt.clientX;
        dragPosition.current.y = evt.clientY;

        evt.preventDefault();
    }
    const handleDrop = (evt) => {
        if (!evt.target)
            return false;

        const draggedColumn = props.getDraggedColumn();

        if (!draggedColumn)
            return false;

        if (props.order === draggedColumn.order)
            return false;

        // Determine rectangle on screen
        const hoverBoundingRect = evt.target.getBoundingClientRect();

        // Get vertical middle
        const hoverMiddleX =
            (hoverBoundingRect.right - hoverBoundingRect.left) / 2;

        // Get pixels to the top
        const hoverClientX = dragPosition.current.x - hoverBoundingRect.left;

        // Time to actually perform the action
        props.updateColumnOrder(draggedColumn.id, props.columnId, draggedColumn.order, props.order, hoverClientX > hoverMiddleX);
    }

    if (props.allowsSorting) {
        sortingButton = (
            <ColumnSortButton
                columnId={props.columnId}
                direction={sortInfo.direction}
                order={sortInfo.order}
                applySort={props.applySort}
            />
        );
    }

    const colName = t(props.name);

    if (props.fixed === columnFixed.right) {
        return (
            <div
                className="gr-header-label fixed-right"
                style={columnStyle}
                ref={elementRef}
                draggable="true"
                onDragEnd={handleDragEnd}
                onDragOver={handleDragOver}
                onDragStart={handleDragStart}
                onDrop={handleDrop}
                title={colName}
            >
                <div className="gr-col-resize" onMouseDown={handleMouseDown}>
                </div>
                <div
                    className="gr-col-content"
                    onClick={handleClick}
                >
                    {sortingButton}
                    <div className="gr-col-name">
                        <strong>{colName}</strong>
                    </div>
                </div>
                <ColumnMenu
                    columnId={props.columnId}
                    sort={sort}
                    fixed={props.fixed}
                    insertable={props.insertable}
                    hideColumn={props.hideColumn}
                    fixColumn={props.fixColumn}
                    applySortAscending={props.applySortAscending}
                    applySortDescending={props.applySortDescending}
                    applySortReset={props.applySortReset}
                />
            </div>
        );
    }

    return (
        <div
            className={className}
            style={columnStyle}
            ref={elementRef}
            draggable="true"            
            onDragEnd={handleDragEnd}
            onDragOver={handleDragOver}
            onDragStart={handleDragStart}
            onDrop={handleDrop}
            title={colName}
        >
            <div
                className="gr-col-content"
                onClick={handleClick}
            >
                {sortingButton}
                <div className="gr-col-name">
                    <strong>{colName}</strong>
                </div>
            </div>
            <ColumnMenu
                columnId={props.columnId}
                sort={sort}
                fixed={props.fixed}
                insertable={props.insertable}
                hideColumn={props.hideColumn}
                fixColumn={props.fixColumn}
                applySortAscending={props.applySortAscending}
                applySortDescending={props.applySortDescending}
                applySortReset={props.applySortReset}
            />
            <div className="gr-col-resize" onMouseDown={handleMouseDown}>
                <div />
            </div>
        </div>
    );
}