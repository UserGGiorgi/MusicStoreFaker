import React from 'react';

interface Props {
    region: string;
    setRegion: (r: string) => void;
    seed: number;
    setSeed: (s: number) => void;
    likes: number;
    setLikes: (l: number) => void;
    view: 'table' | 'gallery';
    setView: (v: 'table' | 'gallery') => void;
}

const Toolbar: React.FC<Props> = ({
    region, setRegion,
    seed, setSeed,
    likes, setLikes,
    view, setView
}) => {
    const randomSeed = () => setSeed(Math.floor(Math.random() * Number.MAX_SAFE_INTEGER));

    const handleExport = async () => {
        try {
            const response = await fetch(
                `/api/export?region=${region}&seed=${seed}&likes=${likes}&count=20`
            );
            if (!response.ok) throw new Error('Export failed');

            const blob = await response.blob();
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `music_export_${region}_${seed}.zip`;
            document.body.appendChild(a);
            a.click();
            a.remove();
            window.URL.revokeObjectURL(url);
        } catch (err) {
            console.error('Export failed', err);
            alert('Export failed – check console for details');
        }
    };

    return (
        <div style={{
            display: 'flex',
            flexWrap: 'wrap',
            gap: '1rem',
            padding: '1rem',
            background: '#f0f0f0',
            alignItems: 'center',
            borderBottom: '1px solid #ccc'
        }}>
            <select
                value={region}
                onChange={e => setRegion(e.target.value)}
                style={{ padding: '0.3rem' }}
            >
                <option value="en-US">English (US)</option>
                <option value="de-DE">German (DE)</option>
            </select>

            <div style={{ display: 'flex', alignItems: 'center', gap: '0.3rem' }}>
                <label>Seed:</label>
                <input
                    type="number"
                    value={seed}
                    onChange={e => setSeed(Number(e.target.value))}
                    style={{ width: '120px', padding: '0.2rem' }}
                />
                <button onClick={randomSeed} title="Random seed">
                    🎲
                </button>
            </div>

            <div style={{ display: 'flex', alignItems: 'center', gap: '0.3rem' }}>
                <label>Likes: {likes.toFixed(1)}</label>
                <input
                    type="range"
                    min="0"
                    max="10"
                    step="0.1"
                    value={likes}
                    onChange={e => setLikes(Number(e.target.value))}
                    style={{ width: '100px' }}
                />
            </div>

            <div style={{ display: 'flex', gap: '0.2rem' }}>
                <button
                    onClick={() => setView('table')}
                    disabled={view === 'table'}
                    style={{ padding: '0.3rem 0.7rem' }}
                >
                    📋 Table
                </button>
                <button
                    onClick={() => setView('gallery')}
                    disabled={view === 'gallery'}
                    style={{ padding: '0.3rem 0.7rem' }}
                >
                    🖼️ Gallery
                </button>
            </div>

            <button
                onClick={handleExport}
                style={{
                    padding: '0.3rem 1rem',
                    background: '#2e7d32',
                    color: 'white',
                    border: 'none',
                    borderRadius: 4,
                    cursor: 'pointer',
                    marginLeft: 'auto'
                }}
            >
                📦 Export ZIP
            </button>
        </div>
    );
};

export default Toolbar;