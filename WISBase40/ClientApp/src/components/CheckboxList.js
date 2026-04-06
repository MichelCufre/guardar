import React, { useState } from 'react';
import { CheckboxListItem } from './CheckboxListItem';
import { Form } from 'react-bootstrap';

export function CheckboxList(props) {
    const [searchValue, setSearchValue] = useState("");
    const [searchFocus, setSearchFocus] = useState(false);

    const items = props.items || [];

    const handleChange = (evt) => {
        props.onSelectAllChange(evt.target.checked);
    }

    const handleSearch = (evt) => {
        setSearchValue(evt.target.value.toLowerCase());
    };

    const handleFocus = (evt) => {
        setSearchFocus(true);
    };

    const handleBlur = (evt) => {
        setSearchFocus(false);
    };

    const list = items.filter(d => d.label.toLowerCase().indexOf(searchValue) > -1).map(d => (
        <CheckboxListItem key={d.id} id={d.id} label={d.label} selected={d.selected} onChange={props.onChange} />
    ));

    const containerClass = `checkbox-list ${searchFocus ? "focused" : ""} ${props.className}`;

    const itemsStyle = props.style || {
        maxHeight: 200
    };

    return (
        <div className={containerClass}>
            <div className="checkbox-list-search">
                <div className="checkbox-list-select-all">
                    <input type="checkbox" checked={props.allSelected} onChange={handleChange} />
                </div>
                <input className="form-control border-bottom-0" type="text" onChange={handleSearch} onFocus={handleFocus} onBlur={handleBlur} />
            </div>
            <div className="checkbox-list-items" style={itemsStyle}>
                <div>
                    {list}
                </div>
            </div>
        </div>
    );
}