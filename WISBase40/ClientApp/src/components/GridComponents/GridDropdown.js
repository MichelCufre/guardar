import React, { useRef, useState, useEffect } from 'react';
import { MenuItem } from './GridMenuItem';
import { useTranslation } from 'react-i18next';


export function GridDropdown(props) {
    const { t } = useTranslation();

    const dropdownRef = useRef(null);
    const timeout = useRef(false);
    const ignoreFirst = useRef(false);
    const [leftPosition, setLeftPosition] = useState(0);

    const handleMenuLeave = () => {
        if (!ignoreFirst.current) {
            ignoreFirst.current = true; //Fix para problema con reposicion de dropdown
            return false;
        }

        if (props.show) {
            timeout.current = setTimeout(() => {                
                props.closeDropdown();
            }, 300);
        }
    };

    const handleMouseEnter = () => {
        if (timeout.current) {
            clearTimeout(timeout.current);
        }
    };

    const handleClick = (evt) => {
        evt.preventDefault();

        props.performButtonAction(props.rowId, props.columnId, evt.target.id, false, evt.ctrlKey);

        props.closeDropdown();
    }

    const menuItems = props.items.map((d, index) => (
        <MenuItem
            type={d.itemType}
            key={index}
            id={d.id}
            index={index}
            label={t(d.label)}
            className={d.cssClass}
            onClick={handleClick}
        />
    ));

    useEffect(d => {
        if (props.show) {
            if (dropdownRef.current && dropdownRef.current.clientWidth > props.spaceAvailableRight) {
                setLeftPosition(props.left - dropdownRef.current.clientWidth - 5 - props.buttonWidth);
            }
            else {
                setLeftPosition(props.left + 1);
            }
        }
    }, [props.show]);

    const style = {
        display: props.show ? "block" : "none",
        left: leftPosition,
        top: props.top - 2
    };

    return (
        <div
            ref={dropdownRef}
            className="dropdown-menu"
            style={style}
            onMouseLeave={handleMenuLeave}
            onMouseEnter={handleMouseEnter}
        >
            {menuItems}
        </div>
    );
}