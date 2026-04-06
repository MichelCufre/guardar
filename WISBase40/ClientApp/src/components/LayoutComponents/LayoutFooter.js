import React from 'react';
import { LanguageSelector } from '../LanguageSelector';
import { useTranslation } from 'react-i18next';

export function LayoutFooter(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    return (
        <footer>
            <div className="footer-box-left" />
            <div className="footer-box-center">
                <span className="text-muted">                    
                </span>
            </div>
            <div className="footer-box-right">
                <LanguageSelector
                    selectedLanguage={props.userLanguage}
                    changeLanguage={props.changeLanguage}
                />
            </div>
        </footer>
    );
}