import React, { useRef, useEffect, useState } from 'react';
import Autosuggest from 'react-autosuggest';

export function FilterText(props) {
    const inputRef = useRef(null);
    const isTypingFunction = useRef(false);
    const isTypingColumn = useRef(false);
    const isHighlighted = useRef(false);
    const [options, setOptions] = useState([]);

    const suggestions = [
        { key: "=IN", value: "=IN(value1; value2; value3...)" },
        { key: "=NOTIN", value: "=NOTIN(value1; value2; value3...)" },
        { key: "=BETWEEN", value: "=BETWEEN(value1; value2)" }
    ];

    useEffect(() => {
        if (inputRef.current && props.shouldFocus) {
            inputRef.current.focus();
        }
    }, []);

    const handleKeyDown = (evt) => {
        if (evt.which === 13 && !isHighlighted.current) {
            evt.preventDefault();

            props.applyFilter();
        }
    };

    const handleChange = (evt, { newValue, method }) => {
        if (method === "enter") {
            const isOpen = document.querySelector(".react-autosuggest__suggestions-container.react-autosuggest__suggestions-container--open");

            if (isOpen) {
                props.updateFilter(props.columnId, newValue + "(");
                evt.preventDefault();
            }
        }
        else {
            props.updateFilter(props.columnId, evt.target.value);
        }
    };

    const handleDrop = (evt) => {
        props.updateFilter(props.columnId, evt.dataTransfer.getData("text"));
        evt.preventDefault();
    };

    const handlePaste = (evt) => {
        evt.preventDefault();

        const pasted = evt.clipboardData.getData("text").trim();
        const input = evt.target;
        const current = input.value;

        if (pasted.includes("\n") || pasted.includes("\r")) {
            const cleanPasted = pasted.includes("\n") || pasted.includes("\r") ? pasted.replace(/\r?\n/g, ";") : pasted;
            const newValue = current.substring(0, input.selectionStart) + cleanPasted + ")" + current.substring(input.selectionEnd);
            props.updateFilter(props.columnId, newValue);
        } else {
            const newValue = current.substring(0, input.selectionStart) + pasted + current.substring(input.selectionEnd);
            props.updateFilter(props.columnId, newValue);
        }
    };

    const handleDragover = (evt) => {
        evt.preventDefault();
    };

    if (!props.allowsFiltering) {
        return (
            <div />
        );
    }

    const getSuggestions = value => {
        const inputValue = value.toLowerCase();

        if (inputValue.length === 0)
            return [];

        if (inputValue.indexOf(")") > -1)
            return [];

        const valueToSearch = inputValue.indexOf("(") > -1 ? inputValue.substring(0, inputValue.indexOf("(")) : inputValue;

        const option = suggestions.filter(option =>
            option.key.toLowerCase().slice(0, valueToSearch.length) === valueToSearch
        );

        return option;
    };

    // When suggestion is clicked, Autosuggest needs to populate the input
    // based on the clicked suggestion. Teach Autosuggest how to calculate the
    // input value for every given suggestion.
    const getSuggestionValue = suggestion => suggestion.key;

    // Use your imagination to render suggestions.
    const renderSuggestion = suggestion => (
        <span>
            {suggestion.value}
        </span>
    );

    // Autosuggest will call this function every time you need to update suggestions.
    // You already implemented this logic above, so just use it.
    const onSuggestionsFetchRequested = ({ value }) => {
        setOptions(getSuggestions(value));
    };

    // Autosuggest will call this function every time you need to clear suggestions.
    const onSuggestionsClearRequested = () => {
        setOptions([]);
    };

    const renderSuggestionsContainer = ({ containerProps, children, query }) => {
        const style = {
            left: inputRef.current ? inputRef.current.getBoundingClientRect().left : 0
        };

        return (
            <div {...containerProps} style={style}>
                {children}
            </div>
        );
    }

    const onSuggestionHighlighted = ({ suggestion }) => {
        isHighlighted.current = !!suggestion;
    }

    const onSuggestionSelected = (event, { suggestion, suggestionValue, suggestionIndex, sectionIndex, method }) => {
        props.updateFilter(props.columnId, suggestionValue + "(");

        event.preventDefault();
        event.stopPropagation();
    }

    const inputProps = {
        placeholder: '',
        value: props.value || "",
        ref: inputRef,
        onChange: handleChange,
        onKeyDown: handleKeyDown,
        onPaste: handlePaste
    };

    return (
        <div>
            <Autosuggest
                suggestions={options}
                onSuggestionsFetchRequested={onSuggestionsFetchRequested}
                onSuggestionsClearRequested={onSuggestionsClearRequested}
                getSuggestionValue={getSuggestionValue}
                onSuggestionHighlighted={onSuggestionHighlighted}
                onSuggestionSelected={onSuggestionSelected}
                renderSuggestion={renderSuggestion}
                renderSuggestionsContainer={renderSuggestionsContainer}
                inputProps={inputProps}
            />
            <div className="gr-filter-search-icon">
                <i className="fas fa-search" />
            </div>
        </div>
    );
}