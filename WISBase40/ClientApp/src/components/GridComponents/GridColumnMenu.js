import React, { useRef, useState, useEffect } from 'react';
import { GridColumnDropdown } from './GridColumnDropdown';
import { columnFixed } from '../Enums';
import { MenuItemButton } from './GridMenuItemButton';
import { MenuItemDivider } from './GridMenuItemDivider';
import { useTranslation } from 'react-i18next';

export function ColumnMenu(props) {
    const { t } = useTranslation();

    const [isDropdownVisible, toggleDropdown] = useState(false);
    const [leftPosition, setLeftPosition] = useState(0);
    const [topPosition, setTopPosition] = useState(0);
    const buttonRef = useRef(null);   

    const handleClick = (evt) => {
        evt.preventDefault();

        setLeftPosition(buttonRef.current.getBoundingClientRect().left);
        setTopPosition(buttonRef.current.getBoundingClientRect().bottom);

        toggleDropdown(!isDropdownVisible);
    };
    const handleDropdownClose = (evt) => {
        toggleDropdown(false);
    };
    const handleClickHide = (evt) => {
        evt.preventDefault();

        if (!props.insertable) {
            props.hideColumn(props.columnId);

            toggleDropdown(false);
        }
    };
    const handleClickFixLeft = (evt) => {
        evt.preventDefault();

        props.fixColumn(props.columnId, columnFixed.left);

        toggleDropdown(false);
    };
    const handleClickFixRight = (evt) => {
        evt.preventDefault();

        props.fixColumn(props.columnId, columnFixed.right);

        toggleDropdown(false);
    };
    const handleClickRelease = (evt) => {
        evt.preventDefault();

        props.fixColumn(props.columnId, columnFixed.none);

        toggleDropdown(false);
    };
    const handleClickSortAscending = (evt) => {
        evt.preventDefault();

        props.applySortAscending(props.columnId);

        toggleDropdown(false);
    }
    const handleClickSortDescending = (evt) => {
        evt.preventDefault();

        props.applySortDescending(props.columnId);

        toggleDropdown(false);
    }
    const handleClickSortReset = (evt) => {
        evt.preventDefault();

        props.applySortReset(props.columnId);

        toggleDropdown(false);
    }

    let menuItems = null;
    let sortItems = null;

    if (props.sort) {
        if (props.sort.direction === 1) {
            sortItems = (
                <React.Fragment>
                    <MenuItemButton
                        id="btnColumnDropdownSortReset"
                        className="fas fa-undo-alt"
                        label={t("General_Sec0_btn_SortReset")}
                        onClick={handleClickSortReset}
                    />
                    <MenuItemButton
                        id="btnColumnDropdownSortDown"
                        className="fas fa-sort-amount-down"
                        label={t("General_Sec0_btn_SortDesc")}
                        onClick={handleClickSortDescending}
                    />
                </React.Fragment>
            );
        }
        else if (props.sort.direction === 2) {
            sortItems = (
                <React.Fragment>
                    <MenuItemButton
                        id="btnColumnDropdownSortUp"
                        className="fas fa-sort-amount-up"
                        label={t("General_Sec0_btn_SortAsc")}
                        onClick={handleClickSortAscending}
                    />
                    <MenuItemButton
                        id="btnColumnDropdownSortReset"
                        className="fas fa-undo-alt"
                        label={t("General_Sec0_btn_SortReset")}
                        onClick={handleClickSortReset}
                    />
                </React.Fragment>
            );
        }
    }
    else {
        sortItems = (
            <React.Fragment>
                <MenuItemButton
                    id="btnColumnDropdownSortUp"
                    className="fas fa-sort-amount-up"
                    label={t("General_Sec0_btn_SortAsc")}
                    onClick={handleClickSortAscending}
                />
                <MenuItemButton
                    id="btnColumnDropdownSortDown"
                    className="fas fa-sort-amount-down"
                    label={t("General_Sec0_btn_SortDesc")}
                    onClick={handleClickSortDescending}
                />
            </React.Fragment>
        );
    }

    if (props.fixed === columnFixed.right) {
        menuItems = (
            <React.Fragment>
                <MenuItemButton
                    id="btnColumnDropdownHide"
                    className="fas fa-eye-slash"
                    label={t("General_Sec0_btn_Ocultar")}
                    onClick={handleClickHide}
                    disabled={props.insertable}
                />
                <MenuItemDivider />
                <MenuItemButton
                    id="btnColumnDropdownRelease"
                    className="fas fa-thumbtack fa-rotate-90"
                    label={t("General_Sec0_btn_Soltar")}
                    onClick={handleClickRelease}
                />
                <MenuItemButton
                    id="btnColumnDropdownFixLeft"
                    className="fas fa-thumbtack"
                    label={t("General_Sec0_btn_FijarIzquierda")}
                    onClick={handleClickFixLeft}
                />
                <MenuItemDivider />
                {sortItems}
            </React.Fragment>
        );
    }
    else if (props.fixed === columnFixed.left) {
        menuItems = (
            <React.Fragment>
                <MenuItemButton
                    id="btnColumnDropdownHide"
                    className="fas fa-eye-slash"
                    label={t("General_Sec0_btn_Ocultar")}
                    onClick={handleClickHide}
                    disabled={props.insertable}
                />
                <MenuItemDivider />
                <MenuItemButton
                    id="btnColumnDropdownRelease"
                    className="fas fa-thumbtack fa-rotate-90"
                    label={t("General_Sec0_btn_Soltar")}
                    onClick={handleClickRelease}
                />
                <MenuItemButton
                    id="btnColumnDropdownFixRight"
                    className="fas fa-thumbtack"
                    label={t("General_Sec0_btn_FijarDerecha")}
                    onClick={handleClickFixRight}
                />
                <MenuItemDivider />
                {sortItems}
            </React.Fragment>
        );
    }
    else {
        menuItems = (
            <React.Fragment>
                <MenuItemButton
                    id="btnColumnDropdownHide"
                    className="fas fa-eye-slash"
                    label={t("General_Sec0_btn_Ocultar")}
                    onClick={handleClickHide}
                    disabled={props.insertable}
                />
                <MenuItemDivider />
                <MenuItemButton
                    id="btnColumnDropdownFixLeft"
                    className="fas fa-thumbtack"
                    label={t("General_Sec0_btn_FijarIzquierda")}
                    onClick={handleClickFixLeft}
                />
                <MenuItemButton
                    id="btnColumnDropdownFixRight"
                    className="fas fa-thumbtack"
                    label={t("General_Sec0_btn_FijarDerecha")}
                    onClick={handleClickFixRight}
                />
                <MenuItemDivider />
                {sortItems}
            </React.Fragment>
        );
    }

    return (
        <React.Fragment>
            <div ref={buttonRef} className="gr-col-menu" onClick={handleClick}>
                <i className="fas fa-ellipsis-v"></i>
            </div>
            <GridColumnDropdown
                onClose={handleDropdownClose}
                top={topPosition}
                left={leftPosition}
                show={isDropdownVisible}
            >
                {menuItems}
            </GridColumnDropdown>
        </React.Fragment>
    );
}