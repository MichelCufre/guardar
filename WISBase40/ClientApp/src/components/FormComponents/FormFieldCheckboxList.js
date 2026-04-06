import { connect } from 'formik';
import React, { useEffect, useLayoutEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { fieldType } from '../Enums';
import { FieldCheckboxListItem } from './FormFieldCheckboxListItem';
import { withFormContext } from './WithFormContext';

function FieldCheckboxListInternal(props) {
    const { t } = useTranslation();

    const [searchValue, setSearchValue] = useState("");
    const [searchFocus, setSearchFocus] = useState(false);

    const items = props.items || [];

    const fieldData = {
        props: {
            name: props.name,
            hidden: props.hidden,
            readOnly: props.readOnly,
            disabled: props.disabled,
            size: props.size
        }
    };
    useLayoutEffect(() => props.formProps.registerField(fieldData, fieldType.checkboxList), []);

    useEffect(() => {
        return () => props.formProps.unregisterField(props.name);
    }, []);

    const updateValue = async (value) => {
        await props.formik.setFieldValue(props.name, value, false);
        await props.formProps.validateField(props.name, value);
    };


    const handleSelectAllChange = async (evt) => {
        var values = [];

        list.forEach((el) => {
            var selected = !props.allSelected;
            values.push({ "id": el.props.id, "label": el.props.label, "selected": selected });
        })

        await updateValue(JSON.stringify(values));
        props.onSelectAllChange(evt.target.checked);
    }

    const handleChange = async (evt, id) => {
        var values = [];

        list.forEach((el) => {
            var selected = el.props.id == id ? !el.props.selected : el.props.selected;
            values.push({ "id": el.props.id, "label": el.props.label, "selected": selected });
        })

        await updateValue(JSON.stringify(values));
        props.onChange(evt, id);
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
        <FieldCheckboxListItem key={d.id} id={d.id} label={t(d.label)} selected={d.selected} onChange={handleChange} />
    ));

    const containerClass = `checkbox-list ${searchFocus ? "focused" : ""} ${props.className}`;

    const itemsStyle = props.style || {
        maxHeight: 200
    };

    return (
        <div className={containerClass}>
            <div className="checkbox-list-search">
                <div className="checkbox-list-select-all">
                    <input type="checkbox" checked={props.allSelected} onChange={handleSelectAllChange} />
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

export const FieldCheckboxList = withFormContext(connect(FieldCheckboxListInternal));
