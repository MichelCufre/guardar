import React, { useState, useRef } from 'react';
import Select from 'react-select';
import { useTranslation } from 'react-i18next';
import { Dropdown } from './GridCellSelectDropdown';
import { navigationRestrictedKeys } from '../Constants';
import { GridCellSelectButton } from './GridCellSelectButton';

function InternalFilterCheckbox(props) {
    const [open, setOpen] = useState(false);
    const cellRef = useRef(null);
    const selectRef = useRef(null);

    const { t } = useTranslation("translation", { useSuspense: false });

    const options = [
        { value: "", label: t("General_Sec0_lbl_FilterSelectAll") },
        { value: "S", label: t("General_Sec0_lbl_FilterSelectSelected") },
        { value: "N", label: t("General_Sec0_lbl_FilterSelectNotSelected") },
    ];

    const handleChange = (selection, { action }) => {
        if (action === "select-option") {
            props.updateFilter(props.columnId, selection.value, () => props.applyFilter());

            setOpen(false);
        }

        if (cellRef.current)
            cellRef.current.focus();
    };

    const handleKeydown = (evt) => {
        if (evt.which === 40 || evt.which === 32) {
            toggleOpen(true);
        }

        evt.preventDefault();
    };

    const handleInputKeydown = (evt) => {
        if (evt.which === 27) {
            toggleOpen(false);
            evt.preventDefault();
        }
    }

    const toggleOpen = () => {
        setOpen(!open);
    };

    const selectStyles = {
        control: () => ({ height: "0" }),
        input: () => ({ height: 0 }),
        placeholder: () => ({ display: "none" }),
        menu: () => ({})
    };

    const screenWidthDifference = open ? document.documentElement.clientWidth - cellRef.current.getBoundingClientRect().right : 0;
    const elementWidth = open ? cellRef.current.parentElement.clientWidth : 0;
    const cellLeft = open ? cellRef.current.getBoundingClientRect().left : 0;
    const cellBottom = open ? cellRef.current.getBoundingClientRect().bottom : 0;

    let displayValue = "General_Sec0_lbl_FilterSelectAll";

    switch (props.value) {
        case "S": displayValue = "General_Sec0_lbl_FilterSelectSelected";
            break;
        case "N": displayValue = "General_Sec0_lbl_FilterSelectNotSelected";
            break;
    }

    const translatedValue = t(displayValue);

    const content = {
        value: props.value,
        old: props.value
    };

    const className = open ? "open" : "";

    return (
        <Dropdown
            isOpen={open}
            onClose={toggleOpen}
            siblingsWidth={screenWidthDifference}
            parentWidth={elementWidth}
            left={cellLeft}
            bottom={cellBottom}
            target={
                <GridCellSelectButton
                    ref={cellRef}
                    onClick={toggleOpen}
                    onKeyDown={handleKeydown}
                    content={content}
                    title={translatedValue}
                    className={className}
                >
                    {translatedValue}
                </GridCellSelectButton>
            }
        >
            <Select
                ref={selectRef}
                classNamePrefix="filter-toggle-select"
                autoFocus
                backspaceRemovesValue={false}
                components={{ DropdownIndicator: null, IndicatorSeparator: null }}
                onKeyDown={handleInputKeydown}
                controlShouldRenderValue={false}
                hideSelectedOptions={false}
                isClearable={false}
                menuIsOpen
                isSearchable={false}
                styles={selectStyles}
                value={props.value}
                onChange={handleChange}
                tabSelectsValue
                options={options}
                placeholder={t("General_Sec0_lbl_SELECT_MSG")}
            />
        </Dropdown>
    );
}

function arePropsEqual(prevProps, nextProps) {
    return prevProps.value === nextProps.value && prevProps.columnId === nextProps.columnId;
}

export const FilterCheckbox = React.memo(InternalFilterCheckbox, arePropsEqual);