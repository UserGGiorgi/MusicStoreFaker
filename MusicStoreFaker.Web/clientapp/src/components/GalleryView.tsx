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
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(200px, 1fr))', gap: '1rem' }}>
            {songs.map(song => (
                <div key={song.sequenceIndex} style={{ border: '1px solid #ccc', borderRadius: 8, padding: '0.5rem' }}>
                    <p><strong>{song.title}</strong></p>
                    <p>{song.artist}</p>
                    <p>{song.album}</p>
                    <p>{song.genre}</p>
                    <p>Likes: {song.likes}</p>
                </div>
            ))}
            <div ref={loader} style={{ height: 20 }} />
        </div>
    );
};

export default GalleryView;