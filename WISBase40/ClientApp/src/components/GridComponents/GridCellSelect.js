import React, { useState, useRef } from 'react';
import Select from 'react-select';
import { textAlign } from '../Enums';
import { useTranslation } from 'react-i18next';
import { navigationRestrictedKeys } from '../Constants';
import { Dropdown } from './GridCellSelectDropdown';
import { DropdownIndicator } from './GridCellSelectDropdownIndicator';
import { withKeyNavigation } from './WithKeyNavigation';
import { GridCellEditControl } from './GridCellEditControl';
import { CellValue } from './GridCellValue';
import { GridCellSelectButton } from './GridCellSelectButton';

function InternalCellSelect(props) {
    const [inputValue, setInputValue] = useState("");
    const cellRef = useRef(null);
    const selectRef = useRef(null);
    const { t } = useTranslation("translation", { useSuspense: false });

    const handleChange = (selection, { action }) => {
        props.clearEditingCell();

        if (action === "select-option")
            props.updateCellValue(props.rowId, props.column.id, selection.value);

        if (cellRef.current)
            cellRef.current.focus();
    };

    const toggleOpen = (evt) => {
        evt.preventDefault();

        if (!props.editing) {
            props.setEditingCell(props.rowId, props.column.id);
        }
        else {
            props.clearEditingCell();
            setInputValue("");
        }
    };

    const addHighlight = (evt) => {
        props.toggleCellHighlight(evt.shiftKey);
    }
    

    const handleValue = (evt) => {
        if (!evt.ctrlKey && (!evt.shiftKey || evt.key !== "Shift")) {
            if (props.isEditingEnabled && navigationRestrictedKeys.indexOf(evt.which) === -1 && evt.key !== "Dead" && evt.which !== 32) {
                setInputValue(evt.key);
            }

            toggleOpen(evt);
        }

        evt.preventDefault();
    }

    const handleKeydown = (evt) => {
        switch (evt.which) {
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
            case 13:
                props.navigation.handleDown(evt.target);
                evt.preventDefault();
                break;
            case 35:
                props.navigation.handleEnd(evt.target);
                evt.preventDefault();
                break;
            case 36:
                props.navigation.handleHome(evt.target);
                evt.preventDefault();
                break;
            case 9:
                handleTab(evt);
                evt.preventDefault();
                break;
            default:
                handleValue(evt);
                evt.preventDefault();
        }
    };
    const handleInputKeyDown = (evt) => {
        if (evt.which === 9) {
            if (selectRef.current.select.state.focusedOption) {
                props.updateCellValue(props.rowId, props.column.id, selectRef.current.select.state.focusedOption.value); //sorry react
            }

            if (evt.shiftKey)
                props.moveToPreviousEditableCell(props.rowId, props.column.id);
            else
                props.moveToNextEditableCell(props.rowId, props.column.id);

            evt.preventDefault();
        }
        else if (evt.which === 13) {
            if (selectRef.current.select.state.focusedOption) {
                props.updateCellValue(props.rowId, props.column.id, selectRef.current.select.state.focusedOption.value);
            }

            if (props.rowIsNew) {
                props.moveToNewRowEditableCell();
            }
            else {
                props.clearEditingCell(() => {
                    if (!props.navigation.handleDown(cellRef.current)) {
                        cellRef.current.focus();
                    }
                });
            }

            evt.preventDefault();
        }
        else if (evt.which === 27) {
            props.clearEditingCell(() => cellRef.current.focus());

            evt.preventDefault();
        }
        else if (evt.which === 36) {
            props.clearEditingCell(() => props.navigation.handleHome(cellRef.current));

            evt.preventDefault();
        }
        else if (evt.which === 35) {
            props.clearEditingCell(() => props.navigation.handleEnd(cellRef.current));

            evt.preventDefault();
        }
    }
    const handleInputChange = (value, { action }) => {
        if(action === "input-change")
            setInputValue(value);
    }

    const handleTab = (evt) => {
        if (evt.shiftKey)
            props.moveToPreviousEditableCell(props.rowId, props.column.id, () => cellRef.current.parentElement.classList.toggle("selected", false));
        else
            props.moveToNextEditableCell(props.rowId, props.column.id, () => cellRef.current.parentElement.classList.toggle("selected", false));
    };

    const handleClick = (evt) => {
        if (!props.rowIsNew && evt.ctrlKey)
            props.openRelatedLink(props.rowId, props.column.id);
    };

    const getNoOptionsMessage = () => {
        return t("General_Sec0_lbl_SELECT_NO_OPTIONS");
    };

    if (!props.isEditingEnabled || !props.content.editable) {
        let className = "gr-cell-content-select";

        if (props.content.value !== props.content.old) {
            className = "gr-cell-content-select edited";
        }

        return (
            <div
                ref={cellRef}
                className={className}
                onFocus={addHighlight}
                onClick={handleClick}
                tabIndex="0"
            >
                <GridCellEditControl>
                    <CellValue value={props.content.value} translator={props.translator} translate={props.column.translate} />
                </GridCellEditControl>
            </div>
        );
    }

    const selectStyles = {
        control: provided => ({ ...provided, minWidth: 240, margin: 8 }),
        menu: () => ({ boxShadow: 'inset 0 1px 0 rgba(0, 0, 0, 0.1)' })
    };

    const screenWidthDifference = props.editing ? document.documentElement.clientWidth - cellRef.current.getBoundingClientRect().right : 0;
    const elementWidth = props.editing ? cellRef.current.parentElement.clientWidth : 0;
    const cellLeft = props.editing ? cellRef.current.getBoundingClientRect().left : 0;
    const cellBottom = props.editing ? cellRef.current.getBoundingClientRect().bottom : 0;

    var options = props.column.options;

    if (props.column.translate) {
        options = options.map(function (option) {
            return {
                "label": option.value + " - " + t(option.label),
                "value": option.value
            }
        });
    }

    return (
        <Dropdown
            isOpen={props.editing}
            onClose={toggleOpen}    
            siblingsWidth={screenWidthDifference}
            parentWidth={elementWidth}
            left={cellLeft}
            bottom={cellBottom}
            target={
                <GridCellSelectButton
                    ref={cellRef}
                    onClick={toggleOpen}
                    content={props.content}
                    onFocus={addHighlight}
                    onKeyDown={handleKeydown}
                >
                    <CellValue value={props.content.value ? props.content.value : t("General_Sec0_lbl_SELECT_MSG")} translator={props.translator} translate={props.column.translate} />
                </GridCellSelectButton>
            }
        >
            <Select
                ref={selectRef}
                autoFocus
                backspaceRemovesValue={false}
                components={{ DropdownIndicator, IndicatorSeparator: null }}
                controlShouldRenderValue={false}
                hideSelectedOptions={false}
                isClearable={false}
                menuIsOpen
                styles={selectStyles}
                value={props.content.value}
                inputValue={inputValue}
                onInputChange={handleInputChange}
                onKeyDown={handleInputKeyDown}
                onChange={handleChange}
                tabSelectsValue
                options={options}
                cacheOptions
                placeholder={t("General_Sec0_lbl_SELECT_MSG")}
                noOptionsMessage={getNoOptionsMessage}
            />
        </Dropdown>
    );
}

export const CellSelect = withKeyNavigation(InternalCellSelect);