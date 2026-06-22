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

    useEffect(() => {
        setPage(1);
        setExpanded(null);
        setDetail(null);
        setLyrics([]);
        setCurrentLine(-1);
    }, [region, seed, likes]);

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

    useEffect(() => {
        if (currentLine >= 0) {
            const el = document.getElementById(`lyric-${currentLine}`);
            el?.scrollIntoView({ behavior: 'smooth', block: 'center' });
        }
    }, [currentLine]);

    return (
        <div>
            <table>
                <thead>
                    <tr>
                        <th>#</th>
                        <th>Title</th>
                        <th>Artist</th>
                        <th>Album</th>
                        <th>Genre</th>
                        <th>Likes</th>
                    </tr>
                </thead>
                <tbody>
                    {songs.map(song => (
                        <React.Fragment key={song.sequenceIndex}>
                            <tr onClick={() => toggleExpand(song.sequenceIndex)} style={{ cursor: 'pointer' }}>
                                <td>{song.sequenceIndex}</td>
                                <td>{song.title}</td>
                                <td>{song.artist}</td>
                                <td>{song.album}</td>
                                <td>{song.genre}</td>
                                <td>{song.likes}</td>
                            </tr>
                            {expanded === song.sequenceIndex && detail && (
                                <tr>
                                    <td colSpan={6}>
                                        <div style={{ display: 'flex', gap: '2rem', padding: '1rem', background: '#fafafa' }}>
                                            <div>
                                                <img
                                                    src={detail.coverUrl}
                                                    alt="cover"
                                                    style={{ width: 150, height: 150, borderRadius: 4 }}
                                                />
                                                <p style={{ maxWidth: 200 }}>{detail.review}</p>
                                            </div>

                                            <div style={{ flex: 1 }}>
                                                <audio
                                                    ref={audioRef}
                                                    controls
                                                    src={detail.audioUrl}
                                                    onTimeUpdate={handleTimeUpdate}
                                                    onError={(e) => {
                                                        console.error('Audio playback failed:', detail.audioUrl, e);
                                                        alert('Could not load audio for this song. Please try another one.');
                                                    }}
                                                    style={{ width: '100%', marginBottom: '1rem' }}
                                                />
                                                {lyrics.length > 0 && (
                                                    <div
                                                        style={{
                                                            maxHeight: 300,
                                                            overflow: 'auto',
                                                            border: '1px solid #ddd',
                                                            padding: '0.5rem',
                                                            borderRadius: 4,
                                                            background: '#fff'
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

            <div style={{ display: 'flex', gap: '1rem', justifyContent: 'center', margin: '1rem' }}>
                <button onClick={() => setPage(p => Math.max(1, p - 1))} disabled={page === 1}>
                    Prev
                </button>
                <span>Page {page}</span>
                <button onClick={() => setPage(p => p + 1)}>Next</button>
            </div>
        </div>
    );
};

export default TableView;