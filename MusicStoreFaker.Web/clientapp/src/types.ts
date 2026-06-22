export interface SongData {
	sequenceIndex: number;
	title: string;
	artist: string;
	album: string;
	genre: string;
	likes: number;
	isSingle: boolean;
}

export interface SongDetail extends SongData {
	coverUrl: string;
	audioUrl: string;
	review: string;
}
export interface LyricLine {
	timeMs: number;
	text: string;
}