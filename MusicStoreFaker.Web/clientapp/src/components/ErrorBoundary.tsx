import React, { Component, ErrorInfo, ReactNode } from 'react';

interface Props {
    children: ReactNode;
}

interface State {
    hasError: boolean;
    error: Error | null;
}

class ErrorBoundary extends Component<Props, State> {
    constructor(props: Props) {
        super(props);
        this.state = { hasError: false, error: null };
    }

    static getDerivedStateFromError(error: Error): State {
        if (error.message?.includes('MetaMask')) {
            return { hasError: false, error: null };
        }
        return { hasError: true, error };
    }

    componentDidCatch(error: Error, errorInfo: ErrorInfo) {
        if (error.message?.includes('MetaMask')) {
            console.log('Suppressed MetaMask error');
            return;
        }
        console.error('Uncaught error:', error, errorInfo);
    }

    render() {
        if (this.state.hasError) {
            return <div style={{ padding: '2rem', color: 'red' }}>Something went wrong: {this.state.error?.message}</div>;
        }
        return this.props.children;
    }
}

export default ErrorBoundary;