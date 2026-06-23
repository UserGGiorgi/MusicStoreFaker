import React, { useEffect, useState, useRef, useCallback } from 'react';
import { SongData } from '../types';

interface Props {
    region: string;
    seed: number;
    likes: number;
}

const BATCH_SIZE = 20;

const GalleryView: React.FC<Props> = ({ region, seed, likes }) => {
    const [songs, setSongs] = useState<SongData[]>([]);
    const [startIndex, setStartIndex] = useState(1);
    const loader = useRef<HTMLDivElement>(null);

    const fetchBatch = useCallback(async (start: number) => {
        const res = await fetch(
            `/api/songs/gallery?region=${region}&seed=${seed}&likes=${likes}&startIndex=${start}&count=${BATCH_SIZE}`
        );
        const data = await res.json();
        setSongs(prev => [...prev, ...data.songs]);
        setStartIndex(data.nextStartIndex);
    }, [region, seed, likes]);

    useEffect(() => {
        setSongs([]);
        setStartIndex(1);
    }, [region, seed, likes]);

    useEffect(() => {
        if (songs.length === 0) {
            fetchBatch(1);
        }
    }, [songs, fetchBatch]);

    useEffect(() => {
        const observer = new IntersectionObserver(entries => {
            if (entries[0].isIntersecting) {
                fetchBatch(startIndex);
            }
        });
        if (loader.current) observer.observe(loader.current);
        return () => observer.disconnect();
    }, [startIndex, fetchBatch]);

    return (
        <div style={{
            display: 'grid',
            gridTemplateColumns: 'repeat(auto-fill, minmax(220px, 1fr))',
            gap: '1.5rem',
            padding: '1.5rem'
        }}>
            {songs.map(song => (
                <div
                    key={song.sequenceIndex}
                    style={{
                        border: '1px solid #e0e0e0',
                        borderRadius: 10,
                        padding: '1.2rem',
                        background: '#fff',
                        boxShadow: '0 2px 5px rgba(0,0,0,0.05)',
                        transition: 'transform 0.2s ease, box-shadow 0.2s ease'
                    }}
                    onMouseEnter={e => (e.currentTarget.style.transform = 'translateY(-3px)')}
                    onMouseLeave={e => (e.currentTarget.style.transform = 'none')}
                >
                    <h3 style={{ margin: '0 0 0.5rem', fontSize: '1.1rem' }}>{song.title}</h3>
                    <p style={{ margin: '0.3rem 0', color: '#666' }}>{song.artist}</p>
                    <p style={{ margin: '0.3rem 0', color: '#888' }}>{song.album}</p>
                    <p style={{ margin: '0.3rem 0', color: '#888' }}>{song.genre}</p>
                    <p style={{ margin: '0.5rem 0 0', fontWeight: 500 }}>Likes: {song.likes}</p>
                </div>
            ))}
            <div ref={loader} style={{ height: 20 }} />
        </div>
    );
};

export default GalleryView;