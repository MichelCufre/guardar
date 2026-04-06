import React, { useEffect, useState } from 'react';

export function LanguageSelector(props) {
    const [languages, setLanguages] = useState(null);

    useEffect(() => {
        fetch("/locales/languages.json").then(d => d.json()).then(d => setLanguages(d.languages));
    }, []);

    const handleChange = (evt) => {
        props.changeLanguage(evt.target.value);
    };

    if (languages) {
        const options = languages.map(d => <option key={d.Language} value={d.Language}>{d.Name}</option>);

        return (
            <select className="language-list" value={props.selectedLanguage} onChange={handleChange}>
                {options}
            </select>
        );
    }

    return null;
}