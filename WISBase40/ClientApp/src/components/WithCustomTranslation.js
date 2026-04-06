import React, { Component } from 'react';

export default function withCustomTranslation(WrappedComponent) {
    return class WithCustomTranslation extends Component {
        translate = (message, args) => {

            if (message) {
                const messageParts = message.split(" ");

                const translatedGroup = messageParts.map(d => this.props.t(d));

                let translatedMessage = translatedGroup.join(" ");

                if (args && args.length > 0) {
                    args.forEach((arg, index) => {
                        translatedMessage = translatedMessage.replace(/\{\d+\}/, arg);
                    });
                }

                return translatedMessage;
            }

            return message;
        }

        render() {
            return (
                <WrappedComponent
                    {...{ ...this.props, t: this.translate }}
                />
            );
        }
    };
}