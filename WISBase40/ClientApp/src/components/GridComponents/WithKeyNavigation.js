import React, { Component } from 'react';

export function withKeyNavigation(WrappedComponent) {
    return class WithKeyNavigation extends Component {
        getRowElementIndex = (element) => {
            const row = element.parentElement.parentElement;
            const body = row.parentElement;

            for (var i = 0; i < body.children.length; i++) {
                if (body.children[i] === row) {
                    return i;
                }
            }
        }
        getFirstNonEmptyBody = (bodyContainer) => {
            for (var i = 0; i < bodyContainer.children.length; i++) {
                if (!bodyContainer.children[i].firstElementChild.firstElementChild.classList.contains("empty")) {                    
                    return bodyContainer.children[i].firstElementChild.firstElementChild;
                }
            }

            return null;
        }
        getLastNonEmptyBody = (bodyContainer) => {
            let lastElement;

            for (var i = 0; i < bodyContainer.children.length; i++) {
                if (!bodyContainer.children[i].firstElementChild.firstElementChild.classList.contains("empty")) {
                    lastElement = bodyContainer.children[i].firstElementChild.firstElementChild;
                }
            }

            return lastElement;
        }

        handleLeft = (target) => {
            if (target.parentElement.previousElementSibling) {
                target.parentElement.previousElementSibling.firstElementChild.focus();
                return true;
            }
            else {
                const rowIndex = this.getRowElementIndex(target);

                const bodyScroll = target.closest(".gr-body-scroll");

                if (bodyScroll.parentElement.previousElementSibling) {
                    const previousBody = bodyScroll.parentElement.previousElementSibling.querySelector(".gr-body");

                    if (previousBody.children[rowIndex] && previousBody.children[rowIndex].lastElementChild && previousBody.children[rowIndex].lastElementChild.firstElementChild) {
                        previousBody.children[rowIndex].lastElementChild.firstElementChild.focus();
                        return true;
                    }
                }
            }

            return false;            
        }
        handleRight = (target) => {
            if (target.parentElement.nextElementSibling) {
                target.parentElement.nextElementSibling.firstElementChild.focus();
                return true;
            }
            else {
                const rowIndex = this.getRowElementIndex(target);

                const bodyScroll = target.closest(".gr-body-scroll");

                if (bodyScroll.parentElement.nextElementSibling) {
                    const nextBody = bodyScroll.parentElement.nextElementSibling.querySelector(".gr-body");

                    if (nextBody.children[rowIndex] && nextBody.children[rowIndex].firstElementChild && nextBody.children[rowIndex].firstElementChild.firstElementChild) {
                        nextBody.children[rowIndex].firstElementChild.firstElementChild.focus();
                        return true;
                    }                    
                }
            }

            return false;
        }
        handleUp = (target) => {
            const index = [...target.parentElement.parentElement.children].indexOf(target.parentElement);

            if (target.parentElement.parentElement.previousElementSibling
                && target.parentElement.parentElement.previousElementSibling.children[index]
                && target.parentElement.parentElement.previousElementSibling.children[index].firstElementChild) {
                target.parentElement.parentElement.previousElementSibling.children[index].firstElementChild.focus();
                return true;
            }

            return false;
        }
        handleDown = (target) => {
            const index = [...target.parentElement.parentElement.children].indexOf(target.parentElement);

            if (target.parentElement.parentElement.nextElementSibling
                && target.parentElement.parentElement.nextElementSibling.children[index]
                && target.parentElement.parentElement.nextElementSibling.children[index].firstElementChild) {
                target.parentElement.parentElement.nextElementSibling.children[index].firstElementChild.focus();
                return true;
            }

            return false;            
        }
        handleTab = (target, shiftKey) => {
            if (shiftKey) {
                return this.handleTabLeft(target);
            }

            return this.handleTabRight(target);
        }
        handleHome = (target) => {
            const container = target.closest(".gr-body-pane-container");

            const rowIndex = this.getRowElementIndex(target);

            for (let i = 0; i < container.children.length; i++) {
                if (container.children[i].classList.contains("gr-vp-body-selection"))
                    continue;

                if (container.children[i].firstElementChild.firstElementChild.children[rowIndex].children.length > 0) {
                    container.children[i].firstElementChild.firstElementChild.children[rowIndex].firstElementChild.firstElementChild.focus();
                    break;
                }
            }

        }
        handleEnd = (target) => {
            const container = target.closest(".gr-body-pane-container");

            const rowIndex = this.getRowElementIndex(target);

            for (let i = container.children.length - 1; i >= 0; i--) {
                if (container.children[i].classList.contains("gr-vp-body-selection"))
                    continue;

                if (container.children[i].firstElementChild.firstElementChild.children[rowIndex].children.length > 0) {
                    container.children[i].firstElementChild.firstElementChild.children[rowIndex].lastElementChild.firstElementChild.focus();
                    break;
                }
            }
        }

        handleTabLeft = (target) => {
            if (target.parentElement.previousElementSibling) {
                target.parentElement.previousElementSibling.firstElementChild.focus();
                return true;
            }
            else {
                const rowIndex = this.getRowElementIndex(target);

                const bodyScroll = target.closest(".gr-body-scroll");

                if (bodyScroll.parentElement.previousElementSibling && !bodyScroll.parentElement.previousElementSibling.firstElementChild.firstElementChild.classList.contains("empty")) {
                    const previousBody = bodyScroll.parentElement.previousElementSibling.firstElementChild.firstElementChild;

                    if (previousBody.children[rowIndex] && previousBody.children[rowIndex].lastElementChild && previousBody.children[rowIndex].lastElementChild.firstElementChild) {
                        previousBody.children[rowIndex].lastElementChild.firstElementChild.focus();
                        return true;
                    }
                }
                else {
                    const bodyContainer = bodyScroll.parentElement.parentElement;

                    const body = this.getLastNonEmptyBody(bodyContainer);

                    if (body && body.children[rowIndex - 1] && body.children[rowIndex - 1].classList.contains("gr-row")) {
                        body.children[rowIndex - 1]
                            .lastElementChild
                            .lastElementChild
                            .focus();
                        return true;
                    }
                }
            }

            return false;
        }
        handleTabRight = (target) => {
            if (target.parentElement.nextElementSibling) {
                target.parentElement.nextElementSibling.firstElementChild.focus();
                return true;
            }
            else {
                const rowIndex = this.getRowElementIndex(target);

                const bodyScroll = target.closest(".gr-body-scroll");

                if (bodyScroll.parentElement.nextElementSibling && !bodyScroll.parentElement.nextElementSibling.firstElementChild.firstElementChild.classList.contains("empty")) {
                    const nextBody = bodyScroll.parentElement.nextElementSibling.firstElementChild.firstElementChild;

                    if (nextBody.children[rowIndex] && nextBody.children[rowIndex].firstElementChild && nextBody.children[rowIndex].firstElementChild.firstElementChild) {
                        nextBody.children[rowIndex].firstElementChild.firstElementChild.focus();
                        return true;
                    }
                }
                else {
                    const bodyContainer = bodyScroll.parentElement.parentElement;

                    const body = this.getFirstNonEmptyBody(bodyContainer);

                    if (body && body.children[rowIndex + 1] && body.children[rowIndex + 1].classList.contains("gr-row")) {
                        body.children[rowIndex + 1]
                            .firstElementChild
                            .firstElementChild
                            .focus();
                        return true;
                    }
                }
            }

            return false;
        }

        render() {
            return (
                <WrappedComponent
                    navigation={{
                        handleLeft: this.handleLeft,
                        handleRight: this.handleRight,
                        handleUp: this.handleUp,
                        handleDown: this.handleDown,
                        handleTab: this.handleTab,
                        handleHome: this.handleHome,
                        handleEnd: this.handleEnd
                    }}
                    {...this.props}
                />
            );
        }
    };
}