import React, { useState, useEffect } from 'react';
import { LayoutMenuSection } from './LayoutMenuSection';
import { LayoutMenuHeader } from './LayoutMenuHeader';
import { useTranslation } from 'react-i18next';

export function LayoutMenu(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const [menuOpen, setOpen] = useState(false);
    const [sections, setSections] = useState([]);
    const [valueToSearch, setValueToSearch] = useState("");

    const isTooManySessions = window.location.pathname === "/TooManySessions";
    const isExpiredLicense = window.location.pathname === "/ExpiredLicense";
    const isInvalidLicense = window.location.pathname === "/InvalidLicense";
    const isUserWithoutLocations = window.location.pathname === "/UserWithoutLocations";

    if (!isTooManySessions && !isExpiredLicense && !isInvalidLicense && !isUserWithoutLocations) {
        const request = {
            method: "POST",
            cache: "no-cache",
            headers: {
                "Content-Type": "application/json",
                "X-Requested-With": "XMLHttpRequest"
            }
        };

        useEffect(() => {
            fetch("/Menu/GetMenuItems", request)
                .then(d => {
                    if (d.status === 401)
                        throw "Authorization Error";

                    return d.json();
                })
                .then(d => setSections(d || []))
                .catch(d => {
                    if (window.location.pathname !== "/api/Security/Logout")
                        window.location = "/api/Security/Logout";
                });
        }, []);
    }

    function handleTransitionEnd() {
        props.setMenuOpening(false);
    }

    function updateSearch(value) {
        setValueToSearch(value);

        setSections(sections.map(d => searchValue(d, value)));
    }
    function openMenu() {
        setOpen(true);
    }
    function closeMenu() {
        setValueToSearch("");
        setOpen(false);
    }

    function searchValue(currentElement, value, parentIsVisible) {
        let result = { ...currentElement };

        result.visible = parentIsVisible || t(result.label).toLowerCase().indexOf(value.toLowerCase()) > -1;

        if (result.submenuItems) {
            result.submenuItems = result.submenuItems.map(d => searchValue(d, value, result.visible));

            if (!result.visible && result.submenuItems.some(d => d.visible))
                result.visible = true;
        }

        return result;
    }

    const menuSections = sections.map(s => (
        <LayoutMenuSection
            key={s.id}
            id={s.id}
            label={s.label}
            icon={s.icon}
            url={s.url}
            items={s.submenuItems}
            visible={s.visible}
            isLocal={s.isLocal}
            searchValue={valueToSearch}
            menuOpen={menuOpen}
        />
    ));

    const menuClass = menuOpen ? "wis-nav-open" : "wis-nav-closed";

    return (
        <nav className={menuClass} onTransitionEnd={handleTransitionEnd}>
            <div className="wis-nav-container">
                <LayoutMenuHeader
                    updateSearch={updateSearch}
                    openMenu={openMenu}
                    closeMenu={closeMenu}
                    menuOpen={menuOpen}
                />
                <div className="wis-menu">
                    {menuSections}
                </div>
            </div>
        </nav>
    );
}