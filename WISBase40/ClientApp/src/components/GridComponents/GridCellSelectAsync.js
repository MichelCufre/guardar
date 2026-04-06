import debounce from "debounce-promise";
import React, { useEffect, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import Select from 'react-select/async-creatable';
import { navigationRestrictedKeys } from '../Constants';
import { Loading } from '../Loading';
import { GridCellEditControl } from './GridCellEditControl';
import { GridCellSelectButton } from './GridCellSelectButton';
import { Dropdown } from './GridCellSelectDropdown';
import { DropdownIndicator } from './GridCellSelectDropdownIndicator';
import { CellValue } from './GridCellValue';
import { withKeyNavigation } from './WithKeyNavigation';

export function InternalCellSelectAsync(props) {
    const [inputValue, setInputValue] = useState("");
    const cellRef = useRef(null);
    const selectRef = useRef(null);
    const { t } = useTranslation("translation", { useSuspense: false });

    useEffect(() => {
        if (props.editing)
            props.fixSelectGridHeight(cellRef.current.getBoundingClientRect().top, 400);
        else
            props.fixSelectGridHeight(0, 0);
    }, [props.editing]);

    const addHighlight = (evt) => {
        props.toggleCellHighlight(evt.shiftKey);
    }

    const getNoOptionsMessage = () => {
        return t("General_Sec0_lbl_SELECT_NO_OPTIONS_ASYNC");
    };

    const loadOptions = debounce(async (value) => {
        const result = await props.searchSelectValue(props.rowId, props.column.id, value);

        if (result.moreResultsAvailable)
            return [...result.options, { label: t("General_Sec0_lbl_SELECT_MSG_MORE_RESULTS"), value: "NonSelectableItem", isDisabled: true, isInfoOption: true }];

        return result.options;

    }, 1000, { leading: true });

    const loadingMessage = () => <Loading size="sm" />;

    const toggleOpen = (evt) => {
        evt.preventDefault();

        if (!props.editing) {
            props.setEditingCell(props.rowId, props.column.id);
        }
        else {
            props.clearEditingCell();
        }
    };

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
            case 8: handleBackspace(evt);
                break;
            default: handleKey(evt);
                break;
        }
    };
    const handleChange = (selection, { action }) => {
        props.clearEditingCell();

        if (action === "select-option")
            props.updateCellValue(props.rowId, props.column.id, selection.value);

        if (cellRef.current)
            cellRef.current.focus();

        props.fixSelectGridHeight(0, 0);
    };
    const handleInputKeyDown = (evt) => {
        if (evt.which === 9) {
            const selectedValue = getSelectedOption(evt);

            if (selectedValue) {
                props.updateCellValue(props.rowId, props.column.id, selectedValue.value); //sorry react
            }

            if (evt.shiftKey) {
                props.moveToPreviousEditableCell(props.rowId, props.column.id);
            }
            else {
                props.moveToNextEditableCell(props.rowId, props.column.id);
            }

            props.fixSelectGridHeight(0, 0);

            evt.preventDefault();
        }
        else if (evt.which === 13) {
            const selectedValue = getSelectedOption(evt);

            if (selectedValue) {
                props.updateCellValue(props.rowId, props.column.id, selectedValue.value);
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

            props.fixSelectGridHeight(0, 0);

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
        if (action === "input-change")
            setInputValue(value);
    }

    const handleBackspace = (evt) => {
        evt.preventDefault();

        if (props.content.editable) {
            props.clearEditingCell();

            props.fixSelectGridHeight(0, 0);

            props.updateCellValue(props.rowId, props.column.id, "");
        }
    };
    const handleKey = (evt) => {
        if (!evt.ctrlKey && (!evt.shiftKey || evt.key !== "Shift")) {
            if (props.isEditingEnabled && navigationRestrictedKeys.indexOf(evt.which) === -1 && evt.key !== "Dead" && evt.which !== 32) {
                setInputValue(evt.key);
            }

            toggleOpen(evt);
        }

        evt.preventDefault();
    };
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

    const getSelectedOption = (evt) => {
        //¯\_(ヅ)_/¯ se aceptan sugerencias
        const menuList = evt.target.closest("div[class^='select-async'][class$='container']").querySelector(".select-async__menu-list").children;

        let index = 0;

        while (index < menuList.length) {
            if (menuList[index].classList.contains("css-1n7v3ny-option"))
                break;

            index++;
        }

        if (!selectRef.current.select)
            return null;

        return selectRef.current.select.select.state.options[index];
    }

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
                onKeyDown={handleKeydown}
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
        menu: () => ({ boxShadow: 'inset 0 1px 0 rgba(0, 0, 0, 0.1)' }),
        option: (provided, op) => {
            if (op.data.isInfoOption) {

                return {
                    ...provided,
                    whiteSpace: "initial",
                    textAlign: "center"
                };
            }

            return provided;
        }
    };

    const formatCreateLabel = (input) => `${t("")} "${input}"`;

    const isValidNewOption = (inputValue) => {
        return false;
    };

    const screenWidthDifference = props.editing && cellRef.current ? document.documentElement.clientWidth - cellRef.current.getBoundingClientRect().right : 0;
    const elementWidth = props.editing && cellRef.current ? cellRef.current.parentElement.clientWidth : 0;
    const cellLeft = props.editing && cellRef.current ? cellRef.current.getBoundingClientRect().left : 0;
    const cellBottom = props.editing && cellRef.current ? cellRef.current.getBoundingClientRect().bottom : 0;

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
                    onFocus={addHighlight}
                    onKeyDown={handleKeydown}
                    content={props.content}
                >
                    <CellValue value={props.content.value ? props.content.value : t("General_Sec0_lbl_SELECT_MSG")} translator={props.translator} translate={props.column.translate} />
                </GridCellSelectButton>
            }
        >
            <Select
                ref={selectRef}
                className="select-async"
                classNamePrefix="select-async"
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
                loadOptions={loadOptions}
                cacheOptions={true}
                placeholder={t("General_Sec0_lbl_SELECT_MSG_ASYNC")}
                noOptionsMessage={getNoOptionsMessage}
                loadingMessage={loadingMessage}
                isValidNewOption={isValidNewOption}
            />
        </Dropdown>
    );
}

export const CellSelectAsync = withKeyNavigation(InternalCellSelectAsync);