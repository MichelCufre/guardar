import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import i18n from '../i18n';

export default class MenuTablet extends Component {

    constructor(props) {
        super(props);

        document.getElementById("page-title").innerHTML = "";

        this.state = {
            sections: [
                {
                    icon: "fa fa-road",
                    id: "WPOR000_Section_Access_Poteria",
                    isLocal: true,
                    label: "Master_Menu_lbl_WPORTERIA",
                    module: null,
                    submenuItems: [
                        {
                            icon: "fa fa-truck",
                            id: "WPOR010_Section_Access_RegitroEntrada",
                            isLocal: true,
                            label: "Master_Menu_lbl_WPOTERIA_WPOR010",
                            module: "evo40",
                            url: "/porteria/POR010",
                            visible: true,
                            submenuItems: null
                        },
                        {
                            icon: "fa fa-truck mirror",
                            id: "WPOR020_Section_Access_RegitroSalida",
                            isLocal: true,
                            label: "Master_Menu_lbl_WPOTERIA_WPOR020",
                            module: "evo40",
                            url: "/porteria/POR020",
                            visible: true,
                            submenuItems: null
                        },
                        {
                            icon: "fas fa-search",
                            id: "WPOR030_Section_Access_vheiculosDeposito",
                            isLocal: true,
                            label: "Master_Menu_lbl_WPOTERIA_WPOR030",
                            module: "evo40",
                            url: "/porteria/POR030",
                            visible: true,
                            submenuItems: null
                        }
                    ]
                },

            ], loading: true
        };

    }

    subMenuSession = (subSections) => {

        if (subSections) {

            return subSections.map(s => (

                <div id={s.id} className="wis-menu-item">
                    <Link to={s.url} className="wis-item-action">
                        <i className={s.icon} />{' '}
                        <span>{i18n.t(s.label)}</span>
                    </Link>
                </div>

            ))
        }

    };

    menuSections = () => {
        return this.state.sections.map(s => (
            <div id={s.id} className="wis-menu-section">
                <a className="wis-item-label">
                    <i className={s.icon} />
                    <span>{i18n.t(s.label)}</span>
                </a>
                <div className="wis-item-submenu open">
                    {this.subMenuSession(s.submenuItems)}
                </div>

            </div>
        ));

    }


    render() {
        return (
            <div className="wis-menu-tablet">
                {this.menuSections()}
            </div>
        );
    }
}
