import React from 'react';
import { useTranslation } from 'react-i18next';

export function LayoutMenuHeader(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    function handleClick(evt) {
        window.location = "/";
    }
    function handleSearch(evt) {
        props.updateSearch(evt.target.value);
    }

    if (!props.menuOpen) {
        return (
            <div className="wis-nav-header">
                <div className="wis-nav-logo-mini">
                    <a onClick={handleClick} >
                        <span>{t("Master_Sec0_metaTitle_MasterTitle")}</span>
                    </a>
                </div>
                <div className="wis-nav-open-button" onClick={props.openMenu}>
                    <span className="fa fa-cog" />
                </div>
            </div>
        );
    }
    else {

        return (
            <div className="wis-nav-header">
                <div className="wis-nav-logo">
                    <a onClick={handleClick} >                        
                        <span>{t("Master_Sec0_metaTitle_MasterTitle")}</span>
                    </a>
                </div>
                <div className="wis-nav-search">
                    <div className="wis-nav-search-field">
                        <input className="wis-nav-search-input" placeholder="Buscar" onChange={handleSearch} />
                    </div>
                </div >
                <div className="wis-nav-close-button" onClick={props.closeMenu}>
                    <span className="fa fa-chevron-left" />
                </div>
            </div>
        );
    }
}