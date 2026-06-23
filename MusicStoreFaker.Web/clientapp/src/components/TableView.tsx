import React, { useEffect, useState, useRef } from 'react';
import { SongData, SongDetail, LyricLine } from '../types';

interface Props {
    region: string;
    seed: number;
    likes: number;
}

const PAGE_SIZE = 20;

const TableView: React.FC<Props> = ({ region, seed, likes }) => {
    const [page, setPage] = useState(1);
    const [songs, setSongs] = useState<SongData[]>([]);
    const [expanded, setExpanded] = useState<number | null>(null);
    const [detail, setDetail] = useState<SongDetail | null>(null);
    const [lyrics, setLyrics] = useState<LyricLine[]>([]);
    const [currentLine, setCurrentLine] = useState(-1);
    const audioRef = useRef<HTMLAudioElement>(null);

    // Reset on parameter change
    useEffect(() => {
        setPage(1);
        setExpanded(null);
        setDetail(null);
        setLyrics([]);
        setCurrentLine(-1);
    }, [region, seed, likes]);

    // Fetch songs for current page
    useEffect(() => {
        const fetchData = async () => {
            const res = await fetch(
                `/api/songs?region=${region}&seed=${seed}&likes=${likes}&page=${page}&pageSize=${PAGE_SIZE}`
            );
            const data = await res.json();
            setSongs(data.songs);
        };
        fetchData();
    }, [region, seed, likes, page]);

    const toggleExpand = async (index: number) => {
        if (expanded === index) {
            setExpanded(null);
            setDetail(null);
            setLyrics([]);
            setCurrentLine(-1);
            return;
        }
        setExpanded(index);

        // Fetch detail + lyrics in parallel
        const [detailRes, lyricsRes] = await Promise.all([
            fetch(`/api/songs/${index}?region=${region}&seed=${seed}`),
            fetch(`/api/songs/${index}/lyrics?seed=${seed}`)
        ]);
        const det: SongDetail = await detailRes.json();
        const lyr: LyricLine[] = await lyricsRes.json();
        setDetail(det);
        setLyrics(lyr);
        setCurrentLine(-1);
    };

    const handleTimeUpdate = () => {
        if (!audioRef.current || lyrics.length === 0) return;
        const currentTimeMs = audioRef.current.currentTime * 1000;
        const idx = lyrics.findIndex(line => line.timeMs > currentTimeMs);
        setCurrentLine(idx === -1 ? lyrics.length - 1 : idx - 1);
    };

    // Auto‑scroll active lyric into view
    useEffect(() => {
        if (currentLine >= 0) {
            const el = document.getElementById(`lyric-${currentLine}`);
            el?.scrollIntoView({ behavior: 'smooth', block: 'center' });
        }
    }, [currentLine]);

    return (
        <div style={{ width: '100%', overflowX: 'auto' }}>
            <table style={{
                width: '100%',
                borderCollapse: 'collapse',
                tableLayout: 'fixed',
                minWidth: 700
            }}>
                <thead>
                    <tr style={{ background: '#f5f5f5' }}>
                        <th style={{ width: '5%', padding: '0.75rem', textAlign: 'left' }}>#</th>
                        <th style={{ width: '25%', padding: '0.75rem', textAlign: 'left' }}>Title</th>
                        <th style={{ width: '20%', padding: '0.75rem', textAlign: 'left' }}>Artist</th>
                        <th style={{ width: '20%', padding: '0.75rem', textAlign: 'left' }}>Album</th>
                        <th style={{ width: '15%', padding: '0.75rem', textAlign: 'left' }}>Genre</th>
                        <th style={{ width: '10%', padding: '0.75rem', textAlign: 'left' }}>Likes</th>
                    </tr>
                </thead>
                <tbody>
                    {songs.map(song => (
                        <React.Fragment key={song.sequenceIndex}>
                            <tr
                                onClick={() => toggleExpand(song.sequenceIndex)}
                                style={{
                                    cursor: 'pointer',
                                    borderBottom: '1px solid #eee',
                                    background: expanded === song.sequenceIndex ? '#f0f7ff' : 'white'
                                }}
                            >
                                <td style={{ padding: '0.5rem 0.75rem' }}>{song.sequenceIndex}</td>
                                <td style={{ padding: '0.5rem 0.75rem', fontWeight: 500 }}>{song.title}</td>
                                <td style={{ padding: '0.5rem 0.75rem' }}>{song.artist}</td>
                                <td style={{ padding: '0.5rem 0.75rem' }}>{song.album}</td>
                                <td style={{ padding: '0.5rem 0.75rem' }}>{song.genre}</td>
                                <td style={{ padding: '0.5rem 0.75rem' }}>{song.likes}</td>
                            </tr>
                            {expanded === song.sequenceIndex && detail && (
                                <tr>
                                    <td colSpan={6} style={{ padding: '1.5rem', background: '#fafafa' }}>
                                        <div style={{ display: 'flex', gap: '2rem', flexWrap: 'wrap' }}>
                                            {/* Cover + Review */}
                                            <div>
                                                <img
                                                    src={detail.coverUrl}
                                                    alt="cover"
                                                    style={{ width: 180, height: 180, borderRadius: 6, boxShadow: '0 2px 8px rgba(0,0,0,0.1)' }}
                                                />
                                                <p style={{ maxWidth: 220, marginTop: '0.5rem', fontStyle: 'italic' }}>{detail.review}</p>
                                            </div>

                                            {/* Audio + Lyrics */}
                                            <div style={{ flex: 1, minWidth: 280 }}>
                                                <audio
                                                    ref={audioRef}
                                                    controls
                                                    src={detail.audioUrl}
                                                    onTimeUpdate={handleTimeUpdate}
                                                    onError={() => console.error('Audio playback failed')}
                                                    style={{ width: '100%', marginBottom: '1rem' }}
                                                />
                                                {lyrics.length > 0 && (
                                                    <div
                                                        style={{
                                                            maxHeight: 300,
                                                            overflow: 'auto',
                                                            border: '1px solid #ddd',
                                                            borderRadius: 4,
                                                            background: '#fff',
                                                            padding: '0.5rem'
                                                        }}
                                                    >
                                                        {lyrics.map((line, i) => (
                                                            <p
                                                                id={`lyric-${i}`}
                                                                key={i}
                                                                style={{
                                                                    margin: '0.3rem 0',
                                                                    fontWeight: i === currentLine ? 'bold' : 'normal',
                                                                    color: i === currentLine ? '#d32f2f' : '#333',
                                                                    background: i === currentLine ? '#fff3e0' : 'transparent',
                                                                    padding: '0.2rem 0.4rem',
                                                                    borderRadius: 4,
                                                                    transition: 'all 0.2s ease'
                                                                }}
                                                            >
                                                                {line.text}
                                                            </p>
                                                        ))}
                                                    </div>
                                                )}
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                            )}
                        </React.Fragment>
                    ))}
                </tbody>
            </table>

            <div style={{ display: 'flex', gap: '1rem', justifyContent: 'center', margin: '1.5rem 0' }}>
                <button
                    onClick={() => setPage(p => Math.max(1, p - 1))}
                    disabled={page === 1}
                    style={{ padding: '0.4rem 1rem', cursor: 'pointer' }}
                >
                    ← Prev
                </button>
                <span style={{ padding: '0.4rem 0', fontWeight: 500 }}>Page {page}</span>
                <button
                    onClick={() => setPage(p => p + 1)}
                    style={{ padding: '0.4rem 1rem', cursor: 'pointer' }}
                >
                    Next →
                </button>
            </div>
        </div>
    );
};

export default TableView;