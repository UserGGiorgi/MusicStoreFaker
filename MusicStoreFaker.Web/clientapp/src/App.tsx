import React, { useState } from 'react';
import Toolbar from './components/Toolbar';
import TableView from './components/TableView';
import GalleryView from './components/GalleryView';

type ViewMode = 'table' | 'gallery';

function App() {
    const [region, setRegion] = useState('en-US');
    const [seed, setSeed] = useState(42);
    const [likes, setLikes] = useState(5.0);
    const [view, setView] = useState<ViewMode>('table');

    return (
        <div className="App">
            <Toolbar
                region={region} setRegion={setRegion}
                seed={seed} setSeed={setSeed}
                likes={likes} setLikes={setLikes}
                view={view} setView={setView}
            />
            {view === 'table' ? (
                <TableView region={region} seed={seed} likes={likes} />
            ) : (
                <GalleryView region={region} seed={seed} likes={likes} />
            )}
        </div>
    );
}

export default App;