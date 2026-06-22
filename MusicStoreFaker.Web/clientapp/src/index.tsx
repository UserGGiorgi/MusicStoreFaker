import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';
import ErrorBoundary from './components/ErrorBoundary';
import reportWebVitals from './reportWebVitals';

const root = ReactDOM.createRoot(document.getElementById('root')!);
root.render(
  <React.StrictMode>
    <ErrorBoundary>
      <App />
    </ErrorBoundary>
  </React.StrictMode>
);
window.addEventListener('error', (event) => {
    if (event.message?.includes('MetaMask')) {
        event.preventDefault();
        event.stopPropagation();
    }
});

window.addEventListener('unhandledrejection', (event) => {
    if (event.reason?.message?.includes('MetaMask')) {
        event.preventDefault();
        event.stopPropagation();
    }
});
reportWebVitals();
