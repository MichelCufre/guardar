import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import './NavMenu.css';

export class NavMenu extends Component {
    displayName = NavMenu.name

    render() {
        return (
            <div>
                <Link to='/'>Home</Link>
                <Link to='/counter'>Counter</Link>
                <Link to='/fetchdata'>Fetch data</Link>
                <Link to='/stock/STO150'>STO150</Link>
                <Link to='/stock/STO110'>STO110</Link>
                <Link to='/stock/STO120'>STO120</Link>
                <Link to='/documento/DOC080'>DOC080</Link>
                <Link to='/documento/DOC260'>DOC260</Link>
                <Link to='/documento/DOC020'>DOC020</Link>
                <Link to='/documento/DOC020'>DOC020</Link>
                <Link to='/documento/DOC095'>DOC095</Link>
                <Link to='/documento/DOC035'>DOC035</Link>
                <Link to='/documento/DOC036'>DOC036</Link>
                <Link to='/documento/DOC036'>DOC036</Link>
                <Link to='/documento/DOC290'>DOC290</Link>
                <Link to='/documento/DOC300'>DOC300</Link>
                <Link to='/documento/DOC320'>DOC320</Link>
                <Link to='/produccion/KIT200'>KIT200</Link>
            </div>
        );
    }
}
