import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';

export function LayoutHeader() {
    const isTooManySessions = window.location.pathname === "/TooManySessions";
    const isExpiredLicense = window.location.pathname === "/ExpiredLicense";
    const isInvalidLicense = window.location.pathname === "/InvalidLicense";
    const isUserWithoutLocations = window.location.pathname === "/UserWithoutLocations";

    const request = {
        method: "POST",
        cache: "no-cache",
        headers: {
            "Content-Type": "application/json",
            "X-Requested-With": "XMLHttpRequest",
            "PreventSessionUpdate": (isTooManySessions || isExpiredLicense || isInvalidLicense || isUserWithoutLocations),
        }
    };

    const [userName, setUsername] = useState("");
    const [Predio, setUserPredio] = useState("");
    const [Predios, setUserPredios] = useState([]);
    const { t } = useTranslation("translation", { useSuspense: false });

    useEffect(() => {
        fetch("/api/Security/GetUserData", request)
            .then(d => {
                if (d.status === 401)
                    throw "Authorization Error";

                return d.json()
            })
            .then(d => {
                setUsername(d.UserName || "");
                setUserPredio(d.Predio || "");
                setUserPredios(d.Predios || []);
            })
            .catch(d => {
                if (window.location.pathname !== "/api/Security/Logout")
                    window.location = "/api/Security/Logout";
            });
    }, []);

    const handleChange = (evt) => {
        changePredio(evt.target.value);
    };

    const changePredio = (predio) => {
        setUserPredio(predio);

        const request = {
            method: "POST",
            cache: "no-cache",
            body: JSON.stringify({
                Predio: predio
            }),
            headers: {
                "Content-Type": "application/json",
                "X-Requested-With": "XMLHttpRequest"
            }
        };

        fetch("/api/Security/SetUserPredio", request)
            .then(d => {
                if (d.status === 401)
                    throw "Authorization Error";

                window.location.reload();
            })
            .catch(d => {
                if (window.location.pathname !== "/api/Security/Logout")
                    window.location = "/api/Security/Logout";
            });
    };

    const renderUserInfo = () => {
        if (Predios.length > 1) {
            const options = (['S/D'].concat(Predios)).map(p => <option key={p}>{p}</option>);
            return (
                <div className="wis-header-username">
                    <span>{userName} | {t("Master_Sec0_lbl_Predio")} </span>
                    <select className="predio-list" value={Predio} onChange={handleChange}>
                        {options}
                    </select>
                </div>
            );
        } else {
            return (
                <span className="wis-header-username">{userName} | {t("Master_Sec0_lbl_Predio")} {Predio}</span>
            );
        }
    }

    return (
        <header>
            <div className="wis-header-container">
                <div className="wis-header-title">
                    <span className="customer-brand">
                        <img src="/api/Image/Get?id=logo" alt="Logo" />
                    </span>

                    <span className="title-separator">| </span>
                    <span id="page-title" className="header-page-title"></span>

                </div>
                <div className="wis-header-user">
                    {renderUserInfo()}
                    <a className="wis-header-logout" href="/api/Security/Logout">
                        <i className="fas fa-user-times" />
                    </a>
                </div>
            </div>
        </header>
    );
}