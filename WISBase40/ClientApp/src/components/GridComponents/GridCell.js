import React, { Component } from 'react';
import { CellContent } from './GridCellContent';
import { gridStatus, textAlign } from '../Enums';
import { GridCellErrorMessage } from './GridCellErrorMessage';

export class Cell extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isHovering: false
        };

        this.cellRef = React.createRef();
        this.errorMsgRef = React.createRef();
    }

    componentDidMount() {
        if (this.props.highlighted && this.cellRef.current)
            this.cellRef.current.classList.toggle("selected", true); //Performance
    }
    shouldComponentUpdate(nextProps, nextState) {
        return this.props.content.value !== nextProps.content.value
            || this.props.content.old !== nextProps.content.old
            || this.props.content.editable !== nextProps.content.editable
            || this.props.content.status !== nextProps.content.status
            || this.props.content.error !== nextProps.content.error
            || this.props.column.width !== nextProps.column.width
            || this.props.content.cssClass !== nextProps.content.cssClass
            || this.props.editing !== nextProps.editing
            || this.state !== nextState
            || this.getUniqueId(this.props.disabledButtons) !== this.getUniqueId(nextProps.disabledButtons)
            || this.props.content.old !== this.props.content.value
            || nextProps.content.old !== nextProps.content.value;
    }

    getUniqueId(disabledButtons) {
        if (!disabledButtons || disabledButtons.length === 0)
            return "";

        let concat = "";

        for (let i = 0; i < disabledButtons.length; i++) {
            concat = concat + disabledButtons[i] + ";";
        }

        return concat;
    }

    handleMouseEnter = () => {
        this.setState({
            isHovering: true
        });
    }
    handleMouseLeave = () => {
        this.setState({
            isHovering: false
        });
    }

    getAlignment = () => {
        switch (this.props.column.textAlign) {
            case textAlign.center:
                return "align-center";
            case textAlign.right:
                return "align-right";
            default:
                return "align-left";
        }
    }

    getClassName = () => {
        const error = this.props.content.status === gridStatus.error ? " error" : "";
        const selected = this.props.editing ? "selected" : "";
        const hasLink = !this.props.rowIsNew && this.props.column.hasLink ? "has-link" : "";
        const alignment = this.getAlignment();

        return `gr-cell ${error} ${this.props.content.cssClass ? this.props.content.cssClass : ""} ${selected} ${hasLink} ${alignment}`;
    }

    getStyle = () => {
        return {
            minWidth: this.props.addExtraPixel ? this.props.column.width + 1 : this.props.column.width
        };
    }

    getErrorMessage = () => {
        if (this.props.content.error.message && this.state.isHovering)
            return (
                <GridCellErrorMessage
                    cellRef={this.cellRef}
                    content={this.props.content}
                />
            );

        return null;
    }

    toggleCellHighlight = (forceSelection) => {
        if (this.cellRef.current) {
            if (!forceSelection) {
                this.props.clearHighlighted();

                this.cellRef.current.classList.toggle("selected"); //Performance

                const clear = !this.props.isControlActive();

                this.props.toggleRowHighlight({
                    rowId: this.props.rowId,
                    columnId: this.props.column.id,
                    index: this.props.rowIndex
                }, clear);
            }
            else {
                //TODO: Implementar seleccion multiple
            }
        }
    }

    renderCellContent = () => {
        return (
            <CellContent
                type={this.props.column.type}
                rowId={this.props.rowId}
                column={this.props.column}
                disabledButtons={this.props.disabledButtons}
                rowIsNew={this.props.rowIsNew}
                rowIsDeleted={this.props.rowIsDeleted}
                content={this.props.content}
                updateCellValue={this.props.updateCellValue}
                performButtonAction={this.props.performButtonAction}
                openDropdown={this.props.openDropdown}
                searchSelectValue={this.props.searchSelectValue}

                editing={this.props.editing}
                setEditingCell={this.props.setEditingCell}
                clearEditingCell={this.props.clearEditingCell}
                moveToPreviousEditableCell={this.props.moveToPreviousEditableCell}
                moveToNextEditableCell={this.props.moveToNextEditableCell}
                moveToFirstEditableCell={this.props.moveToFirstEditableCell}
                moveToLastEditableCell={this.props.moveToLastEditableCell}
                moveToNewRowEditableCell={this.props.moveToNewRowEditableCell}

                toggleHighlight={this.props.toggleHighlight}
                clearHighlighted={this.props.clearHighlighted}
                isEditingEnabled={this.props.isEditingEnabled}
                fixSelectGridHeight={this.props.fixSelectGridHeight}

                openRelatedLink={this.props.openRelatedLink}

                toggleRowHighlight={this.props.toggleRowHighlight}
                toggleCellHighlight={this.toggleCellHighlight}

                isControlActive={this.props.isControlActive}

                gridClientLeft={this.props.gridClientLeft}

                translator={this.props.translator}
            />
        );
    }

    render() {
        if (this.props.content.status === gridStatus.error && this.props.content.error) {
            return (
                <div
                    className={this.getClassName()}
                    style={this.getStyle()}
                    ref={this.cellRef}
                    onMouseEnter={this.handleMouseEnter}
                    onMouseLeave={this.handleMouseLeave}
                >
                    {this.renderCellContent()}
                    {this.getErrorMessage()}
                </div>
            );
        }

        return (
            <div
                className={this.getClassName()}
                style={this.getStyle()}
                ref={this.cellRef}
            >
                {this.renderCellContent()}
            </div>
        );
    }
}